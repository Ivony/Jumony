using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{


  /// <summary>
  /// HTML 筛选器提供程序
  /// </summary>
  public interface IHtmlFilterProvider
  {


    /// <summary>
    /// 获取当前可用的筛选器列表
    /// </summary>
    /// <returns></returns>
    IHtmlFilter[] GetFilters();

  }
}
