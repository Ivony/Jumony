using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Ivony.Fluent;

namespace Ivony.Web.Html
{
  public abstract class HtmlHandler : IHttpHandler
  {

    /// <summary>
    /// 派生类重写此方法确定实例是否可以被重用
    /// </summary>
    public abstract bool IsReusable
    {
      get;
    }

    /// <summary>
    /// 派生类重写此方法处理文档
    /// </summary>
    protected abstract void ProcessDocument();

    /// <summary>
    /// 派生类重写此方法分析HTML文本为 IHtmlDocument 对象
    /// </summary>
    /// <param name="documentContent">HTML文本</param>
    /// <returns>IHtmlDocument 对象</returns>
    protected abstract IHtmlDocument ParseDocument( string documentContent );


    void IHttpHandler.ProcessRequest( HttpContext context )
    {
      Context = context;

      OriginUrl = Context.Items["HtmlRewriteModule_OriginUrl"] as Uri;

      if ( OriginUrl == null )
      {
        Trace.Warn( "Core", "origin url is not found." );

        var builder = new UriBuilder( Request.Url );
        var path = builder.Path;

        if ( !path.EndsWith( ".ashx" ) )
          throw new InvalidOperationException();

        builder.Path = path.Remove( path.Length - 5 );

        Trace.Warn( "Core", "redirect to template vitual path." );
        Response.Redirect( builder.Uri.AbsoluteUri );
      }

      Trace.Write( "Core", "Begin Parse Document" );
      Document = ParseDocument( LoadTemplateContent() );
      Trace.Write( "Core", "End Parse Document" );

      Trace.Write( "Core", "Begin Process Document" );
      ProcessDocument();
      Trace.Write( "Core", "End Process Document" );
    }



    /// <summary>
    /// 加载并应用绑定样式表
    /// </summary>
    protected void ApplyBindingSheets()
    {
      var bindingSheets = Find( "link[rel=Bindingsheet]" )
        .Select( link => link.Attribute( "href" ).Value() )
        .Where( href => !string.IsNullOrEmpty( href ) )
        .Select( href => LoadBindingSheet( href ) )
        .Where( sheet => sheet != null );


      using ( var bindingContext = HtmlBindingContext.Enter( Document, "ApplyBindingSheet" ) )
      {
        bindingSheets
          .ForAll( sheet => sheet.Apply( bindingContext ) );

        bindingContext.Commit();
      }
    }


    /// <summary>
    /// 从指定文件加载样式表
    /// </summary>
    /// <param name="physicalPath"></param>
    /// <returns></returns>
    private IHtmlBindingSheet LoadBindingSheet( string virtualPath )
    {

      string physicalPath = MapPath( virtualPath );

      if ( !File.Exists( physicalPath ) )
      {
        Trace.Warn( "Core", string.Format( "在 \"{0}\" 找不到样式表文件", physicalPath ) );
        return null;
      }

      return HtmlBindingSheet.Load( physicalPath );
    }


    /// <summary>
    /// 获取正在处理的页面文档
    /// </summary>
    protected IHtmlDocument Document
    {
      get;
      private set;
    }


    /// <summary>
    /// 在文档范围类使用选择器查找符合要求的元素
    /// </summary>
    /// <param name="selector">CSS选择器</param>
    /// <returns>符合选择器要求的元素</returns>
    protected IEnumerable<IHtmlElement> Find( string selector )
    {
      return Document.Find( selector );
    }

    /// <summary>
    /// 在文档范围类使用选择器查找符合要求的元素
    /// </summary>
    /// <param name="selector">多个CSS选择器，结果会合并</param>
    /// <returns>符合选择器要求的元素</returns>
    protected IEnumerable<IHtmlElement> Find( params string[] selectors )
    {
      return Document.Find( selectors );
    }

    /// <summary>
    /// 获取页面模板（即HTML静态页）
    /// </summary>
    /// <returns></returns>
    protected virtual string LoadTemplateContent()
    {
      var physicalPath = GetTemplateFilePath();
      if ( !File.Exists( physicalPath ) )
      {
        var exception = new HttpException( 404, "未找到模板文件，这可能是直接访问.html.ashx文件所造成的。" );
        Trace.Warn( "Core", "template not found!", exception );
        throw exception;
      }

      var cacheKey = string.Format( "HtmlHandler_TemplateContentCache_{0}", physicalPath );
      var templateContent = Cache[cacheKey] as string;

      if ( templateContent == null )
      {
        Trace.Warn( "Core", "template file cache miss." );
        Trace.Write( "Core", "Begin Load Template" );
        using ( var reader = File.OpenText( physicalPath ) )
        {
          templateContent = reader.ReadToEnd();
        }

        Cache.Insert( cacheKey, templateContent, new System.Web.Caching.CacheDependency( physicalPath ) );

        Trace.Write( "Core", "End Load Template" );
      }

      return templateContent;
    }


    /// <summary>
    /// 获取模板文件路径
    /// </summary>
    /// <returns></returns>
    private static string GetTemplateFilePath()
    {
      return Path.ChangeExtension( HttpContext.Current.Request.PhysicalPath, "" );
    }

    /// <summary>
    /// 获取与该页关联的 HttpContext 对象。
    /// </summary>
    protected HttpContext Context
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取原始请求的Url
    /// </summary>
    protected Uri OriginUrl
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取请求的页的 HttpRequest 对象
    /// </summary>
    public HttpRequest Request
    {
      get { return Context.Request; }
    }


    /// <summary>
    /// 获取与该 Page 对象关联的 HttpResponse 对象。该对象使您得以将 HTTP 响应数据发送到客户端，并包含有关该响应的信息
    /// </summary>
    public HttpResponse Response
    {
      get { return Context.Response; }
    }


    /// <summary>
    /// 获取 Server 对象，它是 HttpServerUtility 类的实例
    /// </summary>
    public HttpServerUtility Server
    {
      get { return Context.Server; }
    }


    /// <summary>
    /// 为当前 Web 请求获取 HttpApplicationState 对象
    /// </summary>
    public HttpApplicationState Application
    {
      get { return Context.Application; }
    }


    /// <summary>
    /// 为当前 Web 请求获取 TraceContext 对象
    /// </summary>
    protected TraceContext Trace
    {
      get { return Context.Trace; }
    }


    /// <summary>
    /// 获取与该页驻留的应用程序关联的 Cache 对象
    /// </summary>
    protected System.Web.Caching.Cache Cache
    {
      get { return Context.Cache; }
    }


    /// <summary>
    /// 返回与 Web 服务器上的指定虚拟路径相对应的物理文件路径
    /// </summary>
    /// <param name="virtualPath">Web 服务器的虚拟路径</param>
    /// <returns>与 path 相对应的物理文件路径</returns>
    protected string MapPath( string virtualPath )
    {
      return Request.MapPath( virtualPath );
    }


    #region IDisposable 成员

    public void Dispose()
    {

    }

    #endregion
  }
}
