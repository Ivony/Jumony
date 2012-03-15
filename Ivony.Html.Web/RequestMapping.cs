using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Web;

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
    /// 
    /// <param name="templatePath">HTML 模版路径</param>
    /// <param name="handler">HTML 文档处理程序</param>
    public RequestMapping( IRequestMapper mapper, string templatePath, IHtmlHandler handler )
    {

      if ( !VirtualPathUtility.IsAppRelative( templatePath ) )
        throw new ArgumentException( "模版文件路径只能使用应用程序根相对路径，即以~/开头的路径，调用VirtualPathUtility.ToAppRelative方法或使用HttpRequest.AppRelativeCurrentExecutionFilePath属性获取", "templatePath" );

      TemplatePath = templatePath;
      Handler = handler;
    }


    /// <summary>
    /// 派生类调用创建 RequestMapping 对象
    /// </summary>
    /// <param name="mapper">请求映射器</param>
    /// <param name="handler">HTML 文档处理程序</param>
    protected RequestMapping( IRequestMapper mapper, IHtmlHandler handler )
    {
      Mapper = mapper;
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
    protected virtual string TemplatePath
    {
      get;
      set;
    }


    private bool _templateLoaded;
    private string _templateCacheKey;

    /// <summary>
    /// 获取模版缓存键
    /// </summary>
    public virtual string TemplateCacheKey
    {
      get
      {
        if ( _templateLoaded )
          return _templateCacheKey;

        else
          throw new InvalidOperationException( "模版尚未加载" );

      }
    }

    /// <summary>
    /// 加载 HTML 文档模版
    /// </summary>
    /// <returns>HTML 文档模版</returns>
    public virtual IHtmlDocument LoadTemplate()
    {
      var document = LoadDocument( out _templateCacheKey );

      _templateLoaded = true;
      return document;
    }

    /// <summary>
    /// 加载文档
    /// </summary>
    /// <param name="cacheKey">文档缓存键</param>
    /// <returns>HTML 文档</returns>
    protected virtual IHtmlDocument LoadDocument( out string cacheKey )
    {
      var document = HtmlProviders.LoadDocument( new HttpContextWrapper( HttpContext.Current ), TemplatePath, out cacheKey );
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
