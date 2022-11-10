using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AresLuaExtend.Network
{
	//TODO CustomYieldInstruction
	public class AresDownload : CustomYieldInstruction
	{
        /// <summary>
        ///     等待下载的队列
        /// </summary>
        private static readonly List<AresDownload> Prepared = new List<AresDownload>();
        public override bool keepWaiting => throw new System.NotImplementedException();
        /// <summary>
        ///     下载缓存，防止重复下载
        /// </summary>
        private static readonly Dictionary<string, AresDownload> Cache = new Dictionary<string, AresDownload>();
        public long downloadedBytes { get; private set; }
        public DownloadStatus status { get; private set; }
        public DownloadInfo info { get; private set; }
        public float progress => downloadedBytes * 1f / info.size;
        public Action<AresDownload> completed { get; set; }
        /// <summary>
        ///     获取当前下载的总带宽，可以用来显示速度
        /// </summary>
        public static long TotalBandwidth { get; private set; }

        /// <summary>
        ///     当前总下载的字节数
        /// </summary>
        public static long TotalDownloadedBytes
        {
            get
            {
                var value = 0L;
                foreach (var item in Cache) value += item.Value.downloadedBytes;

                return value;
            }
        }

        /// <summary>
        ///     当前总下载大小
        /// </summary>
        public static long TotalSize
        {
            get
            {
                var value = 0L;
                foreach (var item in Cache) value += item.Value.info.size;

                return value;
            }
        }

        public static AresDownload DownloadAsync(string url, string savePath, Action<AresDownload> completed = null,
    long size = 0, string hash = null)
        {
            return DownloadAsync(new DownloadInfo
            {
                url = url,
                savePath = savePath,
                hash = hash,
                size = size
            }, completed);
        }

        public static AresDownload DownloadAsync(DownloadInfo info, Action<AresDownload> completed = null)
        {
            if (!Cache.TryGetValue(info.url, out var download))
            {
                download = new AresDownload
                {
                    info = info
                };
                Prepared.Add(download);
                Cache.Add(info.url, download);
            }
            else
            {
                Debug.LogWarning($"Download url {0} already exist.{info.url}");
            }

            if (completed != null) download.completed += completed;

            return download;
        }
    }


    public class DownloadInfo
    {
        public string hash;
        public string savePath;
        public long size;
        public string url;

        public long downloadedSize
        {
            get
            {
                var info = new FileInfo(savePath);
                if (info.Exists)
                {
                    return info.Length;
                }
                return 0;
            }
        }

        public long downloadSize => size - downloadedSize;
    }
}