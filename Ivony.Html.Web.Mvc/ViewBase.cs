using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using System.Web.Caching;
using System.IO;
using System.Web.Mvc.Html;
using Ivony.Fluent;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Html.Web.Mvc
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
    /// 获取视图模型
    /// </summary>
    protected object ViewModel
    {
      get { return ViewContext.ViewData.Model; }
    }

    /// <summary>
    /// 获取视图数据
    /// </summary>
    protected ViewDataDictionary ViewData
    {
      get { return ViewContext.ViewData; }
    }

    /// <summary>
    /// 获取当前 HTTP 上下文
    /// </summary>
    protected HttpContextBase HttpContext
    {
      get { return ViewContext.HttpContext; }
    }

    /// <summary>
    /// 获取当前 HTTP 响应的追踪上下文对象
    /// </summary>
    protected TraceContext Trace
    {
      get { return HttpContext.Trace; }
    }



    /// <summary>
    /// 获取请求上下文
    /// </summary>
    protected RequestContext RequestContext
    {
      get { return ViewContext.RequestContext; }
    }

    /// <summary>
    /// 获取路由信息
    /// </summary>
    protected RouteData RouteData
    {
      get { return ViewContext.RouteData; }
    }

    /// <summary>
    /// 获取 TempData
    /// </summary>
    protected TempDataDictionary TempData
    {
      get { return ViewContext.TempData; }
    }

    /// <summary>
    /// 获取缓存提供对象
    /// </summary>
    protected Cache Cache
    {
      get { return HttpContext.Cache; }
    }

    /// <summary>
    /// 获取 Url 帮助器
    /// </summary>
    public JumonyUrlHelper Url
    {
      get;
      private set;
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
    protected void Initialize( string virtualPath, bool partialMode )
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
        return FindPartialScope( document );

      else
        return document;

    }

    /// <summary>
    /// 查找部分视图渲染范畴
    /// </summary>
    /// <param name="document">加载的文档</param>
    /// <returns>渲染范畴</returns>
    protected virtual IHtmlContainer FindPartialScope( IHtmlDocument document )
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
    protected void InitializeView( ViewContext viewContext )
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


      Url = new JumonyUrlHelper( this );

      HttpContext.Trace.Write( "Jumony View", "Begin InitializeScope" );
      Scope = InitializeScope( VirtualPath, PartialMode );
      HttpContext.Trace.Write( "Jumony View", "End InitializeScope" );

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



    /// <summary>
    /// 视图引擎调用此方法清理视图所使用的所有非托管资源
    /// </summary>
    public virtual void Dispose()
    {
    }


    /// <summary>
    /// 创建 IViewDataContainer 实例
    /// </summary>
    /// <returns>包含当前视图的 IViewDataContainer 实例</returns>
    internal ViewDataContainer CreateViewDataContainer()
    {
      return new ViewDataContainer( this );
    }


    /// <summary>
    /// 为视图提供 ViewData 的容器类型
    /// </summary>
    public class ViewDataContainer : IViewDataContainer
    {

      private ViewBase _view;

      /// <summary>
      /// 创建 ViewDataContainer 对象
      /// </summary>
      /// <param name="view"></param>
      public ViewDataContainer( ViewBase view )
      {
        _view = view;
      }


      ViewDataDictionary IViewDataContainer.ViewData
      {
        get
        {
          return _view.ViewData;
        }
        set
        {
          throw new NotSupportedException();
        }
      }
    }

  }
}
