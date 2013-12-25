using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using Ivony.Fluent;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 
  /// </summary>
  public class JumonyPartialHandler : JumonyHandler
  {


    /// <summary>
    /// 重写 CreateScope 方法，获取文档的 body 元素
    /// </summary>
    /// <param name="virtualPath">HTML 文档的虚拟路径</param>
    /// <returns>文档的处理范围</returns>
    protected IHtmlContainer CreateScope( string virtualPath )
    {
      var document = (IHtmlDocument) LoadDocument( virtualPath );

      var body = document.Find( "body" ).SingleOrDefault();

      if ( body == null )
        return document;

      else
        return body;
    }


    /// <summary>
    /// 重写 CreateResponse 方法，创建部分视图响应结果
    /// </summary>
    /// <param name="content">响应内容</param>
    /// <returns>可缓存的响应结果</returns>
    protected override ICachedResponse CreateResponse( string content )
    {
      return new PartialResponse() { Content = content };
    }




    public static string RenderPartial( HttpContextBase context, string virtualPath )
    {

      var handler = HtmlHandlerProvider.GetHandler( virtualPath );

      if ( handler == null )
        return HtmlServices.LoadContent( virtualPath ).Content;

      var partialHandler = new JumonyPartialHandler();
      return partialHandler.RenderPartial( context, virtualPath, handler );


    }


    private HttpContextBase _context;

    protected override HttpContextBase HttpContext
    {
      get { return _context; }
    }


    public string RenderPartial( HttpContextBase context, string virtualPath, IHtmlHandler handler )
    {
      _context = context;
      return ProcessRequest( new HtmlRequestContext( context, virtualPath, CreateScope( virtualPath ) ), handler ).CastTo<PartialResponse>().Content;
    }
  }
}
