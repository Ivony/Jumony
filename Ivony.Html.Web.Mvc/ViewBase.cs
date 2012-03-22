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



    private string _virtualPath;
    /// <summary>
    /// 获取或设置 HTML 视图的虚拟路径，此属性必须在处理视图前进行初始化
    /// </summary>
    public string VirtualPath
    {
      get { return _virtualPath; }
      protected set
      {

        if ( !VirtualPathUtility.IsAppRelative( value ) )
          throw new FormatException( "VirtualPath 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取" );
        _virtualPath = value;
      }
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
    protected UrlHelper Url
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


    /// <summary>
    /// 渲染视图
    /// </summary>
    /// <param name="viewContext">视图上下文</param>
    /// <param name="writer">文本编写器</param>
    public virtual void Render( ViewContext viewContext, TextWriter writer )
    {
      ViewContext = viewContext;

      //获取视图筛选器
      Filters = ViewData[ViewFiltersDataKey] as IEnumerable<IViewFilter> ?? Enumerable.Empty<IViewFilter>();
      ViewData.Remove( ViewFiltersDataKey );


      RenderAdapters.Add( new ViewElementAdapter( viewContext ) );


      while ( viewContext.IsChildAction )
      {
        viewContext = viewContext.ParentActionViewContext;//循环上溯最原始的视图上下文
      }
      RawViewContext = viewContext;


      Url = new UrlHelper( RequestContext );

      HttpContext.Trace.Write( "Jumony for MVC", "Begin Process" );
      OnPreProcess();
      ProcessMain();
      OnPostProcess();
      HttpContext.Trace.Write( "Jumony for MVC", "End Process" );


      HttpContext.Trace.Write( "Jumony for MVC", "Begin Render" );
      OnPreRender( writer );
      var content = RenderContent();
      OnPostRender( writer );
      HttpContext.Trace.Write( "Jumony for MVC", "End Render" );

      UpdateCache( content );

      writer.Write( content );
    }



    protected IEnumerable<IViewFilter> Filters
    {
      get;
      private set;
    }


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
    protected virtual void OnPreRender( TextWriter writer )
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
    protected virtual void OnPostRender( TextWriter writer )
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



    /// <summary>
    /// 更新缓存
    /// </summary>
    /// <param name="content">渲染结果</param>
    protected virtual void UpdateCache( string content )
    {
      var response = new RawResponse();

      response.Content = content;
      response.Headers.Add( "ContentType", "text/html" );

      CachedResponse = response;
    }

    /// <summary>
    /// 缓存结果
    /// </summary>
    protected ICachedResponse CachedResponse
    {
      get;
      set;
    }


    ICachedResponse ICacheableResult.GetCachedResponse()
    {
      return CachedResponse;
    }




    /// <summary>
    /// 派生类实现此方法完成对 HTML 文档的处理工作
    /// </summary>
    protected abstract void ProcessMain();

    /// <summary>
    /// 派生类实现此方法渲染 HTML 内容。
    /// </summary>
    /// <returns></returns>
    protected abstract string RenderContent();



    /// <summary>
    /// 派生类调用此方法加载虚拟路径处的文档
    /// </summary>
    /// <returns></returns>
    protected virtual IHtmlDocument LoadDocument()
    {
      return MvcEnvironment.LoadDocument( HttpContext, VirtualPath );
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
    /// 派生类调用此方法处理 Action 路由
    /// </summary>
    /// <param name="container"></param>
    protected void ProcessActionUrls( IHtmlContainer container )
    {
      var elements = container.Find( "a[action] , img[action] , form[action][controller] , script[action]" );

      foreach ( var actionElement in elements )
      {

        lock ( actionElement.SyncRoot )//锁住元素不被修改
        {

          var action = actionElement.Attribute( "action" ).Value() ?? RouteData.Values["action"].CastTo<string>();
          var controller = actionElement.Attribute( "controller" ).Value() ?? RouteData.Values["controller"].CastTo<string>();


          var routeValues = GetRouteValues( actionElement );


          actionElement.RemoveAttribute( "action" );
          actionElement.RemoveAttribute( "controller" );
          actionElement.RemoveAttribute( "inherits" );


          var url = Url.Action( action, controller, routeValues );


          string attributeName;
          switch ( actionElement.Name.ToLowerInvariant() )
          {
            case "a":
              attributeName = "href";
              break;
            case "form":
              attributeName = "action";
              break;
            case "img":
            case "script":
              attributeName = "src";
              break;

            default:
              throw new Exception();//不可能出现的错误
          }

          if ( url == null )
            actionElement.Attribute( attributeName ).Remove();

          else
            actionElement.SetAttribute( attributeName, url );

        }


      }

    }




    /// <summary>
    /// 从元素标签中获取所有的路由值
    /// </summary>
    /// <param name="element">要获取分析路由值的元素</param>
    /// <returns>获取的路由值</returns>
    protected RouteValueDictionary GetRouteValues( IHtmlElement element )
    {

      var routeValues = new RouteValueDictionary();

      var inherits = element.Attribute( "inherits" ).Value();

      if ( inherits != null )
      {

        var inheritsKeys = GetInheritsKeys( inherits ).Distinct( StringComparer.OrdinalIgnoreCase );

        foreach ( var key in inheritsKeys )
          routeValues.Add( key, RouteData.Values[key] );

      }


      foreach ( var attribute in element.Attributes().Where( a => a.Name.StartsWith( "_" ) ).ToArray() )
      {

        var key = attribute.Name.Substring( 1 );
        var value = attribute.Value() ?? RouteData.Values[key];

        routeValues.Remove( key );

        routeValues.Add( key, value );
        attribute.Remove();
      }

      return routeValues;
    }


    private static readonly string wildcardCharacter = "*";

    private IEnumerable<string> GetInheritsKeys( string inherits )
    {

      foreach ( var key in inherits.Split( ',' ) )
      {

        if ( key.EqualsIgnoreCase( "action" ) || key.EqualsIgnoreCase( "controller" ) )
          continue;

        if ( key.StartsWith( wildcardCharacter ) )//以星号开头
        {
          foreach ( var k in RouteData.Values.Keys )
          {
            if ( k.EndsWith( key.Substring( wildcardCharacter.Length ) ) )
              yield return k;
          }
        }

        if ( key.EndsWith( wildcardCharacter ) )//以星号结尾
        {
          foreach ( var k in RouteData.Values.Keys )
          {
            if ( k.StartsWith( key.Substring( 0, key.Length - wildcardCharacter.Length ) ) )
              yield return k;
          }
        }


        if ( RouteData.Values.ContainsKey( key ) )
          yield return key;

      }
    }








    /// <summary>
    /// 转换容器中所有 URI 与当前请求匹配。
    /// </summary>
    /// <param name="container">确定要转换 URI 范围的容器</param>
    protected virtual void ResolveUri( IHtmlContainer container )
    {
      foreach ( var attribute in container.Descendants().SelectMany( e => e.Attributes() ).Where( a => HtmlSpecification.IsUriValue( a ) ).ToArray() )
      {
        ResolveUri( attribute );
      }
    }

    /// <summary>
    /// 转换 URI 与当前请求匹配
    /// </summary>
    /// <param name="attribute"></param>
    protected virtual void ResolveUri( IHtmlAttribute attribute )
    {
      var uriValue = attribute.AttributeValue;

      if ( string.IsNullOrWhiteSpace( uriValue ) )//对于空路径暂不作处理。
        return;

      Uri absoluteUri;
      if ( Uri.TryCreate( uriValue, UriKind.Absolute, out absoluteUri ) )//对于绝对 URI，不采取任何动作。
        return;

      if ( VirtualPathUtility.IsAbsolute( uriValue ) )//对于绝对路径，也不采取任何动作。
        return;

      if ( uriValue.StartsWith( "#" ) )//若是本路径的片段链接，也不采取任何动作。
        return;

      if ( uriValue.StartsWith( "?" ) )//若是本路径的查询链接，也不采取任何动作。
        return;



      attribute.SetValue( ResolveVirtualPath( uriValue ) );

    }


    /// <summary>
    /// 转换虚拟路径
    /// </summary>
    /// <param name="virtualPath"></param>
    /// <returns></returns>
    protected virtual string ResolveVirtualPath( string virtualPath )
    {
      if ( VirtualPathUtility.IsAppRelative( virtualPath ) )
        return VirtualPathUtility.ToAbsolute( virtualPath );

      try
      {
        return VirtualPathUtility.Combine( VirtualPathUtility.ToAbsolute( VirtualPath ), virtualPath );
      }
      catch
      {
        return virtualPath;
      }
    }


    /// <summary>
    /// 渲染部分视图
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
    /// 渲染部分视图
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
          var routeValues = GetRouteValues( partialElement );

          return helper.Action( actionName: action, controllerName: controller, routeValues: routeValues ).ToString();
        }

        else if ( view != null )
        {
          return helper.Partial( partialViewName: view ).ToString();
        }

        else if ( path != null )
        {
          if ( !VirtualPathUtility.IsAppRelative( path ) )
            throw new FormatException( "path 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取" );

          var content = HtmlProviders.LoadContent( HttpContext, path );
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
    /// 创建 HtmlHelper 对象
    /// </summary>
    /// <returns></returns>
    protected HtmlHelper MakeHelper()
    {

      var helper = new HtmlHelper( ViewContext, new ViewDataContainer( this ) );
      return helper;
    }




    public virtual void Dispose()
    {
    }


    protected class ViewDataContainer : IViewDataContainer
    {

      private ViewBase _view;

      public ViewDataContainer( ViewBase view )
      {
        _view = view;
      }


      public ViewDataDictionary ViewData
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

        _view.HttpContext.Trace.Write( "Jumony for MVC", string.Format( "Begin Render Partial: {0}", partialTag ) );
        _view.RenderPartial( element, writer );
        _view.HttpContext.Trace.Write( "Jumony for MVC", string.Format( "End Render Partial: {0}", partialTag ) );
      }
    }


  }
}
