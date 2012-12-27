using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using System.Web.Caching;
using System.IO;
using System.Web.Mvc.Html;
using Ivony.Fluent;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Html.Web.Mvc
{


  /// <summary>
  /// 所有 HTML 视图处理程序的基类，实现 IView 接口，并提供内置的 HTML 扩展功能以及视图结果的缓存。
  /// </summary>
  public abstract class ViewBase : IView, ICacheableResult
  {


    internal const string ViewFiltersDataKey = "Jumony_ViewBase_ViewFilters";


    /// <summary>
    /// 派生类调用创建 ViewBase 的具体实例。
    /// </summary>
    protected ViewBase()
    {
      RenderAdapters = new List<IHtmlAdapter>() { new PartialRenderAdapter( this ) };
    }



    /// <summary>
    /// 获取视图上下文
    /// </summary>
    public ViewContext ViewContext
    {
      get;
      private set;
    }

    /// <summary>
    /// 获取视图模型
    /// </summary>
    protected object ViewModel
    {
      get { return ViewContext.ViewData.Model; }
    }

    /// <summary>
    /// 获取视图数据
    /// </summary>
    protected ViewDataDictionary ViewData
    {
      get { return ViewContext.ViewData; }
    }

    /// <summary>
    /// 获取当前 HTTP 上下文
    /// </summary>
    protected HttpContextBase HttpContext
    {
      get { return ViewContext.HttpContext; }
    }

    /// <summary>
    /// 获取当前 HTTP 响应的追踪上下文对象
    /// </summary>
    protected TraceContext Trace
    {
      get { return HttpContext.Trace; }
    }



    /// <summary>
    /// 获取请求上下文
    /// </summary>
    protected RequestContext RequestContext
    {
      get { return ViewContext.RequestContext; }
    }

    /// <summary>
    /// 获取路由信息
    /// </summary>
    protected RouteData RouteData
    {
      get { return ViewContext.RouteData; }
    }

    /// <summary>
    /// 获取 TempData
    /// </summary>
    protected TempDataDictionary TempData
    {
      get { return ViewContext.TempData; }
    }

    /// <summary>
    /// 获取缓存提供对象
    /// </summary>
    protected Cache Cache
    {
      get { return HttpContext.Cache; }
    }

    /// <summary>
    /// 获取 Url 帮助器
    /// </summary>
    protected JumonyUrlHelper Url
    {
      get;
      private set;
    }

    /// <summary>
    /// 获取原始的（顶层的）视图上下文
    /// </summary>
    protected ViewContext RawViewContext
    {
      get;
      private set;
    }






    private bool _initialized = false;


    /// <summary>
    /// 初始化视图
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <param name="partialMode">是否为部分视图模式</param>
    protected void Initialize( string virtualPath, bool partialMode )
    {
      if ( _initialized )
        throw new InvalidOperationException( "视图已经初始化" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw new FormatException( "VirtualPath 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取" );

      VirtualPath = virtualPath;
      PartialMode = partialMode;

      _initialized = true;
    }

    /// <summary>
    /// 视图的虚拟路径
    /// </summary>
    public string VirtualPath
    {
      get;
      private set;
    }

    /// <summary>
    /// 是否为部分视图
    /// </summary>
    public bool PartialMode
    {
      get;
      private set;
    }



    /// <summary>
    /// 获取渲染和处理的范畴
    /// </summary>
    public IHtmlContainer Scope
    {
      get;
      private set;
    }

    /// <summary>
    /// 初始化视图
    /// </summary>
    /// <returns>渲染和处理的范畴，一般情况下是 IHtmlDocument</returns>
    protected virtual IHtmlContainer InitializeScope( string virtualPath, bool partialMode )
    {
      var document = LoadDocument( virtualPath );

      if ( partialMode )
        return FindPartialScope( document );

      else
        return document;

    }

    /// <summary>
    /// 查找部分视图渲染范畴
    /// </summary>
    /// <param name="document">加载的文档</param>
    /// <returns>渲染范畴</returns>
    protected virtual IHtmlContainer FindPartialScope( IHtmlDocument document )
    {
      var body = document.Find( "body" ).SingleOrDefault();

      if ( body == null )
        return document;

      else
        return body;
    }


    void IView.Render( ViewContext viewContext, TextWriter writer )
    {
      ViewContext = viewContext;

      while ( viewContext.IsChildAction )
      {
        viewContext = viewContext.ParentActionViewContext;//循环上溯最原始的视图上下文
      }
      RawViewContext = viewContext;

      RenderCore( writer );
    }



    /// <summary>
    /// 处理和渲染视图
    /// </summary>
    /// <param name="viewContext">视图上下文</param>
    /// <param name="writer">文本编写器</param>
    protected virtual void RenderCore( TextWriter writer )
    {
      //获取视图筛选器
      Filters = ViewData[ViewFiltersDataKey] as IEnumerable<IViewFilter> ?? Enumerable.Empty<IViewFilter>();
      ViewData.Remove( ViewFiltersDataKey );


      RenderAdapters.Add( new ViewElementAdapter( ViewContext ) );


      if ( !_initialized )
        throw new InvalidOperationException( "视图尚未初始化" );

      Url = new JumonyUrlHelper( this );

      HttpContext.Trace.Write( "Jumony View Engine", "Begin InitializeScope" );
      Scope = InitializeScope( VirtualPath, PartialMode );
      HttpContext.Trace.Write( "Jumony View Engine", "End InitializeScope" );

      var content = RenderCore( Scope );

      CachedResponse = UpdateCache( content );

      writer.Write( content );
    }


    /// <summary>
    /// 处理和渲染指定 HTML 范畴
    /// </summary>
    /// <param name="scope">要处理和渲染的范畴</param>
    /// <returns></returns>
    protected string RenderCore( IHtmlContainer scope )
    {
      HttpContext.Trace.Write( "Jumony View Engine", "Begin Process" );
      OnPreProcess();
      Process( Scope );
      OnPostProcess();
      HttpContext.Trace.Write( "Jumony View Engine", "End Process" );


      HttpContext.Trace.Write( "Jumony View Engine", "Begin ProcessActionRoutes" );
      Url.ProcessActionUrls( Scope );
      HttpContext.Trace.Write( "Jumony View Engine", "End ProcessActionRoutes" );


      HttpContext.Trace.Write( "Jumony View Engine", "Begin ResolveUri" );
      Url.ResolveUri( Scope, VirtualPath );
      HttpContext.Trace.Write( "Jumony View Engine", "End ResolveUri" );

      AddGeneratorMetaData();

      HttpContext.Trace.Write( "Jumony View Engine", "Begin Render" );
      OnPreRender();
      var content = RenderContent( Scope, PartialMode );
      OnPostRender();
      HttpContext.Trace.Write( "Jumony View Engine", "End Render" );

      return content;
    }



    private void AddGeneratorMetaData()
    {

      if ( MvcEnvironment.Configuration.DisableGeneratorTag || PartialMode )
        return;

      var document = Scope as IHtmlDocument;
      if ( document == null )
        return;

      var modifier = document.DomModifier;
      if ( modifier == null )
        return;


      var header = document.Find( "head" ).FirstOrDefault();

      if ( header == null )
        return;


      var metaElement = modifier.AddElement( header, "meta" );

      metaElement.SetAttribute( "name", "generator" );
      metaElement.SetAttribute( "content", "Jumony" );
    }


    /// <summary>
    /// 获取所有视图筛选器
    /// </summary>
    protected IEnumerable<IViewFilter> Filters
    {
      get;
      private set;
    }


    #region Events

    /// <summary>
    /// 初识化结束后，进行任何处理前引发此事件
    /// </summary>
    public event EventHandler PreProcess;

    /// <summary>
    /// 引发 PreProcess 事件
    /// </summary>
    protected virtual void OnPreProcess()
    {

      foreach ( var filter in Filters )
      {
        try
        {
          filter.OnPreRender( ViewContext, this );
        }
        catch { }
      }

      if ( PreProcess != null )
        PreProcess( this, EventArgs.Empty );
    }


    /// <summary>
    /// 对文档的所有处理完成后引发此事件
    /// </summary>
    public event EventHandler PostProcess;

    /// <summary>
    /// 引发 PostProcess 事件
    /// </summary>
    protected virtual void OnPostProcess()
    {
      foreach ( var filter in Filters.Reverse() )
      {
        try
        {
          filter.OnPostProcess( ViewContext, this );
        }
        catch { }
      }

      if ( PostProcess != null )
        PostProcess( this, EventArgs.Empty );
    }


    /// <summary>
    /// 完成所有渲染准备工作后，渲染文档之前引发此事件。
    /// </summary>
    public event EventHandler PreRender;

    /// <summary>
    /// 引发 PreRender 事件
    /// </summary>
    /// <param name="writer">用于输出渲染结果的编写器</param>
    protected virtual void OnPreRender()
    {
      foreach ( var filter in Filters )
      {
        try
        {
          filter.OnPreRender( ViewContext, this );
        }
        catch { }
      }

      if ( PreRender != null )
        PreRender( this, EventArgs.Empty );
    }


    /// <summary>
    /// 文档渲染完毕后引发此事件
    /// </summary>
    public event EventHandler PostRender;

    /// <summary>
    /// 引发 PostRender 事件
    /// </summary>
    /// <param name="writer">用于输出渲染结果的编写器</param>
    protected virtual void OnPostRender()
    {
      foreach ( var filter in Filters.Reverse() )
      {
        try
        {
          filter.OnPostRender( ViewContext, this );
        }
        catch { }
      }

      if ( PostRender != null )
        PostRender( this, EventArgs.Empty );
    }

    #endregion


    /// <summary>
    /// 更新缓存
    /// </summary>
    /// <param name="content">渲染结果</param>
    protected virtual ICachedResponse UpdateCache( string content )
    {
      var response = new RawResponse();

      response.Content = content;
      response.Headers.Add( "ContentType", "text/html" );

      return response;
    }

    /// <summary>
    /// 缓存结果
    /// </summary>
    protected ICachedResponse CachedResponse
    {
      get;
      private set;
    }


    ICachedResponse ICacheableResult.GetCachedResponse()
    {
      return CachedResponse;
    }



    /// <summary>
    /// 派生类实现此方法完成对视图的处理工作
    /// </summary>
    protected abstract void Process( IHtmlContainer container );



    /// <summary>
    /// 渲染 HTML 内容。
    /// </summary>
    /// <returns></returns>
    protected virtual string RenderContent( IHtmlContainer scope, bool partialMode )
    {


      var document = scope as IHtmlDocument;
      if ( document == null )
      {
        var writer = new StringWriter();

        foreach ( var node in scope.Nodes() )
          node.Render( writer, RenderAdapters.ToArray() );

        return writer.ToString();
      }

      else
        return document.Render( RenderAdapters.ToArray() );

    }



    /// <summary>
    /// 派生类调用此方法加载虚拟路径处的文档
    /// </summary>
    /// <returns></returns>
    protected virtual IHtmlDocument LoadDocument( string virtualPath )
    {
      return MvcEnvironment.LoadDocument( virtualPath );
    }


    /// <summary>
    /// 自定义渲染过程的 HTML 转换器
    /// </summary>
    protected virtual IList<IHtmlAdapter> RenderAdapters
    {
      get;
      private set;
    }








    /// <summary>
    /// 渲染部分视图（重写此方法接管 partial 处理逻辑）。
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <param name="writer"></param>
    protected virtual void RenderPartial( IHtmlElement partialElement, TextWriter writer )
    {


      var timeout = MvcEnvironment.Configuration.PartialRenderTimeout;

      if ( timeout > TimeSpan.Zero )
      {

        string result = null;
        Exception exception = null;

        var thread = new Thread( () => result = RenderPartialAsync( partialElement, out exception ) );

        thread.Start();
        if ( thread.Join( timeout ) )
        {
          if ( exception != null )
            throw new HttpException( "渲染 Partial 时发生错误，详见内部异常", exception );

          writer.Write( result );
        }

        else
        {
          thread.Abort();
          writer.Write( "<!--Render partial timeout-->" );
        }
      }

      else
        writer.Write( RenderPartial( partialElement ) );
    }


    /// <summary>
    /// 异步渲染部分视图
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <param name="exception">渲染过程中产生的异常</param>
    /// <returns></returns>
    [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]//捕获任何异常
    protected string RenderPartialAsync( IHtmlElement partialElement, out Exception exception )
    {
      try
      {
        exception = null;
        return RenderPartial( partialElement );
      }
      catch ( ThreadAbortException )
      {
        exception = null;
        return null;
      }
      catch ( Exception e )
      {
        exception = e;
        return null;
      }
    }



    /// <summary>
    /// 渲染部分视图（重写此方法以实现自定义输出 partial 元素）
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <returns></returns>
    protected virtual string RenderPartial( IHtmlElement partialElement )
    {
      var action = partialElement.Attribute( "action" ).Value();
      var view = partialElement.Attribute( "view" ).Value();
      var path = partialElement.Attribute( "path" ).Value();
      var handler = partialElement.Attribute( "handler" ).Value();


      var helper = MakeHelper();


      try
      {
        if ( action != null )//Action 部分视图
        {
          var controller = partialElement.Attribute( "controller" ).Value() ?? (string) RouteData.Values["controller"];
          var routeValues = Url.GetRouteValues( partialElement );

          return helper.Action( actionName: action, controllerName: controller, routeValues: routeValues ).ToString();
        }

        else if ( view != null )
        {
          return RenderPartialView( helper, view, partialElement );
        }

        else if ( path != null )
        {
          if ( !VirtualPathUtility.IsAppRelative( path ) )
            throw new FormatException( "path 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取" );

          var content = HtmlProviders.LoadContent( path );
          if ( content != null )
            return content.Content;
        }

      }
      catch ( ThreadAbortException )
      {

      }
      catch //若渲染时发生错误
      {
        if ( MvcEnvironment.Configuration.IgnorePartialRenderException || partialElement.Attribute( "ignoreError" ) != null )
          return "<!--parital view render failed-->";
        else
          throw;
      }

      throw new NotSupportedException( "无法处理的partial标签：" + ContentExtensions.GenerateTagHtml( partialElement, false ) );

    }

    /// <summary>
    /// 渲染部分视图（重写此方法为指定名称的部分视图提供模型和数据）
    /// </summary>
    /// <param name="helper">HTML Helper</param>
    /// <param name="partialViewName">部分视图名称</param>
    /// <param name="partialElement">partial 元素</param>
    /// <returns>渲染好的部分视图</returns>
    protected virtual string RenderPartialView( HtmlHelper helper, string partialViewName, IHtmlElement partialElement )
    {
      return helper.Partial( partialViewName ).ToString();
    }





    /// <summary>
    /// 创建 HtmlHelper 对象
    /// </summary>
    /// <returns></returns>
    protected HtmlHelper MakeHelper()
    {

      var helper = new HtmlHelper( ViewContext, new ViewDataContainer( this ) );
      return helper;
    }


    /// <summary>
    /// 视图引擎调用此方法清理视图所使用的所有非托管资源
    /// </summary>
    public virtual void Dispose()
    {
    }


    /// <summary>
    /// 为视图提供 ViewData 的容器类型
    /// </summary>
    protected class ViewDataContainer : IViewDataContainer
    {

      private ViewBase _view;

      /// <summary>
      /// 创建 ViewDataContainer 对象
      /// </summary>
      /// <param name="view"></param>
      public ViewDataContainer( ViewBase view )
      {
        _view = view;
      }


      ViewDataDictionary IViewDataContainer.ViewData
      {
        get
        {
          return _view.ViewData;
        }
        set
        {
          throw new NotSupportedException();
        }
      }
    }


    /// <summary>
    /// 用于渲染部分视图的 HTML 渲染代理
    /// </summary>
    public sealed class PartialRenderAdapter : HtmlElementAdapter
    {

      private ViewBase _view;

      /// <summary>
      /// 创建 PartialRenderAdapter 实例
      /// </summary>
      /// <param name="view"></param>
      public PartialRenderAdapter( ViewBase view )
      {
        _view = view;
      }


      /// <summary>
      /// 一个 CSS 选择器，用于选取 partial 标签
      /// </summary>
      protected override string CssSelector
      {
        get { return "partial"; }
      }


      /// <summary>
      /// 渲染 partial 标签
      /// </summary>
      /// <param name="element">partial 标签</param>
      /// <param name="writer">用于渲染的文本编写器</param>
      public override void Render( IHtmlElement element, TextWriter writer )
      {

        var partialTag = ContentExtensions.GenerateTagHtml( element, true );

        _view.HttpContext.Trace.Write( "Jumony View Engine", string.Format( "Begin Render Partial: {0}", partialTag ) );
        _view.RenderPartial( element, writer );
        _view.HttpContext.Trace.Write( "Jumony View Engine", string.Format( "End Render Partial: {0}", partialTag ) );
      }
    }


  }
}
