using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  public static class ViewExtensions
  {

    /// <summary>
    /// 将视图结果交给指定处理器进行筛选
    /// </summary>
    /// <param name="viewResult">视图结果</param>
    /// <param name="handler">HTML 处理器</param>
    /// <returns>包装后的视图结果</returns>
    public static ActionResult Handle( this ViewResultBase viewResult, IHtmlHandler handler )
    {
      return new ViewResultWrapper( viewResult, handler );
    }

  }
}
