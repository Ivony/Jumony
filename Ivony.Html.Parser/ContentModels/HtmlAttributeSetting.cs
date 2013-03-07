using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{

  /// <summary>
  /// 描述一个 HTML 标签属性
  /// </summary>
  public sealed class HtmlAttributeSetting : HtmlContentFragment
  {

    /// <summary>
    /// 创建 HtmlAttributeSetting 对象
    /// </summary>
    /// <param name="info">应当被认为是 HTML 标签属性的 HTML 片段</param>
    /// <param name="name">属性名</param>
    /// <param name="value">属性值</param>
    public HtmlAttributeSetting( HtmlContentFragment info, string name, string value )
      : base( info )
    {
      Name = name;
      Value = value;
    }

    /// <summary>
    /// 属性名
    /// </summary>
    public string Name
    {
      get;
      private set;
    }

    /// <summary>
    /// 属性值
    /// </summary>
    public string Value
    {
      get;
      private set;
    }

  }
}
