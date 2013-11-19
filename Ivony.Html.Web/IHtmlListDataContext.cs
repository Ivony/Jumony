using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 代表一个列表数据上下文
  /// </summary>
  public interface IHtmlListDataContext
  {

    object[] ListData
    {
      get;
      set;
    }


    string TargetSelector
    {
      get;
      set;
    }

  }
}
