using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 定义一个样式设置
  /// </summary>
  public class CssStyleProperty
  {


    /// <summary>
    /// 创建 CssSetting 对象
    /// </summary>
    /// <param name="name">CSS 样式名</param>
    /// <param name="value">CSS 样式值</param>
    public CssStyleProperty( string name, string value ) : this( name, value, false ) { }


    /// <summary>
    /// 创建 CssSetting 对象
    /// </summary>
    /// <param name="name">CSS 样式名</param>
    /// <param name="value">CSS 样式值</param>
    /// <param name="important">是否覆盖其他样式设置</param>
    public CssStyleProperty( string name, string value, bool important )
    {
      Name = name;
      Value = value;
      Important = important;
    }

    /// <summary>
    /// 样式名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 设置的样式值
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// 是否覆盖其他的样式设置
    /// </summary>
    public bool Important { get; private set; }


    public override string ToString()
    {
      if ( Important )
        return string.Format( "{0}:{1}!important", Name, Value );
      else
        return string.Format( "{0}:{1}", Name, Value );
    }

  }
}
