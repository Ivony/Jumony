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
    /// <param name="virtualPath">HTML 页面的虚拟路径</param>
    public PageView( string virtualPath )
    {
      VirtualPath = virtualPath;
    }


    /// <summary>
    /// 创建一个页面视图实例
    /// </summary>
    protected PageView()
    {

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

      EnsurePageView();

      return Document.Render( RenderAdapters.ToArray() );
    }


    /// <summary>
    /// 确保当前确实是在渲染一个页面
    /// </summary>
    protected virtual void EnsurePageView()
    {

      if ( MvcEnvironment.Configuration.EnablePageViewRenderChildAction )
        return;

      if ( ViewContext.IsChildAction )
        throw new InvalidOperationException( "当前设置禁止使用 PageView 来渲染子请求！" );

    }


    /// <summary>
    /// 文档主处理流程
    /// </summary>
    protected override void ProcessMain()
    {
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin LoadDocument" );
      Document = LoadDocument();
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End LoadDocument" );


      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin ProcessDocument" );
      ProcessDocument();
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End ProcessDocument" );


      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin ProcessActionRoutes" );
      ProcessActionUrls( Document );
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End ProcessActionRoutes" );


      HttpContext.Trace.Write( "Jumony for MVC - PageView", "Begin ResolveUri" );
      ResolveUri( Document );
      HttpContext.Trace.Write( "Jumony for MVC - PageView", "End ResolveUri" );

      if ( !MvcEnvironment.Configuration.DisableGeneratorTag )
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
