using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Ivony.Fluent;
using Ivony.Web;
using Ivony.Html;
using Ivony.Html.ExpandedAPI;

namespace Ivony.Html.Web
{
  /// <summary>
  /// 管理网站的样式和脚本资源
  /// </summary>
  public class ResourceManager
  {


    private string[] styleFiles;
    private string[] scriptFiles;



    /// <summary>
    /// 创建资源管理器
    /// </summary>
    /// <param name="virtualPath">要查找资源的虚拟路径</param>
    public ResourceManager( string virtualPath )
    {
      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      var root = HostingEnvironment.VirtualPathProvider.GetDirectory( VirtualPathUtility.ToAbsolute( virtualPath ) );

      styleFiles = root.EnumerateFiles().Select( file => file.VirtualPath ).Where( path => VirtualPathUtility.GetExtension( path ).Equals( ".css" ) ).ToArray();
      scriptFiles = root.EnumerateFiles().Select( file => file.VirtualPath ).Where( path => VirtualPathUtility.GetExtension( path ).Equals( ".js" ) ).ToArray();

    }


    /// <summary>
    /// 获取所有 CSS 样式资源的路径
    /// </summary>
    public string[] AllCssStyleFiles { get { return styleFiles.Copy(); } }

    /// <summary>
    /// 获取所有 JavaScript 脚本资源的路径
    /// </summary>
    public string[] AllJavaScriptFiles { get { return scriptFiles.Copy(); } }


    /// <summary>
    /// 添加所有资源引用
    /// </summary>
    /// <param name="document"></param>
    /// <param name="clearReferenceFirst"></param>
    public void AddAllReference( IHtmlDocument document, bool clearReferenceFirst = true )
    {

      if ( document == null )
        return;

      if ( clearReferenceFirst )
        ClearAllReference( document );

      var headElement = document.FindFirstOrDefault( "head" );
      if ( headElement == null )
      {
        var firstElement = document.Elements().FirstOrDefault();

        if ( firstElement != null )
          headElement = firstElement.AddElementBeforeSelf( "head" );
        else
          headElement = document.AddElement( 0, "head" );
      }

      AddStyleReferences( headElement, styleFiles );
      AddScriptReferences( headElement, scriptFiles );
    }

    private void AddStyleReferences( IHtmlElement headElement, string[] styleFiles )
    {
      foreach ( var path in styleFiles )
        headElement.AddElement( "link" )
          .SetAttribute( "rel", "stylesheet" )
          .SetAttribute( "type", "text/css" )
          .SetAttribute( "href", path );
    }

    private void AddScriptReferences( IHtmlElement headElement, string[] scriptFiles )
    {
      foreach ( var path in scriptFiles )
        headElement.AddElement( "script" )
          .SetAttribute( "type", "text/javascript" )
          .SetAttribute( "src", path );
    }

    /// <summary>
    /// 清除文档中所有的资源文件引用
    /// </summary>
    /// <param name="document">要清除资源文件引用的文档</param>
    /// <param name="headScopeOnly">是否仅清除 &lt;head&gt; 元素内部的引用</param>
    public void ClearAllReference( IHtmlDocument document, bool headScopeOnly = true )
    {
      if ( document == null )
        return;


      if ( headScopeOnly )
        document.Find( "head link[rel=stylesheet][href$=.css], head script[src$=.js]" ).Remove();
      else
        document.Find( "link[rel=stylesheet][href$=.css], script[src$=.js]" ).Remove();
    }


  }
}
