using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 部分视图
  /// </summary>
  public abstract class PartialView : ViewBase
  {

    /// <summary>
    /// 创建部分视图实例
    /// </summary>
    protected PartialView()
    {

    }


    /// <summary>
    /// 创建部分视图实例
    /// </summary>
    /// <param name="virtualPath">部分视图的虚拟路径</param>
    protected PartialView( string virtualPath )
    {
      VirtualPath = virtualPath;
    }


    /// <summary>
    /// 需要处理的容器
    /// </summary>
    public IHtmlContainer Container
    {
      get;
      private set;
    }

    /// <summary>
    /// 部分视图主处理流程
    /// </summary>
    protected override void ProcessMain()
    {

      if ( Container == null )
      {
        HttpContext.Trace.Write( "Jumony for MVC - PartialView", "Begin LoadContainer" );
        Container = LoadContainer();
        HttpContext.Trace.Write( "Jumony for MVC - PartialView", "End LoadContainer" );
      }


      HttpContext.Trace.Write( "Jumony for MVC - PartialView", "Begin ProcessContaner" );
      ProcessContainer();
      HttpContext.Trace.Write( "Jumony for MVC - PartialView", "End ProcessContaner" );


      HttpContext.Trace.Write( "Jumony for MVC - PartialView", "Begin ProcessActionLinks" );
      ProcessActionUrls( Container );
      HttpContext.Trace.Write( "Jumony for MVC - PartialView", "End ProcessActionLinks" );

      if ( VirtualPath != null )//若不是内嵌部分视图，则应当进行 URL 转换。
      {
        HttpContext.Trace.Write( "Jumony for MVC - PartialView", "Begin ResolveUri" );
        ResolveUri( Container );
        HttpContext.Trace.Write( "Jumony for MVC - PartialView", "End ResolveUri" );
      }
    }

    /// <summary>
    /// 处理部分视图
    /// </summary>
    protected abstract void ProcessContainer();


    /// <summary>
    /// 加载部分视图
    /// </summary>
    /// <returns></returns>
    protected virtual IHtmlContainer LoadContainer()
    {
      var document = LoadDocument();

      var body = document.Find( "body" ).SingleOrDefault();

      if ( body == null )
        return document;

      else
        return body;
    }

    /// <summary>
    /// 渲染部分视图
    /// </summary>
    /// <returns>渲染结果</returns>
    protected override string RenderContent()
    {
      var writer = new StringWriter();


      foreach ( var node in Container.Nodes() )
        node.Render( writer, RenderAdapters.ToArray() );


      return writer.ToString();
    }
  }


  /// <summary>
  /// 标准部分视图处理程序，当没有指定部分视图处理类时使用
  /// </summary>
  public class GenericPartialView : PartialView
  {

    /// <summary>
    /// 创建 GenericPartialView 实例
    /// </summary>
    /// <param name="virtualPath"></param>
    public GenericPartialView( string virtualPath )
      : base( virtualPath )
    { }




    /// <summary>
    /// 处理部分视图
    /// </summary>
    protected override void ProcessContainer()
    {
      return;
    }
  }



}
