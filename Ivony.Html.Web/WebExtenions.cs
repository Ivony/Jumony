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


    private const string requestDataToken = "Jumony_HttpContext_RequestMapResult";

    public static RequestMapResult GetMapperResult( this HttpContext context )
    {
      return GetMapperResult( new HttpContextWrapper( context ) );
    }

    public static RequestMapResult GetMapperResult( this HttpContextBase context )
    {
      return (RequestMapResult) context.Items[requestDataToken];
    }

    internal static void SetMapResult( this HttpContext context, RequestMapResult data )
    {
      context.Items[requestDataToken] = data;
    }




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

      var headElement = htmlElement.Elements( "head" ).SingleOrDefault();
      if ( headElement == null )
      {
        var freeHead = htmlElement.Document.GetNodeFactory().CreateElement( "head" );
        headElement = freeHead.InsertTo( htmlElement, 0 );
      }

      return new HtmlHead( headElement );
    }

  }
}
