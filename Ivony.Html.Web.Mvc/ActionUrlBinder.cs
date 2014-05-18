using Ivony.Fluent;
using Ivony.Html.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 对值为 URL 的属性进行 MVC 转换的绑定器
  /// </summary>
  public class ActionUrlBinder : IHtmlBinder
  {



    /// <summary>
    /// 创建 ActionUrlBinder 对象
    /// </summary>
    /// <param name="urlHelper">协助生成 ASP.NET MVC URL 的帮助器</param>
    /// <param name="specification">要处理的文档所遵循的 HTML 规范</param>
    public ActionUrlBinder( JumonyUrlHelper urlHelper, HtmlSpecificationBase specification )
    {

      if ( urlHelper == null )
        throw new ArgumentNullException( "urlHelper" );

      UrlHelper = urlHelper;

      Specification = specification;
      DocumentPath = VirtualPathUtility.ToAbsolute( UrlHelper.VirtualPath );
    }


    /// <summary>
    /// 当前在处理文档的 HTML 规范
    /// </summary>
    protected HtmlSpecificationBase Specification
    {
      get;
      private set;
    }


    /// <summary>
    /// 当前文档的绝对虚拟路径
    /// </summary>
    protected string DocumentPath
    {
      get;
      private set;
    }


    /// <summary>
    /// 产生 URL 的帮助器
    /// </summary>
    public JumonyUrlHelper UrlHelper
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取当前路由数据
    /// </summary>
    protected RouteData RouteData
    {
      get { return UrlHelper.RequestContext.RouteData; }
    }




    /// <summary>
    /// 实现 BindElement 方法对元素的 URL 属性进行处理
    /// </summary>
    /// <param name="context">当前绑定上下文</param>
    /// <param name="element">当前元素上下文</param>
    public void BindElement( HtmlBindingContext context, IHtmlElement element )
    {
      if ( ProcessActionLink( element ) )//对元素进行 Action URL 处理，若处理成功，则跳过元素。
        return;

      element.Attributes().ToArray()
        .Where( attribute => Specification.IsUriValue( attribute ) )
        .ForAll( attribute => UrlHelper.ResolveUri( attribute, DocumentPath ) );
    }



    /// <summary>
    /// 派生类重写此方法对拥有 Action URL 的元素进行处理
    /// </summary>
    /// <param name="element">要处理的元素</param>
    /// <returns>元素是否包含 Action URL 并已经进行处理。</returns>
    protected virtual bool ProcessActionLink( IHtmlElement element )
    {
      if ( element.Attribute( "action" ) == null )
        return false;

      string attributeName;

      if ( element.Name.EqualsIgnoreCase( "a" ) )
        attributeName = "href";

      else if ( element.Name.EqualsIgnoreCase( "img" ) || element.Name.EqualsIgnoreCase( "script" ) )
        attributeName = "src";

      else if ( element.Name.EqualsIgnoreCase( "form" ) && element.Attribute( "controller" ) != null )
        attributeName = "action";

      else
        return false;



      var action = element.Attribute( "action" ).Value() ?? RouteData.Values["action"].CastTo<string>();
      var controller = element.Attribute( "controller" ).Value() ?? RouteData.Values["controller"].CastTo<string>();


      var routeValues = UrlHelper.GetRouteValues( element );


      element.RemoveAttribute( "action" );
      element.RemoveAttribute( "controller" );
      element.RemoveAttribute( "inherits" );


      var url = UrlHelper.Action( action, controller, routeValues );

      if ( url == null )
        element.Attribute( attributeName ).Remove();

      else
        element.SetAttribute( attributeName, url );


      return true;
    }
  }
}
