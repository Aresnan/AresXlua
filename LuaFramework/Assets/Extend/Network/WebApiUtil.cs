using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AresLuaExtend.Network
{
	public static class WebApiUtil
	{
		/// <summary>
		/// 拼接Get链接
		/// </summary>
		/// <param name="url"></param>
		/// <param name="dic"></param>
		/// <returns></returns>
		public static string BuildGetUrl(this string url, Dictionary<string, string> dic)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(url);
			if (dic.Count > 0)
			{
				builder.Append("?");
				int i = 0;
				foreach (var item in dic)
				{
					if (i > 0)
						builder.Append("&");
					builder.AppendFormat("{0}={1}", item.Key, item.Value);
					i++;
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Get方法获取数据
		/// </summary>
		/// <param name="url"></param>
		/// <param name="callback"></param>
		/// <param name="headers"></param>
		/// <param name="responseHeaderNames"></param>
		public static void Get(this string url, Action<Dictionary<string, string>> callback,
	Dictionary<string, string> headers, params string[] responseHeaderNames)
		{
			var task = Send(UnityWebRequest.Get(url), headers, responseHeaderNames);
			if (callback == null) return;
			var awaiter = task.GetAwaiter();
			awaiter.OnCompleted(() => callback.Invoke(awaiter.GetResult()));
		}

		private static async Task<Dictionary<string, string>> Send(UnityWebRequest request,
	Dictionary<string, string> headers,
	string[] responseHeaderNames)
		{
			using (request)
			{
				//加入请求header
				if (headers != null)
				{
					foreach (var header in headers)
						request.SetRequestHeader(header.Key, header.Value);
				}

				var operation = request.SendWebRequest();
				while (!operation.isDone)
					await Task.Yield();
				//如果请求错误，则返回空值
				if (!string.IsNullOrEmpty(request.error))
				{
					Debug.LogError($"unity web request : {request.url} error : {request.error}");
					return null;
				}

				//加入返回的数据
				var result = new Dictionary<string, string> { { "result", request.downloadHandler.text } };
				//加入需要返会的header
				foreach (var headerName in responseHeaderNames)
					result.Add(headerName, request.GetResponseHeader(headerName));

				return result;
			}
		}
	}
}