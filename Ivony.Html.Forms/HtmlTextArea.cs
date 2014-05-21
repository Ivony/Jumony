using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义 &lt;textarea&gt; 控件
  /// </summary>
  public sealed class HtmlTextArea : FormTextControl
  {


    internal HtmlTextArea( HtmlForm form, IHtmlElement element ) : base( form, element ) { }


    /// <summary>
    /// 获取控件值
    /// </summary>
    /// <returns>控件目前设置的值</returns>
    protected override string GetValue()
    {
      return Element.InnerText();
    }

    /// <summary>
    /// 设置控件值
    /// </summary>
    /// <param name="value">要设置的控件的值</param>
    protected override void SetValue( string value )
    {
      Element.InnerText( value );
    }


  }
}
