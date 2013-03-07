using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 母板页视图
  /// </summary>
  public abstract class JumonyMasterView : JumonyView, IMasterView, IView
  {


    /// <summary>
    /// 初始化母板页视图
    /// </summary>
    /// <param name="virtualPath">母板页路径</param>
    internal void Initialize( string virtualPath )
    {
      base.Initialize( virtualPath, false );
    }

    /// <summary>
    /// 母板页文档
    /// </summary>
    protected IHtmlDocument Document
    {
      get;
      private set;
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



    void IMasterView.Initialize( ViewContext context )
    {
      InitializeView( context );

      Document = (IHtmlDocument) Scope;

      HttpContext.Trace.Write( "JumonyMasterView", "Begin Process" );
      ProcessScope( Scope );
      HttpContext.Trace.Write( "JumonyMasterView", "End Process" );


      HttpContext.Trace.Write( "JumonyMasterView", "Begin ProcessActionRoutes" );
      Url.ProcessActionUrls( Scope );
      HttpContext.Trace.Write( "JumonyMasterView", "End ProcessActionRoutes" );


      HttpContext.Trace.Write( "JumonyMasterView", "Begin ResolveUri" );
      Url.ResolveUri( Scope, VirtualPath );
      HttpContext.Trace.Write( "JumonyMasterView", "End ResolveUri" );
    }


    string IMasterView.Render( IContentView view )
    {
      HttpContext.Trace.Write( "JumonyMasterView", "Begin Render" );
      RenderAdapters.Add( view.CreateContentAdapter( this ) );
      var content = Document.Render( RenderAdapters.ToArray() );
      HttpContext.Trace.Write( "JumonyMasterView", "End Render" );

      return content;

    }
  }
}
