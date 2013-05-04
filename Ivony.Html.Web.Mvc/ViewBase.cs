using Ivony.Web;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 所有 HTML 视图处理程序的基类，实现 IView 接口，并提供内置的 HTML 扩展功能以及视图结果的缓存。
  /// </summary>
  public abstract class ViewBase : IView, ICacheableResult
  {

    /// <summary>
    /// 获取视图上下文
    /// </summary>
    public ViewContext ViewContext
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取当前 HTTP 上下文
    /// </summary>
    protected HttpContextBase HttpContext
    {
      get { return ViewContext.HttpContext; }
    }



    /// <summary>
    /// 获取原始的（顶层的）视图上下文
    /// </summary>
    protected ViewContext RawViewContext
    {
      get;
      private set;
    }


    private bool _initialized = false;


    /// <summary>
    /// 初始化视图设置
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <param name="partialMode">是否为部分视图模式</param>
    internal virtual void Initialize( string virtualPath, bool partialMode )
    {
      if ( _initialized )
        throw new InvalidOperationException( "视图已经初始化" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw new FormatException( "VirtualPath 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取" );

      VirtualPath = virtualPath;
      PartialMode = partialMode;

      _initialized = true;
    }

    /// <summary>
    /// 视图的虚拟路径
    /// </summary>
    public string VirtualPath
    {
      get;
      private set;
    }

    /// <summary>
    /// 是否为部分视图
    /// </summary>
    public bool PartialMode
    {
      get;
      private set;
    }



    /// <summary>
    /// 派生类可以重写此方法自定义加载虚拟路径处的文档的逻辑
    /// </summary>
    /// <returns></returns>
    protected virtual IHtmlDocument LoadDocument( string virtualPath )
    {
      return MvcEnvironment.LoadDocument( virtualPath );
    }


    /// <summary>
    /// 获取渲染和处理的范畴
    /// </summary>
    public IHtmlContainer Scope
    {
      get;
      private set;
    }

    /// <summary>
    /// 初始化视图渲染范畴
    /// </summary>
    /// <returns>渲染和处理的范畴，一般情况下是 IHtmlDocument</returns>
    protected virtual IHtmlContainer InitializeScope( string virtualPath, bool partialMode )
    {
      var document = LoadDocument( virtualPath );

      if ( partialMode )
        return GetPartialScope( document );

      else
        return document;

    }

    /// <summary>
    /// 查找部分视图渲染范畴
    /// </summary>
    /// <param name="document">加载的文档</param>
    /// <returns>渲染范畴</returns>
    protected virtual IHtmlContainer GetPartialScope( IHtmlDocument document )
    {
      var body = document.Find( "body" ).SingleOrDefault();

      if ( body == null )
        return document;

      else
        return body;
    }


    void IView.Render( ViewContext viewContext, TextWriter writer )
    {
      InitializeView( viewContext );

      RenderCore( writer );
    }


    /// <summary>
    /// 初始化视图，准备处理和渲染
    /// </summary>
    /// <param name="viewContext">视图上下文</param>
    protected virtual void InitializeView( ViewContext viewContext )
    {

      if ( !_initialized )
        throw new InvalidOperationException( "视图尚未初始化" );


      if ( ViewContext != null )
        throw new InvalidOperationException( "不能重复初始化视图上下文" );

      ViewContext = viewContext;

      while ( viewContext.IsChildAction )
      {
        viewContext = viewContext.ParentActionViewContext;//循环上溯最原始的视图上下文
      }

      RawViewContext = viewContext;


      HttpContext.Trace.Write( "ViewBase", "Begin InitializeScope" );
      Scope = InitializeScope( VirtualPath, PartialMode );
      HttpContext.Trace.Write( "ViewBase", "End InitializeScope" );

    }



    /// <summary>
    /// 处理和渲染视图
    /// </summary>
    /// <param name="writer">文本编写器</param>
    protected virtual void RenderCore( TextWriter writer )
    {
      var content = RenderCore( Scope );

      CachedResponse = UpdateCache( content );

      writer.Write( content );
    }


    /// <summary>
    /// 实现处理和渲染逻辑
    /// </summary>
    /// <param name="scope">要渲染的范畴</param>
    /// <returns>渲染后的字符串</returns>
    protected abstract string RenderCore( IHtmlContainer scope );



    /// <summary>
    /// 更新缓存
    /// </summary>
    /// <param name="content">渲染结果</param>
    protected virtual ICachedResponse UpdateCache( string content )
    {
      var response = new RawResponse();

      response.Content = content;
      response.Headers.Add( "ContentType", "text/html" );

      return response;
    }

    /// <summary>
    /// 缓存结果
    /// </summary>
    protected ICachedResponse CachedResponse
    {
      get;
      private set;
    }


    ICachedResponse ICacheableResult.GetCachedResponse()
    {
      return CachedResponse;
    }
  }
}
