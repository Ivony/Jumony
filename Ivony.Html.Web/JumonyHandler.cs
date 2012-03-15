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

  /// <summary>
  /// Jumony 用于处理 HTTP 请求的处理器
  /// </summary>
  public abstract class JumonyHandler : IHttpHandler, IHtmlHandler, IRequiresSessionState
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
        return _mapping;
      }
    }


    /// <summary>
    /// 实现 IHttpHandler.ProcessRequest
    /// </summary>
    /// <param name="context">当前 HTTP 请求的上下文</param>
    void IHttpHandler.ProcessRequest( HttpContext context )
    {

      ProcessRequest( CreateContext( context ) );

    }

    /// <summary>
    /// 处理 HTTP 请求
    /// </summary>
    /// <param name="context">HTTP 上下文信息</param>
    protected virtual void ProcessRequest( HttpContextBase context )
    {
      Context = context;

      _mapping = Context.GetMapping();

      if ( RequestMapping == null )
        throw new HttpException( 404, "不能直接访问 Jumony 页处理程序。" );


      var response = ProcessRequestCore( context );

      OutputResponse( response );

      Trace.Write( "Jumony Web", "End response." );

    }


    protected virtual ICachedResponse ProcessRequestCore( HttpContextBase context )
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


      {
        OnPreLoadDocument();

        Trace.Write( "Jumony Web", "Begin load page." );
        Document = LoadDocument();
        Trace.Write( "Jumony Web", "End load page." );

        OnPostLoadDocument();
      }


      ( (IHtmlHandler) this ).ProcessDocument( Context, Document );


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
    /// 创建本次请求的上下文，派生类重写此方法提供自定义上下文。
    /// </summary>
    /// <param name="context">HTTP 上下文</param>
    /// <returns>请求上下文信息</returns>
    protected virtual HttpContextBase CreateContext( HttpContext context )
    {
      return new HttpContextWrapper( context );
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

      var policy = HtmlProviders.GetCachePolicy( Context );

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
      responseData.Apply( Response );
    }



    /// <summary>
    /// 实现IHtmlHandler接口
    /// </summary>
    /// <param name="context"></param>
    /// <param name="document"></param>
    void IHtmlHandler.ProcessDocument( HttpContextBase context, IHtmlDocument document )
    {

      Context = context;//如果这里是入口，即被当作IHtmlHandler调用时，需要设置Context供派生类使用
      Document = document;

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
    protected virtual IHtmlDocument LoadDocument()
    {
      var document = RequestMapping.LoadTemplate();

      return document;
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



    /// <summary>在加载文档前引发此事件</summary>
    public event EventHandler PreLoadDocument;
    /// <summary>在加载文档后引发此事件</summary>
    public event EventHandler PostLoadDocument;

    protected virtual void OnPreLoadDocument() { if ( PreLoadDocument != null ) PreLoadDocument( this, EventArgs.Empty ); }
    protected virtual void OnPostLoadDocument() { if ( PostLoadDocument != null ) PostLoadDocument( this, EventArgs.Empty ); }


    /// <summary>在处理文档前引发此事件</summary>
    public event EventHandler PreProcessDocument;
    /// <summary>在处理文档后引发此事件</summary>
    public event EventHandler PostProcessDocument;

    protected virtual void OnPreProcessDocument() { if ( PreProcessDocument != null ) PreProcessDocument( this, EventArgs.Empty ); }
    protected virtual void OnPostProcessDocument() { if ( PostProcessDocument != null ) PostProcessDocument( this, EventArgs.Empty ); }


    /// <summary>在渲染文档前引发此事件</summary>
    public event EventHandler PreRender;
    /// <summary>在渲染文档后引发此事件</summary>
    public event EventHandler PostRender;

    protected virtual void OnPreRender() { if ( PreRender != null ) PreRender( this, EventArgs.Empty ); }
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

    public virtual void Dispose()
    {

    }

    #endregion

  }
}
