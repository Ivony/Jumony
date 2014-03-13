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
  public class StandardFormProvider : IFormProvider
  {


    private ISelector inputTextSelector = CssParser.ParseSelector( "input[type=text][name], input[type=password][name], input[type=hidden][name]" );
    private ISelector textareaSelector = CssParser.ParseSelector( "textarea[name]" );
    private ISelector selectControlSelector = CssParser.ParseSelector( "select[name]" );

    public FormControl[] DiscoveryControls( HtmlForm form )
    {

      return DiscoveryControls( form, form.Element ).ToArray();


    }

    protected virtual IEnumerable<FormControl> DiscoveryControls( HtmlForm form, IHtmlContainer container )
    {

      foreach ( var element in container.Elements() )
      {
        if ( inputTextSelector.IsEligible( element ) )
          yield return new HtmlInputText( form, element );

        else if ( textareaSelector.IsEligible( element ) )
          yield return new HtmlTextArea( form, element );

        else if ( selectControlSelector.IsEligible( element ) )
          yield return new HtmlSelect( form, element );

        else
        {
          foreach ( var control in DiscoveryControls( form, element ) )
            yield return control;
        }
      }

    }




  }
}
