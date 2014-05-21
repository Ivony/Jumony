using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using Ivony.Fluent;
using Ivony.Web;
using System.Diagnostics;

namespace Ivony.Html.Web
{
  /// <summary>
  /// 用于渲染部分视图的 HTML 渲染代理
  /// </summary>
  public class PartialRenderAdapter : HtmlElementAdapter
  {


    /// <summary>
    /// 创建 PartialRenderAdapter 实例
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <param name="handler">视图处理程序对象，用于获取自定义分部视图处理方法</param>
    public PartialRenderAdapter( HttpContextBase context, object handler )
    {
      HttpContext = context;

      var wrapper = handler as IHandlerWrapper;
      if ( wrapper != null )
        Handler = wrapper.Handler;
      else
        Handler = handler;


      PartialExecutors = GetPartialExecutors( Handler.GetType() ).ToDictionary( item => item.Name );
    }



    /// <summary>
    /// 获取当前 HTTP 请求上下文
    /// </summary>
    protected HttpContextBase HttpContext
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取当前 HTML 处理程序
    /// </summary>
    protected object Handler
    {
      get;
      private set;
    }


    /// <summary>
    /// 当前处理程序中定义的部分视图执行程序
    /// </summary>
    protected IDictionary<string, PartialExecutor> PartialExecutors
    {
      get;
      private set;
    }



    /// <summary>
    /// 处理 Partial 的方法名称前缀
    /// </summary>
    public const string partialExecutorMethodPrefix = "Partial_";


    private static readonly KeyedCache<Type, PartialExecutor[]> _cache = new KeyedCache<Type, PartialExecutor[]>();


    private PartialExecutor[] GetPartialExecutors( Type type )
    {

      return _cache.FetchOrCreateItem( type, () => type
        .GetMethods( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
        .Where( m => m.Name.StartsWith( partialExecutorMethodPrefix ) )
        .Where( m => m.ReturnType == typeof( string ) )
        .Select( m => CreateExecutor( m ) ).ToArray() );

    }

    private PartialExecutor CreateExecutor( MethodInfo method )
    {
      if ( method.Name.StartsWith( partialExecutorMethodPrefix ) )
        return new PartialExecutor( method );
      else
        return null;
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
    protected override void Render( IHtmlElement element, IHtmlRenderContext context )
    {

      var partialTag = ContentExtensions.GenerateTagHtml( element, true );

      Trace( string.Format( "Begin Render Partial: {0}", partialTag ) );
      RenderPartial( element, context.Writer );
      Trace( string.Format( "End Render Partial: {0}", partialTag ) );
    }



    /// <summary>
    /// 写入一条追踪信息
    /// </summary>
    /// <param name="message">追踪消息</param>
    protected virtual void Trace( string message )
    {
      WebServiceLocator.GetTraceService().Trace( TraceLevel.Info, "Jumony Partial", message );
    }


    /// <summary>
    /// 渲染部分视图（派生类可以重写此方法接管 partial 处理逻辑）。
    /// </summary>
    /// <param name="partialElement">partial 元素</param>
    /// <param name="writer">输出渲染结果的 TextWriter 对象</param>
    protected virtual void RenderPartial( IHtmlElement partialElement, TextWriter writer )
    {


      var timeout = JumonyWebConfiguration.Configuration.PartialRenderTimeout;

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
      var path = partialElement.Attribute( "path" ).Value();
      var name = partialElement.Attribute( "name" ).Value();

      try
      {

        if ( !PartialExecutors.IsNullOrEmpty() && name != null )
          return RenderNamedPartial( partialElement, name );


        else if ( path != null )
          return RenderVirtualPath( path );
      }
      catch //若渲染时发生错误
      {
        if ( JumonyWebConfiguration.Configuration.IgnorePartialRenderException || partialElement.Attribute( "ignoreError" ) != null )
          return "<!--parital render failed-->";
        else
          throw;
      }

      throw new NotSupportedException( "无法处理的partial标签：" + ContentExtensions.GenerateTagHtml( partialElement, false ) );

    }


    /// <summary>
    /// 渲染命名的部分视图
    /// </summary>
    /// <param name="partialElement">要渲染的　partial 元素</param>
    /// <param name="name">部分视图的名称</param>
    /// <returns>渲染结果</returns>
    protected virtual string RenderNamedPartial( IHtmlElement partialElement, string name )
    {
      PartialExecutor executor;

      if ( PartialExecutors.TryGetValue( name, out executor ) )
        return executor.Execute( Handler, partialElement );

      throw new HttpException( 404, "找不到部分视图处理程序" );
    }


    /// <summary>
    /// 渲染指定了路径的部分视图
    /// </summary>
    /// <param name="path">部分视图的路径</param>
    /// <returns>渲染结果</returns>
    protected virtual string RenderVirtualPath( string path )
    {
      if ( !VirtualPathUtility.IsAppRelative( path ) )
        throw VirtualPathHelper.VirtualPathFormatError( "path" );


      var result = JumonyPartialHandler.RenderPartial( HttpContext, path );

      if ( result == null )
        throw new HttpException( 404, "找不到部分视图" );

      else
        return result;
    }


  }
}
