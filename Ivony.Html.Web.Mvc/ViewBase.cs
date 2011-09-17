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

namespace Ivony.Html.Web.Mvc
{


  /// <summary>
  /// 所有 HTML 视图处理程序的基类，实现 IView 接口，并提供内置的 HTML 扩展功能以及视图结果的缓存。
  /// </summary>
  public abstract class ViewBase : IView, ICachableResult
  {

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
    protected string VirtualPath
    {
      get { return _virtualPath; }
      set
      {

        if ( !VirtualPathUtility.IsAppRelative( value ) )
          throw new FormatException( "VirtualPath 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取" );
        _virtualPath = value;
      }
    }


    /// <summary>
    /// 获取视图上下文
    /// </summary>
    protected ViewContext ViewContext
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


      while ( viewContext.IsChildAction )
      {
        viewContext = viewContext.ParentActionViewContext;//循环上溯最原始的视图上下文
      }
      RawViewContext = viewContext;


      Url = new UrlHelper( RequestContext );

      HttpContext.Trace.Write( "Jumony for MVC", "Begin Process" );
      OnPreProcess( this );
      ProcessMain();
      OnPostProcess( this );
      HttpContext.Trace.Write( "Jumony for MVC", "End Process" );


      HttpContext.Trace.Write( "Jumony for MVC", "Begin Render" );
      OnPreRender( this, writer );
      var content = RenderContent();
      OnPostRender( this, writer );
      HttpContext.Trace.Write( "Jumony for MVC", "End Render" );

      UpdateCache( content );

      writer.Write( content );
    }

    private void OnPreProcess( ViewBase viewBase )
    {
      throw new NotImplementedException();
    }

    private void OnPostProcess( ViewBase viewBase )
    {
      throw new NotImplementedException();
    }

    private void OnPreRender( ViewBase viewBase, TextWriter writer )
    {
      throw new NotImplementedException();
    }

    protected virtual void OnPostRender( ViewBase viewBase, TextWriter writer )
    {
      throw new NotImplementedException();
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


    ICachedResponse ICachableResult.GetCachedResponse()
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
      var elements = container.Find( "a[action] , img[action] , form[action][controller]" );

      foreach ( var actionElement in elements )
      {

        lock ( actionElement.SyncRoot )//锁住元素不被修改
        {

          var action = actionElement.Attribute( "action" ).Value() ?? RouteData.Values["action"].CastTo<string>();
          var controller = actionElement.Attribute( "controller" ).Value() ?? RouteData.Values["controller"].CastTo<string>();


          var routeValues = GetRouteValues( actionElement );


          actionElement.Attribute( "action" ).Remove();

          var controllerAttribute = actionElement.Attribute( "controller" );
          if ( controllerAttribute != null )
            controllerAttribute.Remove();


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
        foreach ( var key in inherits.Split( ',' ) )
        {

          if ( key.EqualsIgnoreCase( "action" ) || key.EqualsIgnoreCase( "controller" ) )
            continue;

          if ( RouteData.Values.ContainsKey( key ) )
            routeValues.Add( key, RouteData.Values[key] );

        }
      }


      foreach ( var attribute in element.Attributes().Where( a => a.Name.StartsWith( "_" ) ).ToArray() )
      {

        var key = attribute.Name.Substring( 1 );
        var value = attribute.Value();

        routeValues.Remove( key );

        routeValues.Add( key, value );
        attribute.Remove();
      }

      return routeValues;
    }








    /// <summary>
    /// 转换容器中所有 URI 与当前请求匹配。
    /// </summary>
    /// <param name="container">确定要转换 URI 范围的容器</param>
    protected virtual void ResolveUri( IHtmlContainer container )
    {
      //ResolveUri( container, container.Document.DocumentUri );


      foreach ( var attribute in container.Descendants().SelectMany( e => e.Attributes() ).Where( a => HtmlSpecification.IsUriValue( a ) ).ToArray() )
      {
        ResolveUri( attribute );
      }

      /**/
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

    protected virtual string ResolveVirtualPath( string virtualPath )
    {
      return VirtualPathUtility.Combine( VirtualPathUtility.ToAbsolute( VirtualPath ), virtualPath );
    }

    /// <summary>
    /// 根据文档基准 URI 转换容器中所有 URI 与当前请求匹配。
    /// </summary>
    /// <param name="container">确定要转换 URI 范围的容器</param>
    /// <param name="baseUri">文档的基准 URI</param>
    protected virtual void ResolveUri( IHtmlContainer container, Uri baseUri )
    {
      if ( baseUri == null )
        throw new ArgumentNullException( "baseUri" );

      foreach ( var attribute in container.Descendants().SelectMany( e => e.Attributes() ).Where( a => HtmlSpecification.IsUriValue( a ) ).ToArray() )
      {

        var value = attribute.Value();
        if ( string.IsNullOrEmpty( value ) )
          continue;


        if ( value.StartsWith( "~/" ) )
          value = VirtualPathUtility.ToAbsolute( value );

        Uri uri;

        if ( !Uri.TryCreate( baseUri, value, out uri ) )
          continue;

        if ( uri.Equals( baseUri ) )
          continue;


        //uri = HttpContext.Request.Url.MakeRelativeUri( uri );

        attribute.Element.SetAttribute( attribute.Name, uri.AbsolutePath );

      }
    }


    /// <summary>
    /// 渲染部分视图
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <param name="writer"></param>
    protected virtual void RenderPartial( IHtmlElement partialElement, TextWriter writer )
    {

      var action = partialElement.Attribute( "action" ).Value();
      var view = partialElement.Attribute( "view" ).Value();
      var handler = partialElement.Attribute( "handler" ).Value();

      if ( action != null && view != null )
        throw new NotSupportedException( "无法处理的partial标签：" + ContentExtensions.GenerateTagHtml( partialElement, false ) );


      if ( action != null )
      {

        var controller = partialElement.Attribute( "controller" ).Value() ?? (string) RouteData.Values["controller"];

        var helper = MakeHelper();

        var routeValues = GetRouteValues( partialElement );

        try
        {
          writer.Write( helper.Action( actionName: action, controllerName: controller, routeValues: routeValues ) );
        }
        catch ( Exception e )
        {
          if ( MvcEnvironment.Configuration.IgnorePartialRenderException )
            writer.Write( "<!--parital view render failed-->" );
        }

        return;
      }


      if ( view != null )
      {

        var helper = MakeHelper();

        try
        {
          writer.Write( helper.Partial( partialViewName: view ) );
        }
        catch ( Exception e )
        {
          if ( MvcEnvironment.Configuration.IgnorePartialRenderException )
            writer.Write( "<!--parital view render failed-->" );
        }

        return;

      }

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
