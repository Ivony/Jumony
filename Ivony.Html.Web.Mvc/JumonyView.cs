using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;


namespace Ivony.Html.Web.Mvc
{
  public abstract class JumonyView : ViewBase
  {

    internal const string ViewFiltersDataKey = "Jumony_ViewBase_ViewFilters";


    protected JumonyView()
    {
      RenderAdapters = new List<IHtmlAdapter>() { new PartialRenderAdapter( this ) };
    }




    /// <summary>
    /// 处理和渲染指定 HTML 范畴
    /// </summary>
    /// <param name="scope">要处理和渲染的范畴</param>
    /// <returns></returns>
    protected override string RenderCore( IHtmlContainer scope )
    {

      //获取视图筛选器
      Filters = ViewData[ViewFiltersDataKey] as IEnumerable<IViewFilter> ?? Enumerable.Empty<IViewFilter>();
      ViewData.Remove( ViewFiltersDataKey );

      RenderAdapters.Add( new ViewElementAdapter( ViewContext ) );


      HttpContext.Trace.Write( "Jumony View", "Begin Process" );
      OnPreProcess();
      Process( Scope );
      OnPostProcess();
      HttpContext.Trace.Write( "Jumony View", "End Process" );


      HttpContext.Trace.Write( "Jumony View", "Begin ProcessActionRoutes" );
      Url.ProcessActionUrls( Scope );
      HttpContext.Trace.Write( "Jumony View", "End ProcessActionRoutes" );


      HttpContext.Trace.Write( "Jumony View", "Begin ResolveUri" );
      Url.ResolveUri( Scope, VirtualPath );
      HttpContext.Trace.Write( "Jumony View", "End ResolveUri" );

      AddGeneratorMetaData();

      HttpContext.Trace.Write( "Jumony View", "Begin Render" );
      OnPreRender();
      var content = RenderContent( Scope, PartialMode );
      OnPostRender();
      HttpContext.Trace.Write( "Jumony View", "End Render" );

      return content;
    }



    private void AddGeneratorMetaData()
    {

      if ( MvcEnvironment.Configuration.DisableGeneratorTag || PartialMode )
        return;

      var document = Scope as IHtmlDocument;
      if ( document == null )
        return;

      var modifier = document.DomModifier;
      if ( modifier == null )
        return;


      var header = document.Find( "head" ).FirstOrDefault();

      if ( header == null )
        return;


      var metaElement = modifier.AddElement( header, "meta" );

      metaElement.SetAttribute( "name", "generator" );
      metaElement.SetAttribute( "content", "Jumony" );
    }


    /// <summary>
    /// 获取所有视图筛选器
    /// </summary>
    protected IEnumerable<IViewFilter> Filters
    {
      get;
      private set;
    }


    #region Events

    /// <summary>
    /// 初识化结束后，进行任何处理前引发此事件
    /// </summary>
    public event EventHandler PreProcess;

    /// <summary>
    /// 引发 PreProcess 事件
    /// </summary>
    protected virtual void OnPreProcess()
    {

      foreach ( var filter in Filters )
      {
        try
        {
          filter.OnPreRender( ViewContext, this );
        }
        catch { }
      }

      if ( PreProcess != null )
        PreProcess( this, EventArgs.Empty );
    }


    /// <summary>
    /// 对文档的所有处理完成后引发此事件
    /// </summary>
    public event EventHandler PostProcess;

    /// <summary>
    /// 引发 PostProcess 事件
    /// </summary>
    protected virtual void OnPostProcess()
    {
      foreach ( var filter in Filters.Reverse() )
      {
        try
        {
          filter.OnPostProcess( ViewContext, this );
        }
        catch { }
      }

      if ( PostProcess != null )
        PostProcess( this, EventArgs.Empty );
    }


    /// <summary>
    /// 完成所有渲染准备工作后，渲染文档之前引发此事件。
    /// </summary>
    public event EventHandler PreRender;

    /// <summary>
    /// 引发 PreRender 事件
    /// </summary>
    /// <param name="writer">用于输出渲染结果的编写器</param>
    protected virtual void OnPreRender()
    {
      foreach ( var filter in Filters )
      {
        try
        {
          filter.OnPreRender( ViewContext, this );
        }
        catch { }
      }

      if ( PreRender != null )
        PreRender( this, EventArgs.Empty );
    }


    /// <summary>
    /// 文档渲染完毕后引发此事件
    /// </summary>
    public event EventHandler PostRender;

    /// <summary>
    /// 引发 PostRender 事件
    /// </summary>
    /// <param name="writer">用于输出渲染结果的编写器</param>
    protected virtual void OnPostRender()
    {
      foreach ( var filter in Filters.Reverse() )
      {
        try
        {
          filter.OnPostRender( ViewContext, this );
        }
        catch { }
      }

      if ( PostRender != null )
        PostRender( this, EventArgs.Empty );
    }

    #endregion


    /// <summary>
    /// 派生类实现此方法完成对视图的处理工作
    /// </summary>
    protected abstract void Process( IHtmlContainer container );



    /// <summary>
    /// 渲染 HTML 内容。
    /// </summary>
    /// <returns></returns>
    protected virtual string RenderContent( IHtmlContainer scope, bool partialMode )
    {


      var document = scope as IHtmlDocument;
      if ( document == null )
      {
        var writer = new StringWriter();

        foreach ( var node in scope.Nodes() )
          node.Render( writer, RenderAdapters.ToArray() );

        return writer.ToString();
      }

      else
        return document.Render( RenderAdapters.ToArray() );

    }


    /// <summary>
    /// 自定义渲染过程的 HTML 转换器
    /// </summary>
    protected virtual IList<IHtmlAdapter> RenderAdapters
    {
      get;
      private set;
    }





  }
}
