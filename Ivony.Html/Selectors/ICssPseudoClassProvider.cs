using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 自定义伪类提供程序，实现此接口为CSS选择器添加自定义伪类支持
  /// </summary>
  public interface ICssPseudoClassProvider
  {

    /// <summary>
    /// 创建伪类选择器
    /// </summary>
    /// <param name="name">伪类名</param>
    /// <param name="args">伪类参数表达式</param>
    /// <returns>伪类选择器</returns>
    ICssPseudoClassSelector CreateSelector( string name, string args );


  }
}
