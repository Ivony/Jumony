using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ivony.Fluent;
using Ivony.Html.ExpandedAPI;


namespace Ivony.Html.Web
{
  /// <summary>
  /// Jumony 视图
  /// </summary>
  public class JumonyView : ViewBase, IContentView
  {

    internal const string ViewFiltersDataKey = "Jumony_ViewBase_ViewFilters";

    /// <summary>
    /// 母板视图
    /// </summary>
    protected IMasterView MasterView
    {
      get;
      private set;
    }


    /// <summary>
    /// 重写 InitializeView 方法，增加 Jumony 视图初始化步骤
    /// </summary>
    /// <param name="viewContext">视图上下文</param>
    protected override void InitializeView( ViewContext viewContext )
    {
      base.InitializeView( viewContext );

      InitailizeJumonyView( viewContext );
    }

    /// <summary>
    /// Jumony 视图初始化
    /// </summary>
    /// <param name="viewContext">视图上下文</param>
    protected virtual void InitailizeJumonyView( ViewContext viewContext )
    {
      Url = new JumonyUrlHelper( viewContext.RequestContext, VirtualPath );
      Filters = GetFilters( viewContext );
    }



    /// <summary>
    /// 获取当前视图所需要应用的筛选器。
    /// </summary>
    /// <returns></returns>
    protected virtual IViewFilter[] GetFilters( ViewContext context )
    {
      var filters = context.ViewData[ViewFiltersDataKey] as IEnumerable<IViewFilter> ?? Enumerable.Empty<IViewFilter>();

      return ViewFilterProvider.GetViewFilters( VirtualPath ).Concat( filters ).ToArray();
    }



    private static readonly string handlerPathCachePrefix = "JumonyHandlerCache_";


    private class HandlerPathCacheItem
    {
      public string HandlerPath { get; set; }
    }


    /// <summary>
    /// 获取视图处理程序
    /// </summary>
    /// <returns>视图处理程序</returns>
    protected virtual IViewHandler GetHandler( string virtualPath )
    {

      var cacheKey = handlerPathCachePrefix + virtualPath;

      var cacheItem = HttpRuntime.Cache.Get( cacheKey );
      if ( cacheItem == null )
      {
        var handlerPath = ViewHandlerProvider.GetHandlerPath( Scope );

        cacheItem = new HandlerPathCacheItem() { HandlerPath = handlerPath };

        HttpRuntime.Cache.Insert( cacheKey, cacheItem, ScopeCacheDependency );

      }


      return ViewHandlerProvider.GetViewHandler( virtualPath );

    }




    /// <summary>
    /// 获取 HTML 渲染代理
    /// </summary>
    /// <remarks>
    /// 默认的渲染代理包含一个：
    /// 1. 部分视图渲染代理，处理 &lt;partial&gt; 标签
    /// </remarks>
    /// <returns>HTML 渲染代理列表</returns>
    public virtual IList<IHtmlRenderAdapter> GetRenderAdapters( IViewHandler handler )
    {

      var result = new List<IHtmlRenderAdapter>()
      {
        new PartialRenderAdapter( ViewContext, Url, handler ),
      };

      var customRenderAdapters = handler as ICustomRenderAdapters;
      if ( customRenderAdapters != null )
        result.AddRange( customRenderAdapters.GetCustomRenderAdapters() );

      return result;
    }


    /// <summary>
    /// 获取用于生成应用程序 URL 的帮助器
    /// </summary>
    public JumonyUrlHelper Url
    {
      get;
      private set;
    }



    /// <summary>
    /// 渲染代理列表
    /// </summary>
    internal IList<IHtmlRenderAdapter> RenderAdapters { get; set; }



    /// <summary>
    /// 处理和渲染指定 HTML 范畴
    /// </summary>
    /// <param name="scope">要处理和渲染的范畴</param>
    /// <returns>渲染结果</returns>
    protected override string RenderCore( IHtmlContainer scope )
    {

      HttpContext.Trace.Write( "JumonyView", "Begin GetViewHandler" );
      var handler = GetHandler( VirtualPath );
      HttpContext.Trace.Write( "JumonyView", "End GetViewHandler" );

      HttpContext.Trace.Write( "JumonyView", "Begin Process" );
      OnPreProcess();
      ProcessScope( handler );
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


      RenderAdapters = GetRenderAdapters( handler );

      string result;

      if ( MasterView != null )
      {
        HttpContext.Trace.Write( "JumonyView", "Begin Initialize Master" );
        MasterView.Initialize( ViewContext );
        HttpContext.Trace.Write( "JumonyView", "End Initialize Master" );


        var jumonyMaster = MasterView as JumonyMasterView;
        if ( jumonyMaster != null )
        {
          HttpContext.Trace.Write( "JumonyView", "Begin Process Master" );
          ProcessMaster( jumonyMaster );
          HttpContext.Trace.Write( "JumonyView", "Begin Process Master" );
        }

        HttpContext.Trace.Write( "JumonyView", "Begin Render" );
        OnPreRender();
        result = MasterView.Render( this );
        OnPostRender();
        HttpContext.Trace.Write( "JumonyView", "End Render" );
      }
      else
      {
        HttpContext.Trace.Write( "JumonyView", "Begin Render" );
        OnPreRender();
        result = RenderContent( RenderAdapters.ToArray() );
        OnPostRender();
        HttpContext.Trace.Write( "JumonyView", "End Render" );
      }


      var disposable = handler as IDisposable;
      if ( disposable != null )
        disposable.Dispose();

      return result;
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
    /// 初始化结束后，进行任何处理前引发此事件
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
    /// 处理 HTML 文档
    /// </summary>
    protected virtual void ProcessScope( IViewHandler handler )
    {
      handler.ProcessScope( CreateViewContext(), Scope, Url );
    }


    /// <summary>
    /// 创建一个视图上下文供视图处理器使用
    /// </summary>
    /// <returns>视图上下文</returns>
    private ViewContext CreateViewContext()
    {
      var viewData = new ViewDataDictionary( ViewContext.ViewData );
      viewData[ViewFiltersDataKey] = Filters.OfType<IChildViewFilter>().ToArray();

      return new ViewContext( ViewContext, this, viewData, ViewContext.TempData, ViewContext.Writer );
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
