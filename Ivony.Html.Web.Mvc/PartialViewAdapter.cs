using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ivony.Fluent;

namespace Ivony.Html.Web
{
  /// <summary>
  /// 用于渲染部分视图的 HTML 渲染代理
  /// </summary>
  public class PartialRenderAdapter : HtmlElementAdapter
  {


    /// <summary>
    /// 创建 HtmlHelper 对象
    /// </summary>
    /// <returns>创建的 HtmlHelper 对象</returns>
    protected HtmlHelper MakeHelper()
    {

      var helper = new HtmlHelper( ViewContext, ViewHandler );
      return helper;
    }


    /// <summary>
    /// 创建 PartialRenderAdapter 实例
    /// </summary>
    /// <param name="view">需要渲染部分视图的宿主视图</param>
    public PartialRenderAdapter( ViewContext viewContext, JumonyUrlHelper urlHelper, IViewHandler viewHandler )
    {
      if ( viewContext == null )
        throw new ArgumentNullException( "viewContext" );

      if ( urlHelper == null )
        throw new ArgumentNullException( "urlHelper" );

      if ( viewHandler == null )
        throw new ArgumentNullException( "viewHandler" );

      ViewContext = viewContext;
      ViewHandler = viewHandler;
      Url = urlHelper;


      var wrapper = viewHandler as IHandlerWrapper;
      if ( wrapper != null )
        _partialExecutors = GetPartialExecutors( wrapper.Handler.GetType() );
      else
        _partialExecutors = GetPartialExecutors( viewHandler.GetType() );
    }



    /// <summary>
    /// 处理 Partial 的方法名称前缀
    /// </summary>
    public const string partialExecutorMethodPrefix = "Partial_";


    private static readonly KeyedCache<Type, PartialViewExecutor[]> _cache = new KeyedCache<Type, PartialViewExecutor[]>();


    private PartialViewExecutor[] _partialExecutors;

    private PartialViewExecutor[] GetPartialExecutors( Type type )
    {

      return _cache.FetchOrCreateItem( type, () => type
        .GetMethods( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
        .Where( m => m.Name.StartsWith( partialExecutorMethodPrefix ) )
        .Where( m => m.ReturnType == typeof( string ) )
        .Select( m => CreateExecutor( m ) ).ToArray() );

    }

    private PartialViewExecutor CreateExecutor( MethodInfo method )
    {
      if ( method.Name.StartsWith( partialExecutorMethodPrefix ) )
        return new PartialViewExecutor( method );
      else
        return null;
    }



    /// <summary>
    /// 获取当前视图上下文
    /// </summary>
    protected ViewContext ViewContext
    {
      get;
      private set;
    }


    /// <summary>
    /// 当前 Url 帮助器
    /// </summary>
    protected JumonyUrlHelper Url
    {
      get;
      private set;
    }

    /// <summary>
    /// 获取当前视图处理程序
    /// </summary>
    protected IViewHandler ViewHandler
    {
      get;
      private set;
    }


    /// <summary>
    /// 重写 CssSelector 属性，用于选取 partial 标签
    /// </summary>
    protected override string CssSelector
    {
      get { return "partial"; }
    }


    /// <summary>
    /// 渲染 partial 标签
    /// </summary>
    /// <param name="element">partial 标签</param>
    /// <param name="context">渲染上下文</param>
    protected override void Render( IHtmlElement element, HtmlRenderContext context )
    {

      var partialTag = ContentExtensions.GenerateTagHtml( element, true );

      ViewContext.HttpContext.Trace.Write( "Jumony View Engine", string.Format( "Begin Render Partial: {0}", partialTag ) );
      RenderPartial( element, context.Writer );
      ViewContext.HttpContext.Trace.Write( "Jumony View Engine", string.Format( "End Render Partial: {0}", partialTag ) );
    }


    /// <summary>
    /// 渲染部分视图（重写此方法接管 partial 处理逻辑）。
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <param name="writer">输出渲染结果的 TextWriter 对象</param>
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
    /// <returns>渲染结果</returns>
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
      var name = partialElement.Attribute( "name" ).Value();

      if ( !_partialExecutors.IsNullOrEmpty() && name != null )
      {
        var executor = _partialExecutors.FirstOrDefault( e => e.Name.EqualsIgnoreCase( name ) );
        if ( executor != null )
        {
          var wrapper = ViewHandler as IHandlerWrapper;
          if ( wrapper != null )
            return executor.Execute( wrapper.Handler, partialElement );
          else
            return executor.Execute( ViewHandler, partialElement );
        }
      }


      var helper = MakeHelper();


      try
      {
        if ( action != null )//Action 部分视图
        {
          var routeValues = Url.GetRouteValues( partialElement );

          return RenderAction( partialElement, action, partialElement.Attribute( "controller" ).Value(), routeValues );
        }

        else if ( view != null )
        {
          return RenderPartial( partialElement, view );
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

    private string RenderAction( IHtmlElement partialElement, string actionName, string controllerName, System.Web.Routing.RouteValueDictionary routeValues )
    {
      return MakeHelper().Action( actionName, controllerName, routeValues ).ToString();
    }

    private string RenderPartial( IHtmlElement partialElement, string viewName )
    {
      return MakeHelper().Partial( viewName ).ToString();
    }
  }
}
