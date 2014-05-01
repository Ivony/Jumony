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


    /// <summary>
    /// 在表单中发现所有标准控件
    /// </summary>
    /// <param name="form">要从中发现空间的表单</param>
    /// <returns>发现的所有标准控件</returns>
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


    /// <summary>
    /// 发现容器中所有可能是控件的元素并包装成控件返回
    /// </summary>
    /// <param name="form">控件所属的表单</param>
    /// <param name="container">要搜索的容器</param>
    /// <returns>找到的控件</returns>
    protected virtual IEnumerable<IFormControl> DiscoveryControls( HtmlForm form, IHtmlContainer container )
    {

      //UNDONE 没有检查 name 是否包含特殊字符

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
