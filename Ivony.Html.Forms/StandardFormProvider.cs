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

    public IFormControl[] DiscoveryControls( HtmlForm form )
    {

      var controls = DiscoveryControls( form, form.Element );

      var buttons = form.Element.Find( "input[type=radio], input[type=checkbox]" )
        .GroupBy( element => element.Attribute( "name" ).Value(), StringComparer.OrdinalIgnoreCase )
        .Select( group => new FormButtonGroup( form, group.Key, group.ToArray() ) )
        .ToArray();


      controls = controls.Concat( buttons );

      var dunplicate = controls.GroupBy( c => c.Name, StringComparer.OrdinalIgnoreCase ).Where( g => g.Count() > 1 ).FirstOrDefault();
      if ( dunplicate != null )
        throw new InvalidOperationException( string.Format( "表单中发现多个名为 \"{0}\" 的控件", dunplicate.Key ) );

      return controls.ToArray();
    }

    protected virtual IEnumerable<IFormControl> DiscoveryControls( HtmlForm form, IHtmlContainer container )
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
