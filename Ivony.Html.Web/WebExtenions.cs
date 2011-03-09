using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 提供一些 Web 相关的扩展方法。
  /// </summary>
  public static class WebExtenions
  {


    private const string requestDataToken = "Jumony_HttpContext_RequestMapping";

    public static RequestMapping GetMapping( this HttpContext context )
    {
      return GetMapping( new HttpContextWrapper( context ) );
    }

    public static RequestMapping GetMapping( this HttpContextBase context )
    {
      return (RequestMapping) context.Items[requestDataToken];
    }

    public static void SetMapping( this HttpContext context, RequestMapping data )
    {
      context.Items[requestDataToken] = data;
    }

    public static void SetMapping( this HttpContextBase context, RequestMapping data )
    {
      context.Items[requestDataToken] = data;
    }



    /*
    /// <summary>
    /// 获取 HtmlHead 对象，用于操作 &lt;head&gt; 元素
    /// </summary>
    /// <param name="document">要获取 &lt;head&gt; 元素的文档</param>
    /// <returns></returns>
    public static HtmlHead Head( this IHtmlDocument document )
    {
      if ( document == null )
        throw new ArgumentNullException( "document" );

      var htmlElement = document.Elements( "html" ).SingleOrDefault();

      if ( htmlElement == null )
        return null;

      var headElement = EnsureHeader( htmlElement );

      return new HtmlHead( headElement );
    }



    private static IHtmlElement EnsureHeader( IHtmlElement htmlElement )
    {
      var headElement = htmlElement.Elements( "head" ).SingleOrDefault();
      if ( headElement == null )
      {
        var freeHead = htmlElement.Document.GetNodeFactory().CreateElement( "head" );
        headElement = freeHead.InsertTo( htmlElement, 0 );
      }
      return headElement;
    }
    */



  }
}
