using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
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


    /// <summary>
    /// 指示视图引擎查找默认母板时是否应上溯查找，默认是false，即只在本文件夹查找，不上溯到父级文件夹。
    /// </summary>
    public bool FallbackDefaultMaster
    {
      get;
      set;
    }


    /// <summary>
    /// 指示在 ASP.NET MVC 应用中，如果找不到合适的缓存策略，是否应当回溯查找传统的缓存策略提供程序。
    /// </summary>
    public bool FallbackCachePolicy
    {
      get;
      set;
    }


    /// <summary>
    /// 指示是否禁用下划线开头的路由值设置，如_id将不被识别为名为id的路由值，而必须使用route-id。
    /// </summary>
    public bool DisableUnderscorePrefix
    {
      get;
      set;
    }


  }
}
