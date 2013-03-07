using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ivony.Fluent;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 扩展 UrlHelper 提供 Jumony 特有的方法。
  /// </summary>
  public class JumonyUrlHelper : UrlHelper
  {

    /// <summary>
    /// 创建 JumonyUrlHelper 实例
    /// </summary>
    /// <param name="view"></param>
    public JumonyUrlHelper( ViewBase view )
      : base( view.ViewContext.RequestContext )
    {
      VirtualPath = view.VirtualPath;
    }


    /// <summary>
    /// 视图基路径
    /// </summary>
    public string VirtualPath
    {
      get;
      private set;
    }


    private UrlHelper Url { get { return this; } }

    private RouteData RouteData { get { return RequestContext.RouteData; } }


    /// <summary>
    /// 处理所有 Action 路由
    /// </summary>
    /// <param name="container">包含所有要处理元素的 HTML 容器</param>
    internal void ProcessActionUrls( IHtmlContainer container )
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


    Dictionary<IHtmlElement, IDictionary<string, string>> elementRouteValuesTable = new Dictionary<IHtmlElement, IDictionary<string, string>>();


    /// <summary>
    /// 为指定元素设置路由参数值，此设置会覆盖元素属性上同名路由参数的设置。
    /// </summary>
    /// <param name="element">要设置路由参数的元素</param>
    /// <param name="routeValues">要设置的路由参数</param>
    /// <exception cref="System.NotSupportedException">元素没有支持路由的属性</exception>
    public void SetRouteValues( IHtmlElement element, IDictionary<string, string> routeValues )
    {

    }



    /// <summary>
    /// 从元素标签中获取路由的虚拟路径
    /// </summary>
    /// <param name="element">要获取分析路由虚拟路径的元素</param>
    /// <returns>获取的虚拟路径</returns>
    public string GetRouteUrl( IHtmlElement element )
    {
      return GetRouteUrl( element, true );
    }


    /// <summary>
    /// 从元素标签中获取路由的虚拟路径
    /// </summary>
    /// <param name="element">要获取分析路由虚拟路径的元素</param>
    /// <param name="clearRouteAttributes">是否清理路由属性设置</param>
    /// <returns>获取的虚拟路径</returns>
    internal string GetRouteUrl( IHtmlElement element, bool clearRouteAttributes )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( element.Attribute( "action" ) == null )
        throw new InvalidOperationException();

      var action = element.Attribute( "action" ).Value() ?? RouteData.Values["action"].CastTo<string>();
      var controller = element.Attribute( "controller" ).Value() ?? RouteData.Values["controller"].CastTo<string>();


      var routeValues = GetRouteValues( element, clearRouteAttributes );


      if ( clearRouteAttributes )
      {
        element.RemoveAttribute( "action" );
        element.RemoveAttribute( "controller" );
        element.RemoveAttribute( "inherits" );
      }

      return Url.Action( action, controller, routeValues );
    }


    /// <summary>
    /// 从元素标签中获取所有的路由值
    /// </summary>
    /// <param name="element">要获取分析路由值的元素</param>
    /// <returns>获取的路由值</returns>
    public RouteValueDictionary GetRouteValues( IHtmlElement element )
    {
      return GetRouteValues( element, true );
    }


    internal RouteValueDictionary GetRouteValues( IHtmlElement element, bool clearRouteAttributes )
    {

      var routeValues = new RouteValueDictionary();

      var inherits = element.Attribute( "inherits" ).Value();

      if ( inherits != null )
      {

        var inheritsKeys = GetInheritsKeys( inherits );

        foreach ( var key in inheritsKeys )
          routeValues.Add( key, RequestContext.RouteData.Values[key] );

      }


      CustomRouteValues( element, "_", routeValues, clearRouteAttributes );
      CustomRouteValues( element, "route-", routeValues, clearRouteAttributes );

      return routeValues;
    }

    private void CustomRouteValues( IHtmlElement element, string prefix, RouteValueDictionary routeValues, bool clearRouteAttributes )
    {
      foreach ( var attribute in element.Attributes().Where( a => a.Name.StartsWith( prefix ) ).ToArray() )
      {

        var key = attribute.Name.Substring( prefix.Length );
        var value = attribute.Value() ?? RouteData.Values[key];

        routeValues.Remove( key );

        routeValues.Add( key, value );

        if ( clearRouteAttributes )
          attribute.Remove();
      }
    }


    private static readonly string wildcardCharacter = "*";

    private IEnumerable<string> GetInheritsKeys( string inherits )
    {
      HashSet<string> result = new HashSet<string>( StringComparer.OrdinalIgnoreCase );

      foreach ( var keySetting in inherits.Split( ',' ) )
      {
        if ( keySetting == wildcardCharacter )
        {
          foreach ( var key in RouteData.Values.Keys )
            result.Add( key );

          break;
        }

        if ( keySetting.StartsWith( wildcardCharacter ) )//以星号开头
        {
          foreach ( var k in RouteData.Values.Keys )
          {
            if ( k.EndsWith( keySetting.Substring( wildcardCharacter.Length ) ) )
              result.Add( k );
          }
        }

        if ( keySetting.EndsWith( wildcardCharacter ) )//以星号结尾
        {
          foreach ( var k in RouteData.Values.Keys )
          {
            if ( k.StartsWith( keySetting.Substring( 0, keySetting.Length - wildcardCharacter.Length ) ) )
              result.Add( k );
          }
        }


        if ( RouteData.Values.ContainsKey( keySetting ) )
          result.Add( keySetting );

      }

      result.Remove( "controller" );
      result.Remove( "action" );

      return result;
    }


    /// <summary>
    /// 转换容器中所有 URI 与当前请求匹配。
    /// </summary>
    /// <param name="container">确定要转换 URI 范围的容器</param>
    public void ResolveUri( IHtmlContainer container )
    {
      ResolveUri( container, VirtualPath );
    }

    /// <summary>
    /// 转换容器中所有 URI 与当前请求匹配。
    /// </summary>
    /// <param name="container">确定要转换 URI 范围的容器</param>
    /// <param name="baseVirtualPath">容器的基路径</param>
    public void ResolveUri( IHtmlContainer container, string baseVirtualPath )
    {

      var absoluteBase = VirtualPathUtility.ToAbsolute( baseVirtualPath );
      foreach ( var attribute in container.Descendants().SelectMany( e => e.Attributes() ).Where( a => HtmlSpecification.IsUriValue( a ) ).ToArray() )
      {
        ResolveUri( attribute, absoluteBase );
      }
    }

    /// <summary>
    /// 转换 URI 与当前请求匹配
    /// </summary>
    /// <param name="attribute">HTML 属性</param>
    /// <param name="baseVirtualPath">基路径</param>
    public void ResolveUri( IHtmlAttribute attribute, string baseVirtualPath )
    {
      if ( attribute == null )
        throw new ArgumentNullException( "attribute" );

      if ( baseVirtualPath == null )
        throw new ArgumentNullException( "baseVirtualPath" );



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

      attribute.SetValue( ResolveVirtualPath( baseVirtualPath, uriValue ) );

    }


    /// <summary>
    /// 转换虚拟路径
    /// </summary>
    /// <param name="baseVirtualPath">基路径</param>
    /// <param name="virtualPath">设置的虚拟路径（相对或绝对）</param>
    /// <returns></returns>
    public string ResolveVirtualPath( string baseVirtualPath, string virtualPath )
    {

      if ( baseVirtualPath == null )
        throw new ArgumentNullException( "baseVitualPath" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );


      if ( VirtualPathUtility.IsAppRelative( virtualPath ) )
        return VirtualPathUtility.ToAbsolute( virtualPath );

      try
      {
        return VirtualPathUtility.Combine( baseVirtualPath, virtualPath );
      }
      catch
      {
        return virtualPath;
      }
    }

  }
}
