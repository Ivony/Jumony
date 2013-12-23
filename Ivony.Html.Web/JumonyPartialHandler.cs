using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    protected override IHtmlContainer CreateScope( string virtualPath )
    {
      var document = (IHtmlDocument) base.CreateScope( virtualPath );

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

  }
}
