using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 请求上下文
  /// </summary>
  public class HtmlRequestContext
  {


    /// <summary>
    /// 创建 HtmlRequestContext 实例
    /// </summary>
    /// <param name="httpContext">当前 HTTP 请求上下文</param>
    /// <param name="virtualPath">当前处理的文档的虚拟路径</param>
    /// <param name="scope">当前要处理的 HTML 文档范围</param>
    public HtmlRequestContext( HttpContextBase httpContext, string virtualPath, IHtmlContainer scope )
    {

      if ( httpContext == null )
        throw new ArgumentNullException( "httpContext" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( scope == null )
        throw new ArgumentException( "scope" );


      HttpContext = httpContext;
      VirtualPath = virtualPath;
      Scope = scope;
    }



    /// <summary>
    /// 当前 HTTP 请求
    /// </summary>
    public HttpContextBase HttpContext { get; private set; }


    /// <summary>
    /// 当前处理的文档的虚拟路径
    /// </summary>
    public string VirtualPath { get; private set; }


    /// <summary>
    /// 要处理的 HTML 范围
    /// </summary>
    public IHtmlContainer Scope { get; private set; }


  }
}
