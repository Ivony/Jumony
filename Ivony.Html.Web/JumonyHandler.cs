using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.IO;
using Ivony.Fluent;
using System.Web.Hosting;

namespace Ivony.Html.Web
{
  public abstract class JumonyHandler : IHttpHandler, IHtmlHandler, IRequiresSessionState
  {

    /// <summary>
    /// 指定此实例是否可以被复用，默认值为false
    /// </summary>
    public virtual bool IsReusable
    {
      get { return false; }
    }


    /// <summary>
    /// 获取映射的结果
    /// </summary>
    protected RequestMapResult MapperResult
    {
      get;
      private set;
    }


    /// <summary>
    /// 实现 IHttpHandler.ProcessRequest
    /// </summary>
    /// <param name="context">当前 HTTP 请求的上下文</param>
    void IHttpHandler.ProcessRequest( HttpContext context )
    {
      Context = new HttpContextWrapper( context );

      MapperResult = Context.GetMapperResult();

      if ( MapperResult == null )
      {
        Trace.Warn( "Jumony Web", "origin url is not found." );

        var builder = new UriBuilder( Request.Url );
        var path = builder.Path;

        if ( !path.EndsWith( ".ashx" ) )
          throw new InvalidOperationException();

        builder.Path = path.Remove( path.Length - 5 );

        Trace.Warn( "Jumony Web", "redirect to template vitualpath." );
        Response.Redirect( builder.Uri.AbsoluteUri );
      }


      Trace.Write( "Jumony Web", "Begin resolve cache." );
      OnPreResolveCache();

      if ( ResolveCache() )
      {
        Trace.Write( "Jumony Web", "Cache resolved." );

        Context.ApplicationInstance.CompleteRequest();
        return;
      }

      OnPostResolveCache();
      Trace.Write( "Jumony Web", "Cache is not resolved." );

      {
        OnPreLoadDocument();

        Trace.Write( "Jumony Web", "Begin load page." );
        Page = LoadPage();
        Trace.Write( "Jumony Web", "End load page." );

        OnPostLoadDocument();
      }

      ((IHtmlHandler) this).ProcessDocument( new HttpContextWrapper( context ), Page.Document );


      {
        Trace.Write( "Jumony Web", "Begin response." );
        var response = CreateResponse();

        UpdateCache( response );

        response.Apply( Response );
        Trace.Write( "Jumony Web", "End response." );
      }
    }


    /// <summary>
    /// 尝试从缓存输出
    /// </summary>
    /// <returns>缓存是否命中</returns>
    protected virtual bool ResolveCache()
    {

      var key = HtmlProviders.GetCacheKey( Context );

      if ( key == null )
        return false;

      var cacheItem = Cache.Get( key ) as RawResponse;
      if ( cacheItem == null )
        return false;

      cacheItem.Apply( Response );

      return true;
    }



    /// <summary>
    /// 刷新输出缓存
    /// </summary>
    /// <param name="cachedResponse">响应的缓存</param>
    protected virtual void UpdateCache( RawResponse cachedResponse )
    {

      var context = new HttpContextWrapper( HttpContext.Current );

      var key = HtmlProviders.GetCacheKey( context );

      if ( key == null )
        return;


      var policy = HtmlProviders.GetCachePolicy( context, this, cachedResponse );


      Cache.Insert( key, cachedResponse, policy.Dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, policy.Duration );

    }


    /// <summary>
    /// 创建响应
    /// </summary>
    /// <returns>响应</returns>
    protected virtual RawResponse CreateResponse()
    {

      var response = new RawResponse();

      Trace.Write( "Jumony Web", "Begin render page." );
      response.Content = Page.Render();
      Trace.Write( "Jumony Web", "End render page." );

      return response;
    }


    void IHtmlHandler.ProcessDocument( HttpContextBase context, IHtmlDocument document )
    {

      OnPreProcessDocument();

      Trace.Write( "Jumony Web", "Begin Process Document." );
      ProcessDocument();
      Trace.Write( "Jumony Web", "End Process Document." );

      OnPostProcessDocument();

      AddGeneratorMetaData();//为处理后的文档加上Jumony生成器的meta信息。

    }


