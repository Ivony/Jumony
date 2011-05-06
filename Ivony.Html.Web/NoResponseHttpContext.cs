using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

#pragma warning disable 1591

namespace Ivony.Html.Web
{
  /// <summary>
  /// 提供一个禁用响应功能的 HttpContext 对象的包装
  /// </summary>
  public class NoResponseHttpContext : HttpContextWrapper
  {

    /// <summary>
    /// 创建 NoResponseHttpContext 实例
    /// </summary>
    /// <param name="context"></param>
    public NoResponseHttpContext( HttpContext context ) : base( context ) { }

    public override HttpResponseBase Response
    {
      get { throw new NotSupportedException( "在当前上下文中，无法产生任何响应" ); }
    }

    public override void RemapHandler( IHttpHandler handler )
    {
      throw new NotSupportedException( "此操作在当前上下文中不受支持" );
    }

    public override void RewritePath( string filePath, string pathInfo, string queryString )
    {
      throw new NotSupportedException( "此操作在当前上下文中不受支持" );
    }

    public override void RewritePath( string filePath, string pathInfo, string queryString, bool setClientFilePath )
    {
      throw new NotSupportedException( "此操作在当前上下文中不受支持" );
    }

    public override void RewritePath( string path )
    {
      throw new NotSupportedException( "此操作在当前上下文中不受支持" );
    }

    public override void RewritePath( string path, bool rebaseClientPath )
    {
      throw new NotSupportedException( "此操作在当前上下文中不受支持" );
    }

  }
}
