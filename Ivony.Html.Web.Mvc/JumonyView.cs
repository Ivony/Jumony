using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;


namespace Ivony.Html.Web
{
  /// <summary>
  /// Jumony 视图
  /// </summary>
  public abstract class JumonyView : ViewBase, IContentView
  {

    internal const string ViewFiltersDataKey = "Jumony_ViewBase_ViewFilters";


    /// <summary>
    /// 创建 JumonyView 对象
    /// </summary>
    protected JumonyView()
    {
      RenderAdapters = new List<IHtmlRenderAdapter>() { new PartialRenderAdapter( this ) };
    }


    /// <summary>
    /// 母板视图
    /// </summary>
    protected IMasterView MasterView
    {
      get;
      private set;
    }


    /// <summary>
    /// 处理和渲染指定 HTML 范畴
    /// </summary>
    /// <param name="scope">要处理和渲染的范畴</param>
    /// <returns></returns>
    protected override string RenderCore( IHtmlContainer scope )
    {

      //初始化视图筛选器
      Filters = InitializeFilters();

      RenderAdapters.Add( new ViewElementAdapter( ViewContext, Url ) );


      HttpContext.Trace.Write( "JumonyView", "Begin Process" );
      OnPreProcess();
      ProcessScope( Scope );
      OnPostProcess();
      HttpContext.Trace.Write( "JumonyView", "End Process" );

      HttpContext.Trace.Write( "JumonyView", "Begin ProcessActionRoutes" );
      Url.ProcessActionUrls( Scope );
      HttpContext.Trace.Write( "JumonyView", "End ProcessActionRoutes" );


      HttpContext.Trace.Write( "JumonyView", "Begin ResolveUri" );
      Scope.Find( "form[postback]" )
        .SetAttribute( "action", RawViewContext.HttpContext.Request.RawUrl )
        .SetAttribute( "method", "post" )
        .RemoveAttribute( "postback" );

      Url.ResolveUri( Scope, VirtualPath );
      HttpContext.Trace.Write( "JumonyView", "End ResolveUri" );

      AddGeneratorMetaData();


      if ( MasterView != null )
      {
        HttpContext.Trace.Write( "JumonyView", "Begin Initialize Master" );
        MasterView.Initialize( ViewContext );
        HttpContext.Trace.Write( "JumonyView", "End Initialize Master" );

        HttpContext.Trace.Write( "JumonyView", "Begin Render" );
        OnPreRender();
        var content = MasterView.Render( this );
        OnPostRender();
        HttpContext.Trace.Write( "JumonyView", "End Render" );

        return content;
      }
      else
      {
        HttpContext.Trace.Write( "JumonyView", "Begin Render" );
        OnPreRender();
        string content = RenderContent( RenderAdapters.ToArray() );
        OnPostRender();
        HttpContext.Trace.Write( "JumonyView", "End Render" );

        return content;
      }
    }

    /// <summary>
    /// 初始化筛选器，获取当前视图所需要应用的筛选器。
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<IViewFilter> InitializeFilters()
    {
      var filters = ViewData[ViewFiltersDataKey] as IEnumerable<IViewFilter> ?? Enumerable.Empty<IViewFilter>();
      ViewData[ViewFiltersDataKey] = filters.OfType<IChildViewFilter>();//重设 Filters 使其只剩下可用于子视图的筛选器。

      filters = ViewFilterProvider.GetViewFilters( VirtualPath ).Concat( filters ).ToArray();

      return filters;
    }


    /// <summary>
    /// 添加 &lt;meta name="generator" value="Jumony" /&gt; 标记
    /// </summary>
    private void AddGeneratorMetaData()
    {

      if ( MvcEnvironment.Configuration.DisableGeneratorTag || PartialMode || MasterView != null )
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
          filter.OnPreProcess( ViewContext, this );
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
    protected virtual void ProcessScope( IHtmlContainer container )
    {

    }


    /// <summary>
    /// 派生类实现此方法处理母板视图
    /// </summary>
    /// <param name="MasterView">页面的母板视图</param>
    protected virtual void ProcessMaster( JumonyMasterView MasterView )
    {
    }



    /// <summary>
    /// 渲染 HTML 内容
    /// </summary>
    /// <returns>渲染结果</returns>
    protected virtual string RenderContent( IHtmlRenderAdapter[] adapters )
    {
      return RenderContent( Scope, PartialMode, adapters );
    }

    /// <summary>
    /// 渲染 HTML 内容。
    /// </summary>
    /// <returns></returns>
    protected virtual string RenderContent( IHtmlContainer scope, bool partialMode, IHtmlRenderAdapter[] adapters )
    {


      var document = scope as IHtmlDocument;
      if ( document == null )
      {
        var writer = new StringWriter();

        foreach ( var node in scope.Nodes() )
          node.Render( writer, adapters );

        return writer.ToString();
      }

      else
        return document.Render( adapters );

    }


    /// <summary>
    /// 自定义渲染过程的 HTML 转换器
    /// </summary>
    internal protected virtual IList<IHtmlRenderAdapter> RenderAdapters
    {
      get;
      private set;
    }


    void IContentView.InitializeMaster( IMasterView master )
    {
      if ( MasterView != null )
        throw new InvalidOperationException( "不能重复初始化母板" );

      MasterView = master;
    }

    IHtmlRenderAdapter IContentView.CreateContentAdapter( IMasterView master )
    {
      return new ContentAdapter( this );
    }

  }
}
