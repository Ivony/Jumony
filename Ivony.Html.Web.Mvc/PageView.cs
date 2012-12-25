using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web.Routing;
using System.Web;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Web.Caching;
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 页面视图
  /// </summary>
  public abstract class PageView : ViewBase
  {

    /// <summary>
    /// 创建一个页面视图实例
    /// </summary>
    /// <param name="virtualPath">HTML 视图的虚拟路径</param>
    /// <param name="isPartial">是否使用部分视图渲染模式</param>
    public PageView( string virtualPath, bool isPartial = false )
    {
      Initialize( virtualPath, isPartial );
    }


    /// <summary>
    /// 创建一个页面视图实例
    /// </summary>
    protected PageView()
    {

    }


    private bool _initialized = false;

    /// <summary>
    /// 初始化视图，必须在处理视图之前先初始化视图
    /// </summary>
    /// <param name="virtualPath">HTML 视图的虚拟路径</param>
    /// <param name="partialMode">是否启用部分视图渲染模式</param>
    protected void Initialize( string virtualPath, bool partialMode )
    {
      if ( _initialized )
        throw new InvalidOperationException( "视图已经初始化" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw new FormatException( "VirtualPath 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取" );

      VirtualPath = virtualPath;
      PartialMode = PartialMode;

      _initialized = true;
    }


    /// <summary>
    /// 获取或设置 HTML 视图的虚拟路径
    /// </summary>
    public string VirtualPath
    {
      get;
      private set;
    }


    /// <summary>
    /// 是否应当将页面当作部分视图来处理
    /// </summary>
    protected bool PartialMode
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取页面文档对象
    /// </summary>
    public IHtmlDocument Document
    {
      get;
      private set;
    }


    /// <summary>
    /// 渲染输出内容
    /// </summary>
    /// <returns>渲染的 HTML</returns>
    protected override string RenderContent()
    {

      if ( PartialMode )
      {
        var body = Document.Find( "body" ).SingleOrDefault();

        if ( body != null )
        {
          var writer = new StringWriter();

          foreach ( var node in body.Nodes() )
            node.Render( writer, RenderAdapters.ToArray() );


          return writer.ToString();
        }
      }


      return Document.Render( RenderAdapters.ToArray() );
    }





    /// <summary>
    /// 文档主处理流程
    /// </summary>
    protected override void ProcessMain()
    {

      if ( !_initialized )
        throw new InvalidOperationException( "视图尚未初始化" );

      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin LoadDocument" );
      Document = LoadDocument( VirtualPath );
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End LoadDocument" );


      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin ProcessDocument" );
      ProcessDocument();
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End ProcessDocument" );


      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin ProcessActionRoutes" );
      ProcessActionUrls( Document );
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End ProcessActionRoutes" );


      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin ResolveUri" );
      ResolveUri( Document, VirtualPath );
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End ResolveUri" );

      if ( !MvcEnvironment.Configuration.DisableGeneratorTag && !PartialMode )
        AddGeneratorMetaData();
    }


    /// <summary>
    /// 派生类重写此方法自定义文档处理逻辑
    /// </summary>
    protected abstract void ProcessDocument();



    private void AddGeneratorMetaData()
    {
      var modifier = Document.DomModifier;
      if ( modifier != null )
      {
        var header = Document.Find( "head" ).FirstOrDefault();

        if ( header != null )
        {

          var metaElement = modifier.AddElement( header, "meta" );

          metaElement.SetAttribute( "name", "generator" );
          metaElement.SetAttribute( "content", "Jumony" );
        }
      }
    }
  }



  internal class GenericPageView : PageView
  {

    public GenericPageView( string virtualPath )
      : base( virtualPath )
    {

    }


    protected override void ProcessDocument()
    {
      return;
    }
  }
}
