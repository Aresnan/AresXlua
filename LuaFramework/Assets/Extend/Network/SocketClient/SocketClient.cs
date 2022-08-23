using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace AresLuaExtend.Network
{
	[LuaCallCSharp]
	public class SocketClient : IDisposable
	{
		[GCOptimize, LuaCallCSharp]
		public enum SocketStatus
		{
			Connecting,
			Connected,
			Closed,
			ReconnectFail
		}
		private ClientWebSocket _client;
		private CancellationTokenSource _cts;
		private SocketStatus _status;
		private Thread _receiveThread;

		private readonly List<Action> _notifyLuaActions = new List<Action>(8);
		private Uri _uri;
		private readonly LuaTable _self;

		private readonly LuaFunction _statusChangeCallback;
		private readonly LuaFunction _packageReceivedCallback;

		private static bool m_logSocketPackage;
		private static bool m_randomDelay;
		private static bool m_interrupt;
		private int m_reconnectTimes;

		private readonly SemaphoreSlim _semSlim;
		private readonly ConcurrentQueue<Tuple<ArraySegment<byte>, WebSocketMessageType, bool>> _sendingQueue;

		private float _lastReconnectTime = 0;
		private const float RECONNECT_INTERVAL = 1;

		#region Editor
#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
		private static void DebugInit()
		{
			m_logSocketPackage = UnityEditor.EditorPrefs.GetBool($"{typeof(SocketClient).FullName}{nameof(m_logSocketPackage)}", false);
			m_randomDelay = UnityEditor.EditorPrefs.GetBool($"{typeof(SocketClient).FullName}{nameof(m_randomDelay)}", false);
			UnityEditor.Menu.SetChecked("Tools/Network/Log Package", m_logSocketPackage);
			UnityEditor.Menu.SetChecked("Tools/Network/Random Delay", m_randomDelay);
		}

		[UnityEditor.MenuItem("Tools/Network/Log Package")]
		private static void LogSocketPackage()
		{
			m_logSocketPackage = !m_logSocketPackage;
			UnityEditor.Menu.SetChecked("Tools/Network/Log Package", m_logSocketPackage);
			UnityEditor.EditorPrefs.SetBool($"{typeof(SocketClient).FullName}{nameof(m_logSocketPackage)}", m_logSocketPackage);
		}

		[UnityEditor.MenuItem("Tools/Network/Random Delay")]
		private static void RandomDelay()
		{
			m_randomDelay = !m_randomDelay;
			UnityEditor.Menu.SetChecked("Tools/Network/Random Delay", m_randomDelay);
			UnityEditor.EditorPrefs.SetBool($"{typeof(SocketClient).FullName}{nameof(m_randomDelay)}", m_randomDelay);
		}

		[UnityEditor.MenuItem("Tools/Network/Interrupt")]
		private static void Interrupt()
		{
			m_interrupt = true;
		}
#endif
		#endregion

		public SocketStatus Status
		{
			get => _status;
			set
			{
				_status = value;
				lock (_notifyLuaActions)
				{
					SocketStatus status = value;
					_notifyLuaActions.Add(() => { _statusChangeCallback.Call(_self, status); });
				}
			}
		}

		public SocketClient(LuaTable self)
		{
			_cts = new CancellationTokenSource(TimeSpan.FromHours(2));
			_client = new ClientWebSocket();
			_self = self;
			_statusChangeCallback = self.GetInPath<LuaFunction>("OnStatusChanged");
			_packageReceivedCallback = self.GetInPath<LuaFunction>("OnPackageReceived");
			_semSlim = new SemaphoreSlim(1, 1);
			_sendingQueue = new ConcurrentQueue<Tuple<ArraySegment<byte>, WebSocketMessageType, bool>>();
		}

		private Tuple<string, string> m_requestHeader;
		private bool _pauseSocketEvent;

		public void SetOrUpdateHeader(string header, string value)
		{
			m_requestHeader = new Tuple<string, string>(header, value);
			_client?.Options.SetRequestHeader(header, value);
		}

		public void ConnectAsync(string uriString)
		{
			_uri = new Uri(uriString);
			TryReconnect("Connect from lua");
		}

		public void SetPauseSocketEvent(bool pause)
		{
			_pauseSocketEvent = pause;
		}


		private async Task LongRunningAsync(Func<Task> funcAsync)
		{
			while (!_cts.IsCancellationRequested)
			{
				try
				{
					await funcAsync().ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Exception inner = e;
					while (inner.InnerException != null) inner = inner.InnerException;
					if (inner is WebSocketException we)
						Debug.Log($"WebSocket Client LongRunningAsync WebSocketException: {we}");
					else if (inner is TaskCanceledException tce)
						Debug.Log($"WebSocket Client LongRunningAsync TaskCanceledException: {tce}");
					else
					{
						Debug.Log($"WebSocket Client LongRunningAsync error: {e}");
						continue;
					}

					break;
				}
			}

			Status = SocketStatus.Closed;
		}

		public void Send(string json)
		{
			if (Status != SocketStatus.Connected)
			{
				Debug.LogWarning("websocket is not connected,send message failed");
				return;
			}

			if (m_logSocketPackage)
			{
				Debug.Log("Send : " + json);
			}

			var buff = Encoding.UTF8.GetBytes(json);
			ArraySegment<byte> sendBuffer = new ArraySegment<byte>(buff);
			_sendingQueue.Enqueue(Tuple.Create(sendBuffer, WebSocketMessageType.Text, true));
			_semSlim.Release();
		}

		public void Tick()
		{
			if (_pauseSocketEvent)
			{
				return;
			}
			if (m_interrupt)
			{
				m_interrupt = false;
				Close();
				return;
			}

			lock (_notifyLuaActions)
			{
				for (int i = 0; i < _notifyLuaActions.Count; i++)
				{
					var notify = _notifyLuaActions[i];
					notify.Invoke();

					if (_pauseSocketEvent)
					{
						return;
					}
				}

				_notifyLuaActions.Clear();
			}


			if (Status == SocketStatus.Closed)
			{
				TryReconnect("Socket closed");
			}
		}


		public async void Close()
		{
			if (Status == SocketStatus.Closed)
			{
				return;
			}

			Debug.LogWarning("Socket closed");
			lock (_notifyLuaActions)
			{
				_notifyLuaActions.Clear();
			}

			try
			{
				await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "client request close", CancellationToken.None);
				_client.Dispose();
			}
			catch (Exception)
			{
				// ignored
			}

			_cts = new CancellationTokenSource(TimeSpan.FromHours(2));
			_client = new ClientWebSocket();
			_client.Options.SetRequestHeader(m_requestHeader.Item1, m_requestHeader.Item2);
			Status = SocketStatus.Closed;
		}

		public void Reconnect()
		{
			Close();

			m_reconnectTimes = 0;
			Status = SocketStatus.Closed;
		}

		private async void ReceiveThread()
		{
			var buff = new byte[65536];
			ArraySegment<byte> receiveBuff = new ArraySegment<byte>(buff);
			var memoryStream = new MemoryStream(65536);
			var currentClient = _client;
			while (true)
			{
				try
				{
					if (_cts.IsCancellationRequested)
					{
						Debug.LogWarning($"Receive error _cts.IsCancellationRequested ");
						break;
					}

					if (currentClient.State != WebSocketState.Open)
					{
						Debug.LogWarning($"Receive error not WebSocketState.Open State Current status is : {currentClient.State}");
						break;
					}

					WebSocketReceiveResult result;
					try
					{
						result = await _client.ReceiveAsync(receiveBuff, _cts.Token);
					}
					catch (Exception e)
					{
						var inner = e;
						while (inner.InnerException != null)
						{
							inner = inner.InnerException;
						}

						if (inner is WebSocketException)
						{
							Debug.LogWarning($"Receive error with exception : {inner}");
						}
						else if (inner is TaskCanceledException)
						{
							Debug.LogWarning($"Receive error with exception : {inner}");
						}
						else
						{
							Debug.LogWarning($"Receive ignore error : {inner}");
							continue;
						}

						break;
					}

					if (result is { MessageType: WebSocketMessageType.Close })
					{
						Debug.LogWarning("Remote close the session.");
						Status = SocketStatus.Closed;
						break;
					}

					if (receiveBuff.Array != null)
						memoryStream.Write(receiveBuff.Array, 0, result.Count);

					if (result.EndOfMessage)
					{
						var json = Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
						if (m_logSocketPackage)
						{
							Debug.Log("Receive : " + json);
						}

						if (m_randomDelay)
						{
							Thread.Sleep(500);
						}

						lock (_notifyLuaActions)
						{
							_notifyLuaActions.Add(() => { _packageReceivedCallback.Call(_self, json); });
						}

						memoryStream.Position = 0;
					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					Status = SocketStatus.Closed;
					break;
				}
			}

			if (currentClient == _client)
			{
				Debug.LogWarning("Receive error close.");
				Close();
			}
		}

		private Task StartSendTaskAsync()
		{
			return LongRunningAsync(async () =>
			{
				await _semSlim.WaitAsync(_cts.Token).ConfigureAwait(false);
				while (_sendingQueue.TryDequeue(out var data))
				{
					await _client.SendAsync(data.Item1, data.Item2, data.Item3, _cts.Token).ConfigureAwait(false);
				}
			});
		}

		private void TryReconnect(string reason)
		{
			if (Time.realtimeSinceStartup - _lastReconnectTime < RECONNECT_INTERVAL)
			{
				return;
			}
			m_reconnectTimes++;
			_lastReconnectTime = Time.realtimeSinceStartup;
			if (m_reconnectTimes > 5)
			{
				Status = SocketStatus.ReconnectFail;
				return;
			}

			Status = SocketStatus.Connecting;
			Debug.LogWarning($"Try to connect : {_uri} {reason}");

			if (_client.State == WebSocketState.Connecting || _client.State == WebSocketState.Open)
			{
				return;
			}

			if (_client.State != WebSocketState.None && _client.State != WebSocketState.Closed)
			{
				Close();
			}

			_client.ConnectAsync(_uri, _cts.Token).ContinueWith(task =>
			{
				if (task.Exception != null)
				{
					Debug.LogException(task.Exception);
					Status = SocketStatus.Closed;
					Close();
				}
				else
				{
					Task.WhenAll(StartSendTaskAsync()).ConfigureAwait(false);
					Status = SocketStatus.Connected;

					_receiveThread = new Thread(ReceiveThread);
					_receiveThread.Start();
				}
			});
		}

		public void Dispose()
		{
			Debug.LogWarning("Socket client disposed");
			_self.Dispose();
			_statusChangeCallback.Dispose();
			_packageReceivedCallback.Dispose();

			_client?.Dispose();
			_cts?.Dispose();
		}
	}
}
