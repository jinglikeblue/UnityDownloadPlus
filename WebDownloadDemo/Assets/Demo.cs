using Jing.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public InputField textUrl;

    public Text textInfo;

    public Text textProgress;

    public Button btnLoad;
    public Button btnCancel;    

    //下载资源的保存目录
    string DOWNLOAD_PATH;

    //下载器
    Downloader _loader;

    void Start()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        DOWNLOAD_PATH = Application.dataPath + "/StreamingAssets/";        
#else
        DOWNLOAD_PATH = Application.persistentDataPath + "/";        
#endif
        
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        btnLoad.gameObject.SetActive(null == _loader ? true : false);
        btnCancel.gameObject.SetActive(!btnLoad.gameObject.activeInHierarchy);
    }
    
    /// <summary>
    /// 开始下载
    /// </summary>
    public void StartDownload()
    {
        string url = textUrl.text.Trim();
        StartCoroutine(Download(url));
    }

    /// <summary>
    /// 取消下载
    /// </summary>
    public void CancelDownload()
    {
        if (null != _loader)
        {
            _loader.Dispose();            
        }
    }

    IEnumerator Download(string url)
    {
        Uri uri = new Uri(url);        
        string filePath = DOWNLOAD_PATH + Path.GetFileName(uri.LocalPath);
        var d = Directory.GetParent(filePath);
        if (false == d.Exists)
        {
            d.Create();
        }
        _loader = new Downloader(url, filePath);
        while (_loader.isDone == false)
        {
            //Debug.LogFormat("下载进度: [{0}/{1}] ({2}%)", _loader.loadedSize, _loader.totalSize, _loader.progress);
            textProgress.text = string.Format("{0}%", (int)(_loader.progress * 100));
            textInfo.text = string.Format("下载进度: [{0} / {1}]", _loader.loadedSize, _loader.totalSize);
            yield return new WaitForEndOfFrame();
        }

        if (_loader.error != null)
        {
            textProgress.text = "Error";
            textInfo.text = _loader.error;
            Debug.LogFormat("下载出错：{0}", _loader.error);            
        }
        else
        {
            textProgress.text = "Done";
            textInfo.text = "SavePath: " + _loader.savePath;
            Debug.Log("下载完成");
        }
        
        _loader.Dispose();        
        _loader = null;

        StartDownload();
    }
}
