using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ivony.Fluent;

namespace Ivony.Html.Web
{
  /// <summary>
  /// 用于渲染部分视图的 HTML 渲染代理
  /// </summary>
  public class PartialViewAdapter : PartialRenderAdapter
  {


    /// <summary>
    /// 创建 HtmlHelper 对象
    /// </summary>
    /// <returns>创建的 HtmlHelper 对象</returns>
    protected HtmlHelper MakeHelper()
    {

      var helper = new HtmlHelper( ViewContext, ViewHandler );
      return helper;
    }


    /// <summary>
    /// 创建 PartialRenderAdapter 实例
    /// </summary>
    /// <param name="urlHelper">用于产生 URL 的 URL 帮助器</param>
    /// <param name="viewContext">当前视图上下文</param>
    /// <param name="viewHandler">当前视图处理程序</param>
    public PartialViewAdapter( ViewContext viewContext, JumonyUrlHelper urlHelper, IViewHandler viewHandler )
      : base( viewContext.HttpContext, viewHandler )
    {
      if ( viewContext == null )
        throw new ArgumentNullException( "viewContext" );

      if ( urlHelper == null )
        throw new ArgumentNullException( "urlHelper" );

      if ( viewHandler == null )
        throw new ArgumentNullException( "viewHandler" );

      ViewContext = viewContext;
      ViewHandler = viewHandler;
      Url = urlHelper;

    }



    /// <summary>
    /// 获取当前视图上下文
    /// </summary>
    protected ViewContext ViewContext
    {
      get;
      private set;
    }


    /// <summary>
    /// 当前 Url 帮助器
    /// </summary>
    protected JumonyUrlHelper Url
    {
      get;
      private set;
    }

    /// <summary>
    /// 获取当前视图处理程序
    /// </summary>
    protected IViewHandler ViewHandler
    {
      get;
      private set;
    }


    /// <summary>
    /// 渲染部分视图（重写此方法以实现自定义输出 partial 元素）
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <returns></returns>
    protected override string RenderPartial( IHtmlElement partialElement )
    {
      var action = partialElement.Attribute( "action" ).Value();
      var view = partialElement.Attribute( "view" ).Value();
      var path = partialElement.Attribute( "path" ).Value();
      var name = partialElement.Attribute( "name" ).Value();

      if ( name != null )
        return RenderNamedPartial( partialElement, name );

      var helper = MakeHelper();


      try
      {
        if ( action != null )//Action 部分视图
        {
          var routeValues = Url.GetRouteValues( partialElement );

          return RenderAction( partialElement, action, partialElement.Attribute( "controller" ).Value(), routeValues );
        }

        else if ( view != null )
        {
          return RenderPartial( partialElement, view );
        }

        else if ( path != null )
        {
          RenderVirtualPath( path );
        }
      }

      catch ( ThreadAbortException )
      {
        throw;//不应屏蔽异常
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

    private string RenderAction( IHtmlElement partialElement, string actionName, string controllerName, System.Web.Routing.RouteValueDictionary routeValues )
    {
      return MakeHelper().Action( actionName, controllerName, routeValues ).ToString();
    }

    private string RenderPartial( IHtmlElement partialElement, string viewName )
    {
      return MakeHelper().Partial( viewName ).ToString();
    }
  }
}
