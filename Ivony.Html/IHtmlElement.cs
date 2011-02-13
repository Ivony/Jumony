using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ivony.Html
{

  /// <summary>
  /// 定义 HTML 元素
  /// </summary>
  public interface IHtmlElement :  IHtmlContainer
  {

    /// <summary>
    /// 元素名
    /// </summary>
    string Name
    {
      get;
    }


    /// <summary>
    /// 获取元素的所有属性
    /// </summary>
    /// <returns>元素的所有属性</returns>
    IEnumerable<IHtmlAttribute> Attributes();

    /// <summary>
    /// 添加一个属性
    /// </summary>
    /// <param name="attributeName">属性名</param>
    /// <returns>添加的属性</returns>
    IHtmlAttribute AddAttribute( string attributeName );
  }
}
