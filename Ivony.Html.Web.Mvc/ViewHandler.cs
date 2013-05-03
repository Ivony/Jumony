using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Web.Routing;
using Ivony.Fluent;

namespace Ivony.Html.Web
{
  /// <summary>
  /// HTML 视图处理程序基类
  /// </summary>
  public class ViewHandler : HtmlHandlerBase, IViewHandler, IHttpHandler
  {


    #region IHttpHandler 成员

    bool IHttpHandler.IsReusable
    {
      get { return false; }
    }

    void IHttpHandler.ProcessRequest( HttpContext context )
    {
      throw new HttpException( 404, "不能直接访问视图处理程序" );
    }

    #endregion



    /// <summary>
    /// 获取当前视图上下文
    /// </summary>
    protected ViewContext ViewContext
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取当前 HTTP 上下文
    /// </summary>
    protected override HttpContextBase HttpContext
    {
      get { return ViewContext.HttpContext; }
    }


    private IHtmlContainer _scope;

    /// <summary>
    /// 获取当前要处理的 HTML 范围
    /// </summary>
    public override IHtmlContainer Scope
    {
      get { return _scope; }
    }

    /// <summary>
    /// 有关视图的虚拟路径帮助器
    /// </summary>
    public JumonyUrlHelper Url
    {
      get;
      private set;
    }

    /// <summary>
    /// 获取当前文档的虚拟路径
    /// </summary>
    public sealed override string VirtualPath
    {
      get { return Url.VirtualPath; }
    }


    /// <summary>
    /// 获取与此请求关联且仅可用于一个请求的数据。
    /// </summary>
    protected TempDataDictionary TempData
    {
      get { return ViewContext.TempData; }
    }


    /// <summary>
    /// 获取 URL 路由信息
    /// </summary>
    protected RouteData RouteData
    {
      get { return ViewContext.RouteData; }
    }





    /// <summary>
    /// 处理指定文档范畴
    /// </summary>
    /// <param name="viewContext">视图上下文</param>
    /// <param name="scope">要处理的范围</param>
    /// <param name="urlHelper">适用于当前文档的虚拟路径帮助器</param>
    void IViewHandler.ProcessScope( ViewContext viewContext, IHtmlContainer scope, JumonyUrlHelper urlHelper )
    {
      ViewContext = viewContext;
      ViewData = viewContext.ViewData;
      _scope = scope;
      Url = urlHelper;

      ProcessScope();
    }


    /// <summary>
    /// 派生类实现此方法处理 HTMl 文档或文档范畴
    /// </summary>
    protected virtual void ProcessScope()
    {

    }



    private ViewDataDictionary _viewData;

    /// <summary>
    /// 获取或设置视图数据
    /// </summary>
    public virtual ViewDataDictionary ViewData
    {
      get
      {
        if ( _viewData == null )
          SetViewData( new ViewDataDictionary() );

        return _viewData;
      }
      set { SetViewData( value ); }
    }


    /// <summary>
    /// 获取模型
    /// </summary>
    protected object Model { get { return ViewData.Model; } }

    /// <summary>
    /// 设置视图数据，此方法仅供框架调用
    /// </summary>
    /// <param name="viewData">视图数据</param>
    protected virtual void SetViewData( ViewDataDictionary viewData )
    {
      _viewData = viewData;
    }
  }


  /// <summary>
  /// 强类型 HTML 视图处理程序的基类
  /// </summary>
  /// <typeparam name="TModel">Model 的类型</typeparam>
  public abstract class ViewHandler<TModel> : ViewHandler
  {

    private ViewDataDictionary<TModel> _viewData;


    /// <summary>
    /// 获取或设置视图数据
    /// </summary>
    public new ViewDataDictionary<TModel> ViewData
    {
      get
      {
        if ( _viewData == null )
          SetViewData( new ViewDataDictionary<TModel>() );

        return _viewData;
      }

      set { _viewData = value; }
    }

    /// <summary>
    /// 重写 SetViewData 方法使用强类型视图
    /// </summary>
    /// <param name="viewData"></param>
    protected sealed override void SetViewData( ViewDataDictionary viewData )
    {
      _viewData = new ViewDataDictionary<TModel>( viewData );
    }

    /// <summary>
    /// 获取模型
    /// </summary>
    protected new TModel Model { get { return ViewData.Model; } }
  }


}

