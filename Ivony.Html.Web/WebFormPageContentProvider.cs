using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{
  /// <summary>
  /// ASPX 文件内容加载器，用于从 ASPX 动态页面中加载内容
  /// </summary>
  public class WebFormPageContentProvider : IHtmlContentProvider
  {
    private static readonly string[] allowsExtensions = new[] { ".aspx" };

    /// <summary>
    /// 读取 ASPX 页面所呈现的 HTML 内容
    /// </summary>
    /// <param name="virtualPath">ASPX 文件路径</param>
    /// <returns>ASPX 页面所呈现的 HTML 内容</returns>
    public HtmlContentResult LoadContent( string virtualPath )
    {

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        return null;



      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ) ) )
        return null;


      using ( var writer = new StringWriter() )
      {
        HttpContext.Current.Server.Execute( virtualPath, writer, false );

        return new HtmlContentResult( writer.ToString(), virtualPath );
      }
    }
  }
}
