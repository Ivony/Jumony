using Ivony.Html.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;
using System.Reflection;
using Ivony.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 处理程序基类，协助实现 HTML 处理程序
  /// </summary>
  public class HtmlHandler : HtmlHandlerBase, IHtmlHandler, IHttpHandler
  {


    /// <summary>
    /// 定义用于确定回发表单的标识。
    /// </summary>
    protected static readonly string PostbackFormSign = "Jumony_Web_Postback";


    private static readonly MethodInfo GetMethod;
    private static readonly MethodInfo PostMethod;



    static HtmlHandler()
    {
      GetMethod = typeof( HtmlHandler ).GetMethod( "GetRequest", BindingFlags.Instance );
      PostMethod = typeof( HtmlHandler ).GetMethod( "PostRequest", BindingFlags.Instance );
    }



    /// <summary>
    /// 处理 HTML 范围
    /// </summary>
    /// <param name="context">当前 HTML 请求上下文</param>
    public void ProcessScope( HtmlRequestContext context )
    {
      Context = context;

      PreProcess();

      DataBind();


      if ( Request.HttpMethod.Equals( "GET" ) )
        ProcessGet();

      if ( Request.HttpMethod.Equals( "POST" ) )
        ProcessPost();


      PostProcess();
    }



    /// <summary>
    /// 进行数据绑定
    /// </summary>
    protected virtual void DataBind()
    {
      if ( DataModel != null )
        HtmlBinding.DataBind( HtmlScope, DataModel );
    }



    /// <summary>
    /// 获取或设置当前数据上下文
    /// </summary>
    protected object DataModel { get; set; }



    /// <summary>
    /// 派生类重写此方法对页面元素进行初始化。
    /// </summary>
    protected virtual void PreProcess()
    {

    }


    /// <summary>
    /// 派生类重写此方法对 GET 请求进行处理。
    /// </summary>
    protected virtual void ProcessGet()
    {

    }


    /// <summary>
    /// 派生类重写此方法对 POST 请求进行处理。
    /// </summary>
    protected virtual void ProcessPost()
    {
      throw new HttpException( 404, "该页面禁止 POST 方式访问" );
    }


    /// <summary>
    /// 派生类重写此方法，在页面处理的最后阶段进行处理。
    /// </summary>
    protected virtual void PostProcess()
    {

    }



    /// <summary>
    /// 要进行处理的 HTML 范围
    /// </summary>
    protected override IHtmlContainer HtmlScope
    {
      get { return Context.Scope; }
    }



    /// <summary>
    /// 当前 HTML 请求上下文
    /// </summary>
    protected HtmlRequestContext Context
    {
      get;
      private set;
    }


    /// <summary>
    /// 当前 HTTP 请求上下文
    /// </summary>
    protected override HttpContextBase HttpContext
    {
      get { return Context.HttpContext; }
    }


    /// <summary>
    /// 此方法用于实现销毁对象的一些操作。
    /// </summary>
    public virtual void Dispose()
    {
    }



    /// <summary>
    /// 此属性指示该处理程序是否可以重用，其始终返回 false。
    /// </summary>
    public bool IsReusable
    {
      get { return false; }
    }


    /// <summary>
    /// 实现此方法当使用此处理程序直接处理 HTTP 请求时，直接抛出异常。
    /// </summary>
    /// <param name="context">HTTP 请求上下文</param>
    public void ProcessRequest( HttpContext context )
    {
      throw JumonyHandler.DirectVisitError();
    }
  }
}
