using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{
  public static class FormExtensions
  {

    /// <summary>
    /// 尝试将一个HTML元素转换为表单
    /// </summary>
    /// <param name="element">要转换为表单的元素</param>
    /// <returns></returns>
    public static HtmlForm AsForm( this IHtmlElement element )
    {
      return new HtmlForm( element );
    }



    /// <summary>
    /// 根据名称获取输入元素
    /// </summary>
    /// <param name="form">要查找元素的表单</param>
    /// <param name="name">元素名</param>
    /// <returns>输入元素</returns>
    public static IHtmlInputControl InputElement( this HtmlForm form, string name )
    {
      return form.InputControls.Where( control => control.Name == name ).SingleOrDefault();
    }

    /// <summary>
    /// 获取输入组当前所有选中的值
    /// </summary>
    /// <param name="group">输入组</param>
    /// <returns></returns>
    public static IEnumerable<string> CurrentValues( this IHtmlGroupControl group )
    {
      return group.Items.Where( item => item.Selected ).Select( item => item.Value );
    }

    /// <summary>
    /// 获取指定控件在客户端设置的值
    /// </summary>
    /// <param name="inputer">输入控件</param>
    /// <returns></returns>
    public static string SubmittedValue( this IHtmlInputControl inputer )
    {
      var data = inputer.Form.SubmittedValues;
      if ( data == null )
        return null;

      return data[inputer.Name];
    }


    /// <summary>
    /// 获取指定输入组在客户端设置的值
    /// </summary>
    /// <param name="group">输入组</param>
    /// <returns></returns>
    public static string[] SubmittedValues( this IHtmlGroupControl group )
    {
      var data = group.Form.SubmittedValues;
      if ( data == null )
        return null;

      if ( !data.AllKeys.Contains( group.Name ) )
        return new string[0];

      return data.GetValues( group.Name );
    }


    /// <summary>
    /// 将客户端提交来的值，应用到对应的输入控件上
    /// </summary>
    /// <param name="control">表单</param>
    public static void ApplySubmittedValue( this HtmlForm form )
    {
      form.InputControls.ForAll( control => ApplySubmittedValue( control ) );
    }


    /// <summary>
    /// 将客户端提交来的值，应用到对应的输入控件上
    /// </summary>
    /// <param name="control">输入控件</param>
    public static void ApplySubmittedValue( this IHtmlInputControl control )
    {
      var textControl = control as IHtmlTextControl;
      if ( textControl != null )
        ApplySubmittedValue( textControl );

      var groupControl = control as IHtmlGroupControl;
      if ( groupControl != null )
        ApplySubmittedValue( groupControl );
    }


    /// <summary>
    /// 将客户端提交来的值，应用到对应的输入控件上
    /// </summary>
    /// <param name="control">输入控件</param>
    public static void ApplySubmittedValue( this IHtmlTextControl control )
    {
      var value = SubmittedValue( control );
      if ( value == null )
        return;

      control.TextValue = value;
    }


    /// <summary>
    /// 将客户端提交来的值，应用到对应的输入控件上
    /// </summary>
    /// <param name="control">输入控件</param>
    public static void ApplySubmittedValue( this IHtmlGroupControl group )
    {

      var values = SubmittedValues( group );
      if ( values == null )
        return;

      ClearValues( group );

      if ( values.Any( v => group.Item( v ) == null ) )
        throw new InvalidOperationException();


      foreach ( var value in values )
      {
        if ( !TrySetValue( group, value ) )
          throw new InvalidOperationException();
      }
    }

    /// <summary>
    /// 清空输入组所有选中的值
    /// </summary>
    /// <param name="group">输入组</param>
    public static void ClearValues( this IHtmlGroupControl group )
    {
      group.Items.ForAll( item => item.Selected = false );
    }



    /// <summary>
    /// 获取输入组所有可能的值
    /// </summary>
    /// <param name="group">输入组</param>
    /// <returns></returns>
    public static IEnumerable<string> PossibleValues( this IHtmlGroupControl group )
    {
      return group.Items.Select( item => item.Value );
    }

    /// <summary>
    /// 通过 value 获取输入组项
    /// </summary>
    /// <param name="group">输入组</param>
    /// <param name="value">要查找输入组项的值</param>
    /// <returns>输入组项，如果没有找到则返回 false</returns>
    public static IHtmlInputGroupItem Item( this IHtmlGroupControl group, string value )
    {
      return group.Items.Where( item => item.Value == value ).SingleOrDefault();
    }


    /// <summary>
    /// 尝试为输入组设置一个值
    /// </summary>
    /// <param name="group">输入组</param>
    /// <param name="value">要设置的值</param>
    /// <returns>是否成功</returns>
    public static bool TrySetValue( this IHtmlGroupControl group, string value )
    {
      var item = Item( group, value );

      if ( item == null )
        return false;

      item.Selected = true;
      return true;
    }


    /// <summary>
    /// 获取字符串形式表达的 Value 值
    /// </summary>
    /// <param name="input">输入控件</param>
    /// <returns></returns>
    public static string ValueString( this IHtmlInputControl input )
    {
      var textInput = input as HtmlInputText;
      if ( textInput != null )
        return textInput.TextValue;

      var group = input as IHtmlGroupControl;
      if ( group != null )
        return string.Join( ",", group.CurrentValues().ToArray() );

      throw new NotSupportedException();
    }


    /// <summary>
    /// 查找与指定元素绑定的 Label
    /// </summary>
    /// <param name="element">要查找绑定的 Label 的元素</param>
    /// <returns>与元素绑定的 Label 集合，如果元素不支持绑定，则返回null</returns>
    public static HtmlLabel[] Labels( this IHtmlFormElement element )
    {
      var control = element as IHtmlFocusableControl;
      if ( control == null )
        return null;

      return Labels( control );
    }


    /// <summary>
    /// 查找与指定元素绑定的 Label
    /// </summary>
    /// <param name="element">要查找绑定的 Label 的元素</param>
    /// <returns>与元素绑定的 Label 集合</returns>
    public static HtmlLabel[] Labels( this IHtmlFocusableControl control )
    {
      return control.Form.FindLabels( control.Element );
    }


    /// <summary>
    /// 尝试获取与指定元素绑定的 Label 的文本
    /// </summary>
    /// <param name="element">要查找绑定的 Label 的元素</param>
    /// <returns>绑定的 Label 的文本，如果元素不支持绑定或没找到则返回null</returns>
    public static string LabelText( this IHtmlFormElement element )
    {
      var control = element as IHtmlFocusableControl;
      if ( control == null )
        return null;

      return LabelText( control );
    }



    /// <summary>
    /// 尝试获取与指定元素绑定的 Label 的文本
    /// </summary>
    /// <param name="element">要查找绑定的 Label 的元素</param>
    /// <returns>绑定的 Label 的文本，如果没找到则返回null</returns>
    public static string LabelText( this IHtmlFocusableControl control )
    {
      var label = Labels( control ).SingleOrDefault();
      if ( label == null )
        return null;

      return label.Text;
    }

  }
}
