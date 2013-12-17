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

    IHtmlFilter[] GetFilters();

  }
}
