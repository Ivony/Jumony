using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{

  /// <summary>
  /// 描述一个HTML标签属性
  /// </summary>
  public sealed class HtmlAttributeSetting : HtmlContentFragment
  {

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