    /// <summary>
    /// 派生类重写此方法处理文档
    /// </summary>
    protected abstract void ProcessDocument();







    private void AddGeneratorMetaData()
    {
      var factory = Document.GetNodeFactory();
      if ( factory != null )
      {
        var meta = factory.CreateElement( "meta" );
        meta.SetAttribute( "name" ).Value( "generator" );
        meta.SetAttribute( "content" ).Value( "Jumony" );

        var header = Document.Find( "html head" ).FirstOrDefault();

        if ( header != null )
          meta.InsertTo( header, 0 );

      }
    }



    /// <summary>
    /// 获取正在处理的页面文档
    /// </summary>
    public IHtmlDocument Document
    {
      get
      {
        if ( Page == null )
          return null;

        return Page.Document;
      }
    }

    /// <summary>
    /// 获取正在处理的Web页面
    /// </summary>
    public WebPage Page
    {
      get;
      private set;
    }



    /// <summary>
    /// 在文档范围内使用选择器查找符合要求的元素
    /// </summary>
    /// <param name="selector">CSS选择器表达式</param>
    /// <returns>符合选择器要求的元素</returns>
    protected IEnumerable<IHtmlElement> Find( string selector )
    {
      return Document.Find( selector );
    }



    /// <summary>
    /// 加载Web页面
    /// </summary>
    /// <returns></returns>
    protected virtual WebPage LoadPage()
    {
      var page = MapperResult.LoadPage();


      return page;
    }


    /// <summary>
    /// 获取与该页关联的 HttpContext 对象。
    /// </summary>
    public HttpContextBase Context
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取请求的页的 HttpRequest 对象
    /// </summary>
    protected HttpRequestBase Request
    {
      get { return Context.Request; }
    }


    /// <summary>
    /// 获取与该 Page 对象关联的 HttpResponse 对象。该对象使您得以将 HTTP 响应数据发送到客户端，并包含有关该响应的信息
    /// </summary>
    protected HttpResponseBase Response
    {
      get { return Context.Response; }
    }


    /// <summary>
    /// 获取 Server 对象，它是 HttpServerUtility 类的实例
    /// </summary>
    protected HttpServerUtilityBase Server
    {
      get { return Context.Server; }
    }


    /// <summary>
    /// 为当前 Web 请求获取 HttpApplicationState 对象
    /// </summary>
    protected HttpApplicationStateBase Application
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
    public string MapPath( string virtualPath )
    {
      return Server.MapPath( virtualPath );
    }



    public event EventHandler PreLoadDocument;
    public event EventHandler PostLoadDocument;

    protected virtual void OnPreLoadDocument() { if ( PreLoadDocument != null ) PreLoadDocument( this, EventArgs.Empty ); }
    protected virtual void OnPostLoadDocument() { if ( PostLoadDocument != null ) PostLoadDocument( this, EventArgs.Empty ); }


    public event EventHandler PreProcessDocument;
    public event EventHandler PostProcessDocument;

    protected virtual void OnPreProcessDocument() { if ( PreProcessDocument != null ) PreProcessDocument( this, EventArgs.Empty ); }
    protected virtual void OnPostProcessDocument() { if ( PostProcessDocument != null ) PostProcessDocument( this, EventArgs.Empty ); }


    public event EventHandler PreRender;
    public event EventHandler PostRender;

    protected virtual void OnPreRender() { if ( PreRender != null ) PreRender( this, EventArgs.Empty ); }
    protected virtual void OnPostRender() { if ( PostRender != null ) PostRender( this, EventArgs.Empty ); }



    public event EventHandler PreResolveCache;
    public event EventHandler PostResolveCache;

    /// <summary>引发PreResolveCache事件</summary>
    protected virtual void OnPreResolveCache() { if ( PreResolveCache != null ) PreResolveCache( this, EventArgs.Empty ); }

    /// <summary>引发PostResolveCache事件</summary>
    protected virtual void OnPostResolveCache() { if ( PostResolveCache != null ) PostResolveCache( this, EventArgs.Empty ); }


    #region IDisposable 成员

    public virtual void Dispose()
    {

    }

    #endregion

  }
}
