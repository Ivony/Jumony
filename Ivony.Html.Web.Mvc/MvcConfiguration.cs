using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{
  public sealed class MvcConfiguration
  {

    internal MvcConfiguration() 
    {

      EnablePageViewRenderPartial = false;

    }


    /// <summary>
    /// 是否允许 PageView 渲染部分视图，默认为 false
    /// </summary>
    public bool EnablePageViewRenderPartial
    {
      get;
      set;
    }



  }
}
