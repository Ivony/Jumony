using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Ivony.Html.Parser;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义请求映射器，Jumony请求映射器为请求获取 HTML 模板和对应的处理程序
  /// </summary>
  public interface IRequestMapper
  {

    /// <summary>
    /// 映射当前请求
    /// </summary>
    /// <param name="request">当前 HTTP 请求</param>
    /// <returns>映射结果</returns>
    RequestMapping MapRequest( HttpRequestBase request );

  }


  /// <summary>
  /// 默认的请求映射器
  /// </summary>
  public class DefaultRequestMapper : IRequestMapper
  {

    private static readonly string[] allowsExtensions = new[] { ".html", ".htm", ".aspx" };

    /// <summary>
    /// 映射当前请求
    /// </summary>
    /// <param name="request">当前 HTTP 请求</param>
    /// <returns>映射结果</returns>
    public RequestMapping MapRequest( HttpRequestBase request )
    {
      var virtualPath = request.AppRelativeCurrentExecutionFilePath;

      if ( !allowsExtensions.Contains( VirtualPathUtility.GetExtension( virtualPath ), StringComparer.InvariantCultureIgnoreCase ) )
        return null;

      if ( !FileExists( virtualPath ) )
        return null;

      var handlerPath = virtualPath + ".ashx";
      if ( !FileExists( handlerPath ) )
        return null;

      var handler = BuildManager.CreateInstanceFromVirtualPath( handlerPath, typeof( JumonyHandler ) ) as JumonyHandler;
      if ( handler == null )
        return null;

      return new RequestMapping( this, virtualPath, handler );
    }


    private static bool FileExists( string virtualPath )
    {
      return HostingEnvironment.VirtualPathProvider.FileExists( virtualPath );
    }

  }



}
