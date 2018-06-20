## 引言
> 通常我们在Unity中下载一个网络上的资源到本地时，简单的方式是使用WWW类进行下载，然后将Byte[]写到本地。但是这种方式有个隐藏问题就是，网络上的资源会被预先全部加载到内存中，才能被写到本地。于是我查了下资料，然后用WebClient类简单封装了一个使用方法类似WWW的下载类。在下载资源时可以有效地避免内存的增长，尤其是下载很大的资源包时特别有用。

## 方案优势
* 在下载资源时可以有效地避免内存的增长，尤其是下载Size很大（上百兆）的资源包时特别有用。


## 文章DEMO对应的IDE版本
* Unity 5.6.5 测试通过，理论上都兼容

## 总结
- 建议将网络加载的资源分为两类，不需要存到本地的资源直接用WWW加载到内存中使用，需要存到本地的资源使用封装的Downloader类进行下载。
- 如果有特殊需要，可以修改Downloader类实现断点续传功能。

## DEMO地址
* 「国外git」GitHub：[https://github.com/jinglikeblue/UnityDownloadPlus.git](https://github.com/jinglikeblue/UnityDownloadPlus.git)
* 「国内git」Coding：[https://git.coding.net/jinglikeblue/WebDownloadDemo.git](https://git.coding.net/jinglikeblue/WebDownloadDemo.git)

