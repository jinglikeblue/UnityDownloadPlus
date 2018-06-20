using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;

namespace Jing.Net
{
    /// <summary>
    /// 下载器
    /// </summary>
    public class Downloader
    {
        /// <summary>
        /// 取消下载的错误描述
        /// </summary>
        const string CANCELLED_ERROR = "Cancelled";

        /// <summary>
        /// 重写的WebClient类
        /// </summary>
        class DownloadWebClient : WebClient
        {
            readonly int _timeout;
            public DownloadWebClient(int timeout = 60)
            {
                if (null == ServicePointManager.ServerCertificateValidationCallback)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                _timeout = timeout * 1000;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                //request.ProtocolVersion = HttpVersion.Version10;
                request.Timeout = _timeout;
                request.ReadWriteTimeout = _timeout;
                request.Proxy = null;
                return request;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                return base.GetWebResponse(request);
            }

            private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                //总是接受,通过Https验证
                return true;
            }
        }

        DownloadWebClient _client;

        bool _isDone;

        /// <summary>
        /// 是否操作完成
        /// </summary>
        public bool isDone
        {
            get
            {
                return _isDone;
            }
        }

        float _progress;

        /// <summary>
        /// 操作进度
        /// </summary>
        public float progress
        {
            get
            {
                return _progress;
            }
        }

        string _error;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error
        {
            get
            {
                return _error;
            }
        }

        long _totalSize;

        /// <summary>
        /// 文件总大小
        /// </summary>
        public long totalSize
        {
            get
            {
                return _totalSize;
            }
        }

        long _loadedSize;

        /// <summary>
        /// 已完成大小
        /// </summary>
        public long loadedSize
        {
            get
            {
                return _loadedSize;
            }
        }

        /// <summary>
        /// 是否已销毁
        /// </summary>
        public bool isDisposeed
        {
            get { return _client == null ? true : false; }
        }


        readonly string _savePath;

        /// <summary>
        /// 文件的保存路径
        /// </summary>
        public string savePath
        {
            get { return _savePath; }
        }

        readonly bool _isAutoDeleteWrongFile;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">下载的文件地址</param>
        /// <param name="savePath">文件保存的地址</param>
        /// <param name="isAutoDeleteWrongFile">是否自动删除未下完的文件</param>
        public Downloader(string url, string savePath, bool isAutoDeleteWrongFile = true)
        {
            _isAutoDeleteWrongFile = isAutoDeleteWrongFile;
            _savePath = savePath;
            _client = new DownloadWebClient();
            _client.DownloadProgressChanged += OnDownloadProgressChanged;
            _client.DownloadFileCompleted += OnDownloadFileCompleted;

            Uri uri = new Uri(url);
            _client.DownloadFileAsync(uri, savePath);
        }

        /// <summary>
        /// 销毁对象，会停止所有的下载
        /// </summary>
        public void Dispose()
        {
            if (null != _client)
            {
                _client.CancelAsync();
            }
        }

        void ClearClient()
        {
            _client.DownloadProgressChanged -= OnDownloadProgressChanged;
            _client.DownloadFileCompleted -= OnDownloadFileCompleted;
            _client.Dispose();
            _client = null;
        }

        /// <summary>
        /// 下载文件完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            ClearClient();

            _isDone = true;

            if (e.Cancelled || e.Error != null)
            {
                _error = e.Error == null ? CANCELLED_ERROR : e.Error.Message;
                if (_isAutoDeleteWrongFile)
                {
                    try
                    {
                        //删除没有下载完成的文件
                        File.Delete(_savePath);
                    }
                    catch
                    {

                    }
                }
            }
        }

        /// <summary>
        /// 下载进度改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _loadedSize = e.BytesReceived;
            _totalSize = e.TotalBytesToReceive;
            if (0 == _totalSize)
            {
                _progress = 0;
            }
            else
            {
                _progress = _loadedSize / (float)_totalSize;
            }

        }
    }
}
