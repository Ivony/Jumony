using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 部分视图处理程序基类
  /// </summary>
  [Obsolete( "此类型已过时，仅出于兼容目的而保留，请使用 JumonyViewHandler 类型" )]
  public abstract class PartialViewHandler : JumonyView, IHttpHandler
  {


    #region IHttpHandler 成员

    bool IHttpHandler.IsReusable
    {
      get { throw new NotImplementedException(); }
    }

    void IHttpHandler.ProcessRequest( HttpContext context )
    {
      throw new NotImplementedException();
    }

    #endregion


    /// <summary>
    /// 派生类重写此方法自定义文档处理逻辑
    /// </summary>
    protected abstract void ProcessContainer();


    /// <summary>
    /// 获取要渲染的部分
    /// </summary>
    protected IHtmlContainer Container
    {
      get;
      private set;
    }

    protected sealed override void ProcessScope()
    {
      Container = Scope;
      ProcessContainer();
    }


  }
}
