using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{
  /// <summary>
  /// 标准表单控件提供程序，用于解析所有标准的 HTML 表单控件
  /// </summary>
  public class StandardFormControlProvider : IFormControlProvider
  {


    private ISelector inputTextSelector = CssParser.ParseSelector( "input[type=text], input[type=password], input[type=hidden]" );
    private ISelector textareaSelector = CssParser.ParseSelector( "textarea" );
    private ISelector selectControlSelector = CssParser.ParseSelector( "select" );

    public FormControl CreateControl( HtmlForm form, IHtmlElement element )
    {

      if ( element.Attribute( "name" ).Value().IsNullOrEmpty() )
        return null;

      if ( inputTextSelector.IsEligibleBuffered( element ) )
        return new HtmlInputText( form, element );

      if ( textareaSelector.IsEligibleBuffered( element ) )
        return new HtmlTextArea( form, element );

      if ( selectControlSelector.IsEligibleBuffered( element ) )
        return new HtmlSelect( form, element );


      return null;


    }
  }
}
