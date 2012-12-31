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








    /// <summary>
    /// 渲染部分视图（重写此方法接管 partial 处理逻辑）。
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <param name="writer"></param>
    protected virtual void RenderPartial( IHtmlElement partialElement, TextWriter writer )
    {


      var timeout = MvcEnvironment.Configuration.PartialRenderTimeout;

      if ( timeout > TimeSpan.Zero )
      {

        string result = null;
        Exception exception = null;

        var thread = new Thread( () => result = RenderPartialAsync( partialElement, out exception ) );

        thread.Start();
        if ( thread.Join( timeout ) )
        {
          if ( exception != null )
            throw new HttpException( "渲染 Partial 时发生错误，详见内部异常", exception );

          writer.Write( result );
        }

        else
        {
          thread.Abort();
          writer.Write( "<!--Render partial timeout-->" );
        }
      }

      else
        writer.Write( RenderPartial( partialElement ) );
    }


    /// <summary>
    /// 异步渲染部分视图
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <param name="exception">渲染过程中产生的异常</param>
    /// <returns></returns>
    [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]//捕获任何异常
    protected string RenderPartialAsync( IHtmlElement partialElement, out Exception exception )
    {
      try
      {
        exception = null;
        return RenderPartial( partialElement );
      }
      catch ( ThreadAbortException )
      {
        exception = null;
        return null;
      }
      catch ( Exception e )
      {
        exception = e;
        return null;
      }
    }



    /// <summary>
    /// 渲染部分视图（重写此方法以实现自定义输出 partial 元素）
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <returns></returns>
    protected virtual string RenderPartial( IHtmlElement partialElement )
    {
      var action = partialElement.Attribute( "action" ).Value();
      var view = partialElement.Attribute( "view" ).Value();
      var path = partialElement.Attribute( "path" ).Value();
      var handler = partialElement.Attribute( "handler" ).Value();


      var helper = MakeHelper();


      try
      {
        if ( action != null )//Action 部分视图
        {
          var controller = partialElement.Attribute( "controller" ).Value() ?? (string) RouteData.Values["controller"];
          var routeValues = Url.GetRouteValues( partialElement );

          return helper.Action( actionName: action, controllerName: controller, routeValues: routeValues ).ToString();
        }

        else if ( view != null )
        {
          return RenderPartialView( helper, view, partialElement );
        }

        else if ( path != null )
        {
          if ( !VirtualPathUtility.IsAppRelative( path ) )
            throw new FormatException( "path 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取" );

          var content = HtmlProviders.LoadContent( path );
          if ( content != null )
            return content.Content;
        }

      }
      catch ( ThreadAbortException )
      {

      }
      catch //若渲染时发生错误
      {
        if ( MvcEnvironment.Configuration.IgnorePartialRenderException || partialElement.Attribute( "ignoreError" ) != null )
          return "<!--parital view render failed-->";
        else
          throw;
      }

      throw new NotSupportedException( "无法处理的partial标签：" + ContentExtensions.GenerateTagHtml( partialElement, false ) );

    }

    /// <summary>
    /// 渲染部分视图（重写此方法为指定名称的部分视图提供模型和数据）
    /// </summary>
    /// <param name="helper">HTML Helper</param>
    /// <param name="partialViewName">部分视图名称</param>
    /// <param name="partialElement">partial 元素</param>
    /// <returns>渲染好的部分视图</returns>
    protected virtual string RenderPartialView( HtmlHelper helper, string partialViewName, IHtmlElement partialElement )
    {
      return helper.Partial( partialViewName ).ToString();
    }





    /// <summary>
    /// 创建 HtmlHelper 对象
    /// </summary>
    /// <returns></returns>
    protected HtmlHelper MakeHelper()
    {

      var helper = new HtmlHelper( ViewContext, new ViewDataContainer( this ) );
      return helper;
    }

    /// <summary>
    /// 用于渲染部分视图的 HTML 渲染代理
    /// </summary>
    public sealed class PartialRenderAdapter : HtmlElementAdapter
    {

      private JumonyView _view;

      /// <summary>
      /// 创建 PartialRenderAdapter 实例
      /// </summary>
      /// <param name="view"></param>
      public PartialRenderAdapter( JumonyView view )
      {
        _view = view;
      }


      /// <summary>
      /// 一个 CSS 选择器，用于选取 partial 标签
      /// </summary>
      protected override string CssSelector
      {
        get { return "partial"; }
      }


      /// <summary>
      /// 渲染 partial 标签
      /// </summary>
      /// <param name="element">partial 标签</param>
      /// <param name="writer">用于渲染的文本编写器</param>
      public override void Render( IHtmlElement element, TextWriter writer )
      {

        var partialTag = ContentExtensions.GenerateTagHtml( element, true );

        _view.HttpContext.Trace.Write( "Jumony View Engine", string.Format( "Begin Render Partial: {0}", partialTag ) );
        _view.RenderPartial( element, writer );
        _view.HttpContext.Trace.Write( "Jumony View Engine", string.Format( "End Render Partial: {0}", partialTag ) );
      }
    }

  }
}
