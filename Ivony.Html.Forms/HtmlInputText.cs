using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义文本输入控件
  /// </summary>
  /// <remarks>
  /// 文本输入控件包括&lt;input type="text"&gt;和&lt;input type="password"&gt;
  /// </remarks>
  public sealed class HtmlInputText : FormTextControl
  {

    internal HtmlInputText( HtmlForm form, IHtmlElement element )
      : base( form, element )
    {
      var value = element.Attribute( "value" ).Value() ?? "";
    }



    /// <summary>
    /// 获取控件值
    /// </summary>
    /// <returns>控件目前设置的值</returns>
    protected override string GetValue()
    {
      return Element.Attribute( "value" ).Value() ?? "";
    }


    /// <summary>
    /// 设置控件值
    /// </summary>
    /// <param name="value">要设置的值</param>
    protected override void SetValue( string value )
    {
      value = value.Replace( "\r", "" ).Replace( "\n", "" );
      Element.SetAttribute( "value", value );
    }


    /// <summary>
    /// 确定能够设置指定的文本值
    /// </summary>
    /// <param name="value">要设置的控件值</param>
    /// <param name="message">若无法设置，获取错误信息</param>
    /// <returns>是否能够设置这个控件值</returns>
    protected override bool CanSetValue( string value, out string message )
    {
      if ( !base.CanSetValue( value, out message ) )
        return false;


      if ( !Form.Configuration.IgnoreNewlineInTextbox && (value.Contains( "\r" ) || value.Contains( "\n" )) )
      {
        message = "单行文本框不能输入多行文本值";
        return false;
      }

      else
        return true;
    }
  }
}
