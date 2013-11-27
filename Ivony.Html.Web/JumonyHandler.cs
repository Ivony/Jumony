using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.IO;
using Ivony.Fluent;
using System.Web.Hosting;

using Ivony.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{

  /// <summary>
  /// Jumony 用于处理 HTTP 请求的处理器
  /// </summary>
  public abstract class JumonyHandler : IHttpHandler, IRequiresSessionState
  {

    /// <summary>
    /// 指定此实例是否可以被复用，默认值为false
    /// </summary>
    public virtual bool IsReusable
    {
      get { return false; }
    }




    private RequestMapping _mapping;

    /// <summary>
    /// 获取映射的结果
    /// </summary>
    protected virtual RequestMapping RequestMapping
    {
      get
      {
        if ( _mapping == null )
          _mapping = HttpContext.GetMapping();

        return _mapping;
      }
    }


    /// <summary>
    /// 实现 IHttpHandler.ProcessRequest
    /// </summary>
    /// <param name="context">当前 HTTP 请求的上下文</param>
    void IHttpHandler.ProcessRequest( HttpContext context )
    {

      ProcessRequest( context.Request.RequestContext );

    }

    /// <summary>
    /// 处理 HTTP 请求
    /// </summary>
    /// <param name="context">HTTP 上下文信息</param>
    protected void ProcessRequest( RequestContext context )
    {
      _httpContext = context.HttpContext;


      if ( context.RouteData == null || !(context.RouteData.Route is IHtmlMapRoute) )
        DirectVisitError();

      var virtualPath = context.RouteData.DataTokens[JumonyRequestMapperRoute.VirtualPathToken] as string;
      virtualPath = virtualPath ?? context.HttpContext.Request.AppRelativeCurrentExecutionFilePath;

      var document = HtmlProviders.LoadDocument( virtualPath );


      ProcessRequest( context, virtualPath );


      Trace.Write( "Jumony Web", "End response." );




    }

    protected virtual void ProcessRequest( RequestContext context, string virtualPath )
    {


      var response = ProcessRequestCore( context.HttpContext, virtualPath );

      OutputResponse( response );
    }





    private void DirectVisitError()
    {
      throw new HttpException( 404, "不能直接访问 Jumony 页处理程序。" );
    }



    protected virtual TraceContext Trace
    {
      get { return HttpContext.Trace; }
    }


    /// <summary>
    /// 派生类重写此方法接管 HTTP 请求处理流程
    /// </summary>
    /// <param name="context">HTTP 请求上下文</param>
    /// <returns>处理后的结果</returns>
    protected virtual ICachedResponse ProcessRequestCore( HttpContextBase context, string virtualPath )
    {

      ICachedResponse response;


      {
        Trace.Write( "Jumony Web", "Begin resolve cache." );
        OnPreResolveCache();


        response = ResolveCache();

        if ( response != null )
        {
          Trace.Write( "Jumony Web", "Cache resolved." );
          return response;
        }

        OnPostResolveCache();
        Trace.Write( "Jumony Web", "Cache is not resolved." );
      }



      IHtmlDocument document;


      {
        OnPreLoadDocument();

        Trace.Write( "Jumony Web", "Begin load page." );
        document = HtmlProviders.LoadDocument( virtualPath );
        Trace.Write( "Jumony Web", "End load page." );

        OnPostLoadDocument();
      }



      {

        var handler = HtmlProviders.GetHandler( virtualPath );


        OnPreProcessDocument();

        Trace.Write( "Jumony Web", "Begin Process Document." );
        handler.ProcessDocument( context, document );
        Trace.Write( "Jumony Web", "End Process Document." );

        OnPostProcessDocument();

      }


      {
        Trace.Write( "Jumony Web", "Begin create response." );

        OnPreRender();

        Trace.Write( "Jumony Web", "Begin render page." );
        var content = Document.Render();
        Trace.Write( "Jumony Web", "End render page." );

        OnPostRender();


        response = CreateResponse( content );

        Trace.Write( "Jumony Web", "End create response." );
      }

      {
        Trace.Write( "Jumony Web", "Begin update cache." );

        UpdateCache( response );

        Trace.Write( "Jumony Web", "End update cache." );
      }


      return response;
    }




    /// <summary>
    /// 获取当前请求的缓存策略
    /// </summary>
    protected CachePolicy CachePolicy
    {
      get;
      private set;
    }

    /// <summary>
    /// 尝试获取缓存的输出
    /// </summary>
    /// <returns>缓存的输出</returns>
    protected virtual ICachedResponse ResolveCache()
    {

      var policy = HtmlProviders.GetCachePolicy( HttpContext );

      if ( policy == null )
        return null;


      CachePolicy = policy;

      return CachePolicy.ResolveCache();
    }


    /// <summary>
    /// 刷新输出缓存
    /// </summary>
    /// <param name="cachedResponse">响应的缓存</param>
    protected virtual void UpdateCache( ICachedResponse cachedResponse )
    {

      if ( CachePolicy == null )
        return;


      CachePolicy.UpdateCache( cachedResponse );

    }


    /// <summary>
    /// 派生类重写此方法自定义创建响应的逻辑
    /// </summary>
    /// <returns>响应</returns>
    protected virtual RawResponse CreateResponse( string content )
    {
      return new RawResponse() { Content = content };
    }



    /// <summary>
    /// 派生类重写此方法自定义输出响应的逻辑
    /// </summary>
    /// <param name="responseData">响应信息</param>
    protected virtual void OutputResponse( ICachedResponse responseData )
    {
      responseData.Apply( HttpContext.Response );
    }


    /// <summary>
    /// 这个方法是用来添加<![CDATA[<meta name="generator" value="jumony" />]]>元素的。
    /// </summary>
    private void AddGeneratorMetaData()
    {
      var modifier = Document.DomModifier;
      if ( modifier != null )
      {
        var header = Document.Find( "html head" ).FirstOrDefault();

        if ( header != null )
        {

          var metaElement = modifier.AddElement( header, "meta" );

          metaElement.SetAttribute( "name", "generator" );
          metaElement.SetAttribute( "content", "Jumony" );
        }
      }
    }


    /// <summary>
    /// 获取正在处理的页面文档
    /// </summary>
    public IHtmlDocument Document
    {
      get;
      private set;
    }



    private HttpContextBase _httpContext;

    /// <summary>
    /// 获取与该页关联的 HttpContext 对象。
    /// </summary>
    protected override HttpContextBase HttpContext
    {
      get { return _httpContext; }
    }

    /// <summary>
    /// 获取要处理的 HTML 范畴
    /// </summary>
    public sealed override IHtmlContainer Scope
    {
      get { return Document; }
    }

    /// <summary>
    /// 获取当前文档的虚拟路径
    /// </summary>
    public sealed override string VirtualPath
    {
      get { return VirtualPath; }
    }



    /// <summary>在加载文档前引发此事件</summary>
    public event EventHandler PreLoadDocument;
    /// <summary>在加载文档后引发此事件</summary>
    public event EventHandler PostLoadDocument;

    /// <summary>引发 PreLoadDocument 事件</summary>
    protected virtual void OnPreLoadDocument() { if ( PreLoadDocument != null ) PreLoadDocument( this, EventArgs.Empty ); }
    /// <summary>引发 PostLoadDocument 事件</summary>
    protected virtual void OnPostLoadDocument() { if ( PostLoadDocument != null ) PostLoadDocument( this, EventArgs.Empty ); }


    /// <summary>在处理文档前引发此事件</summary>
    public event EventHandler PreProcessDocument;
    /// <summary>在处理文档后引发此事件</summary>
    public event EventHandler PostProcessDocument;

    /// <summary>引发 PreProcessDocument 事件</summary>
    protected virtual void OnPreProcessDocument() { if ( PreProcessDocument != null ) PreProcessDocument( this, EventArgs.Empty ); }
    /// <summary>引发 PostProcessDocument 事件</summary>
    protected virtual void OnPostProcessDocument() { if ( PostProcessDocument != null ) PostProcessDocument( this, EventArgs.Empty ); }


    /// <summary>在渲染文档前引发此事件</summary>
    public event EventHandler PreRender;
    /// <summary>在渲染文档后引发此事件</summary>
    public event EventHandler PostRender;

    /// <summary>引发 PreRender 事件</summary>
    protected virtual void OnPreRender() { if ( PreRender != null ) PreRender( this, EventArgs.Empty ); }
    /// <summary>引发 PostRender 事件</summary>
    protected virtual void OnPostRender() { if ( PostRender != null ) PostRender( this, EventArgs.Empty ); }



    /// <summary>在尝试缓存输出前引发此事件</summary>
    public event EventHandler PreResolveCache;
    /// <summary>在缓存未命中后引发此事件</summary>
    public event EventHandler PostResolveCache;

    /// <summary>引发 PreResolveCache 事件</summary>
    protected virtual void OnPreResolveCache() { if ( PreResolveCache != null ) PreResolveCache( this, EventArgs.Empty ); }
    /// <summary>引发 PostResolveCache 事件</summary>
    protected virtual void OnPostResolveCache() { if ( PostResolveCache != null ) PostResolveCache( this, EventArgs.Empty ); }


    #region IDisposable 成员

    /// <summary>
    /// 执行与释放或重置非托管资源相关的应用程序定义的任务
    /// </summary>
    public virtual void Dispose()
    {

    }

    #endregion


  }
}
