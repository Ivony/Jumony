using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 保存 Jumony for MVC 的一些配置信息
  /// </summary>
  public sealed class MvcConfiguration
  {

    internal MvcConfiguration() 
    {


    }


    /// <summary>
    /// 是否允许 PageView 渲染子请求，默认为 false
    /// </summary>
    public bool EnablePageViewRenderChildAction
    {
      get;
      set;
    }


    /// <summary>
    /// 忽略部分视图渲染异常，继续渲染页面其它部分，默认为 false
    /// </summary>
    public bool IgnorePartialRenderException
    {
      get;
      set;
    }


    /// <summary>
    /// 是否允许并行进行部分视图渲染，默认值为 false
    /// </summary>
    public bool EnableParallelPartialRender
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


    /// <summary>
    /// 指示是否禁止生成generator的meta标签，默认是 false 。
    /// </summary>
    public bool DisableGeneratorTag
    {
      get;
      set;
    }


  }
}
