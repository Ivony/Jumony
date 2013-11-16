using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Web;
using Ivony.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// IRequestMapper 产生的映射结果
  /// </summary>
  public class RequestMapping
  {

    /// <summary>
    /// 创建 RequestMapping 对象
    /// </summary>
    /// <param name="mapper">产生此结果的映射器</param>
    /// <param name="virtualPath">HTML 模版路径</param>
    /// <param name="handler">HTML 文档处理程序</param>
    public RequestMapping( IRequestMapper mapper, string virtualPath, IHtmlHandler handler )
    {

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw WebServiceLocator.VirtualPathFormatError( "virtualPath" );

      VirtualPath = virtualPath;
      Handler = handler;
    }


    /// <summary>
    /// 获取请求的映射器
    /// </summary>
    public IRequestMapper Mapper
    {
      get;
      internal set;
    }

    /// <summary>
    /// 获取 HTML 文档模版路径
    /// </summary>
    public virtual string VirtualPath
    {
      get;
      private set;
    }


    private bool _loaded;
    private string _cacheKey;

    /// <summary>
    /// 获取模版缓存键
    /// </summary>
    public string CacheKey
    {
      get
      {
        if ( _loaded )
          return _cacheKey;

        else
          throw new InvalidOperationException( "模版尚未加载" );

      }
    }

    /// <summary>
    /// 加载 HTML 文档模版
    /// </summary>
    /// <returns>HTML 文档模版</returns>
    public IHtmlDocument LoadDocument()
    {
      var document = LoadDocument( out _cacheKey );

      _loaded = true;
      return document;
    }

    /// <summary>
    /// 加载文档
    /// </summary>
    /// <param name="cacheKey">文档缓存键</param>
    /// <returns>HTML 文档</returns>
    protected virtual IHtmlDocument LoadDocument( out string cacheKey )
    {
      var document = HtmlProviders.LoadDocument( VirtualPath, out cacheKey );
      return document;
    }


    /// <summary>
    /// HTML 文档处理程序
    /// </summary>
    public IHtmlHandler Handler
    {
      get;
      private set;
    }

  }
}
