using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace Ivony.Web.Html
{
  public abstract class HtmlHandler : IHttpHandler
  {

    public abstract bool IsReusable
    {
      get;
    }

    /// <summary>
    /// 派生类重写此方法处理文档
    /// </summary>
    protected abstract void ProcessDocument();

    /// <summary>
    /// 派生类重写此方法分析字符串为 IHtmlDocument 对象
    /// </summary>
    /// <param name="documentContent"></param>
    /// <returns></returns>
    protected abstract IHtmlDocument LoadDocument( string documentContent );


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
      Document = LoadDocument( GetTemplateContent() );
      Trace.Write( "Core", "End Parse Document" );

      Trace.Write( "Core", "Begin Process Document" );
      ProcessDocument();
      Trace.Write( "Core", "End Process Document" );
    }

    protected IHtmlDocument Document
    {
      get;
      private set;
    }


    protected IEnumerable<IHtmlElement> Find( string selector )
    {
      return Document.Find( selector );
    }

    protected virtual string GetTemplateContent()
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

    private static string GetTemplateFilePath()
    {
      return Path.ChangeExtension( HttpContext.Current.Request.PhysicalPath, "" );
    }

    protected HttpContext Context
    {
      get;
      private set;
    }

    protected Uri OriginUrl
    {
      get;
      private set;
    }


    public HttpRequest Request
    {
      get { return Context.Request; }
    }

    public HttpResponse Response
    {
      get { return Context.Response; }
    }

    public HttpServerUtility Server
    {
      get { return Context.Server; }
    }

    public HttpApplicationState Application
    {
      get { return Context.Application; }
    }

    public HttpApplication ApplicationInstance
    {
      get { return Context.ApplicationInstance; }
    }

    protected TraceContext Trace
    {
      get { return Context.Trace; }
    }

    protected System.Web.Caching.Cache Cache
    {
      get { return Context.Cache; }
    }

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
