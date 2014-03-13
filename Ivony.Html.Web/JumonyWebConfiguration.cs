using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{

  /// <summary>
  /// Jumony Web 的一些参数配置
  /// </summary>
  public class JumonyWebConfiguration
  {

    /// <summary>
    /// 设置部分视图渲染超时时间。
    /// </summary>
    public TimeSpan PartialRenderTimeout { get; set; }


    /// <summary>
    /// 指示是否应当忽略部分视图渲染时出现的异常
    /// </summary>
    public bool IgnorePartialRenderException { get; set; }




    private static JumonyWebConfiguration _configuration = new JumonyWebConfiguration();


    /// <summary>
    /// 获取当前的配置
    /// </summary>
    public static JumonyWebConfiguration Configuration
    {
      get { return _configuration; }
    }





  }
}
