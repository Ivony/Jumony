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
  public abstract class ViewHandler : HtmlHandlerBase, IHtmlViewHandler, IHttpHandler
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
    /// 处理指定文档范畴
    /// </summary>
    /// <param name="context">视图上下文</param>
    /// <param name="scope">要处理的范围</param>
    public void ProcessScope( ViewContext context, IHtmlContainer scope )
    {
      ViewContext = context;
      _scope = scope;
    }


    /// <summary>
    /// 派生类实现此方法处理文档
    /// </summary>
    protected abstract void ProcessDocument();



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
    /// 设置视图数据，此方法仅供框架调用
    /// </summary>
    /// <param name="viewData">视图数据</param>
    protected virtual void SetViewData( ViewDataDictionary viewData )
    {
      _viewData = viewData;
    }


    /// <summary>
    /// 派生类可以重写此方法完成销毁逻辑
    /// </summary>
    public virtual void Dispose()
    {
    }

  }


  /// <summary>
  /// 强类型 HTML 视图处理程序的基类
  /// </summary>
  /// <typeparam name="T">Model 的类型</typeparam>
  public abstract class ViewHandler<TModel> : ViewHandler
  {

    private ViewDataDictionary<TModel> _viewData;

    public new ViewDataDictionary<TModel> ViewData
    {
      get
      {
        if ( _viewData == null )
          SetViewData( new ViewDataDictionary<TModel>() );

        return _viewData;
      }

      set { SetViewData( value ); }
    }




  }

}
