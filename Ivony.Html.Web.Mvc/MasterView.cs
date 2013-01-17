using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 母板页视图
  /// </summary>
  public abstract class MasterView : JumonyView, IView
  {


    /// <summary>
    /// 初始化母板页视图
    /// </summary>
    /// <param name="virtualPath">母板页路径</param>
    internal void Initialize( string virtualPath )
    {
      base.Initialize( virtualPath, false );
    }


    protected IHtmlDocument Document
    {
      get;
      private set;
    }




    /// <summary>
    /// 合并页面头数据
    /// </summary>
    /// <param name="page">要合并的页面视图</param>
    internal protected virtual void MergeHeader( IHtmlDocument page )
    {

      var headElement = page.Find( "head" ).FirstOrDefault();
      if ( headElement == null )
        return;

      //UNDONE
    }



    /// <summary>
    /// 重写此方法以屏蔽直接渲染母板视图
    /// </summary>
    protected sealed override string RenderCore( IHtmlContainer scope )
    {
      throw new NotSupportedException( "母板页不能当作视图生成" );
    }

    /// <summary>
    /// 重写此方法以屏蔽直接渲染母板视图
    /// </summary>
    void IView.Render( ViewContext viewContext, System.IO.TextWriter writer )
    {
      throw new NotSupportedException( "母板页不能当作视图生成" );
    }


    internal void ProcessCore( ViewContext viewContext )
    {
      InitializeView( viewContext );

      Document = (IHtmlDocument) Scope;

      HttpContext.Trace.Write( "Jumony MasterView", "Begin Process" );
      Process( Scope );
      HttpContext.Trace.Write( "Jumony MasterView", "End Process" );


      HttpContext.Trace.Write( "Jumony MasterView", "Begin ProcessActionRoutes" );
      Url.ProcessActionUrls( Scope );
      HttpContext.Trace.Write( "Jumony MasterView", "End ProcessActionRoutes" );


      HttpContext.Trace.Write( "Jumony MasterView", "Begin ResolveUri" );
      Url.ResolveUri( Scope, VirtualPath );
      HttpContext.Trace.Write( "Jumony MasterView", "End ResolveUri" );

    }

    internal string RenderCore( IHtmlAdapter contentAdapter )
    {
      RenderAdapters.Add( contentAdapter );
      return RenderContent( RenderAdapters.ToArray() );
    }

  }


  /// <summary>
  /// 用页面视图渲染结果替换母板视图中 content 标签的渲染代理
  /// </summary>
  public class ContentRenderAdapter : HtmlElementAdapter
  {

    private string _content;
    /// <summary>
    /// 创建 ContentRenderAdapter 实例
    /// </summary>
    /// <param name="content">页面视图渲染结果</param>
    public ContentRenderAdapter( string content )
    {
      _content = content;
    }

    protected override string CssSelector { get { return "content"; } }

    public override void Render( IHtmlElement element, TextWriter writer )
    {
      writer.Write( _content );
    }
  }

}
