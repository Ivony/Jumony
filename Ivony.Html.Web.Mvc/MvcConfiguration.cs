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


    /// <summary>
    /// 忽略部分视图渲染异常，继续渲染页面其它部分，默认为 false 。
    /// </summary>
    public bool IgnorePartialRenderException
    {
      get;
      set;
    }

    /// <summary>
    /// 获取或设置部分视图的渲染超时时间，设置为任何小于等于零的时间都等于没有渲染超时。默认为零
    /// </summary>
    public TimeSpan PartialRenderTimeout
    {
      get;
      set;
    }

  }
}
