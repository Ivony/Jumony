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


    /// <summary>
    /// 获取或设置 HTML 视图的虚拟路径，此属性必须在处理视图前进行初始化
    /// </summary>
    protected string VirtualPath
    {
      get;
      set;
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
    /// 获取 HTTP 上下文
    /// </summary>
    protected HttpContextBase HttpContext
    {
      get { return ViewContext.HttpContext; }
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
        viewContext = viewContext.ParentActionViewContext;
      }

      RawViewContext = viewContext;

      Url = new UrlHelper( RequestContext );

      ProcessMain();

      var content = RenderContent();

      UpdateCache( content );

      writer.Write( content );
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
    public ICachedResponse CachedResponse
    {
      get;
      protected set;
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
    /// 派生类重写此方法渲染 HTML 内容。
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
    /// 处理 Action 链接
    /// </summary>
    /// <param name="container"></param>
    protected void ProcessActionLinks( IHtmlContainer container )
    {
      var links = container.Find( "a[action]" );

      foreach ( var actionLink in links )
      {

        lock ( actionLink )
        {

          var action = actionLink.Attribute( "action" ).Value() ?? RouteData.Values["action"];
          var controller = actionLink.Attribute( "controller" ).Value() ?? RouteData.Values["controller"];

          var routeValues = new RouteValueDictionary();

          routeValues["action"] = action;
          routeValues["controller"] = controller;


          foreach ( var attribute in actionLink.Attributes().Where( a => a.Name.StartsWith( "_" ) ).ToArray() )
          {

            var key = attribute.Name.Substring( 1 );
            var value = attribute.Value();

            routeValues.Remove( key );

            routeValues.Add( key, value );
            attribute.Remove();
          }


          actionLink.Attribute( "action" ).Remove();

          var controllerAttribute = actionLink.Attribute( "controller" );
          if ( controllerAttribute != null )
            controllerAttribute.Remove();




          var href = Url.RouteUrl( routeValues );

          if ( href == null )
            actionLink.Attribute( "href" ).Remove();

          else
            actionLink.SetAttribute( "href", href );

        }


      }

    }



    /// <summary>
    /// 转换容器中所有 URI 与当前请求匹配。
    /// </summary>
    /// <param name="container">确定要转换 URI 范围的容器</param>
    protected virtual void ResolveUri( IHtmlContainer container )
    {
      ResolveUri( container, container.Document.DocumentUri );
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


        //uri = uri.MakeRelativeUri( HttpContext.Request.Url );

        attribute.Element.SetAttribute( attribute.Name, uri.AbsoluteUri );

      }
    }


    /// <summary>
    /// 渲染部分视图
    /// </summary>
    /// <param name="partialElement"></param>
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

        writer.Write( helper.Action( actionName: action, controllerName: controller ) );

        return;
      }


      if ( view != null )
      {

        var helper = MakeHelper();

        writer.Write( helper.Partial( partialViewName: view ) );

        return;

      }




    }

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


    public class PartialRenderAdapter : HtmlElementAdapter
    {

      private ViewBase _view;

      public PartialRenderAdapter( ViewBase view )
      {
        _view = view;
      }

      protected override string CssSelector
      {
        get { return "partial"; }
      }

      public override void Render( IHtmlElement element, TextWriter writer )
      {
        _view.RenderPartial( element, writer );
      }
    }


  }
}
