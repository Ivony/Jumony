using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Css
{
  public class CssStyleSpecificationBase
  {

    /// <summary>
    /// 从缩写的设置中提取所有的设置
    /// </summary>
    /// <param name="shorthand">缩写的设置</param>
    /// <returns></returns>
    public abstract CssStyleProperty[] ExtractShorthandSetting( CssStyleProperty shorthand );

  }
}
