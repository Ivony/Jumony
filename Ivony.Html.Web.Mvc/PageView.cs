using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web.Routing;
using System.Web;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Web.Caching;
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// HTML 页面视图
  /// </summary>
  public abstract class PageView : ViewBase
  {

    /// <summary>
    /// 创建一个页面视图实例
    /// </summary>
    protected PageView()
    {

    }



    /// <summary>
    /// 获取或设置 HTML 视图的虚拟路径
    /// </summary>
    public string VirtualPath
    {
      get;
      private set;
    }


    /// <summary>
    /// 是否应当将页面当作部分视图来处理
    /// </summary>
    protected bool PartialMode
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取页面文档对象
    /// </summary>
    public IHtmlDocument Document
    {
      get;
      private set;
    }


    /// <summary>
    /// 派生类重写此方法自定义文档处理逻辑
    /// </summary>
    protected abstract void ProcessDocument();



    protected override void Process( IHtmlContainer scope )
    {
      Document = scope.Document;
      ProcessDocument();
    }
  }



  internal class GenericPageView : PageView
  {

    public GenericPageView( string virtualPath )
      : base( virtualPath )
    {

    }


    protected override void ProcessDocument()
    {
      return;
    }
  }
}
