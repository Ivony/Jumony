using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 提供一组关于表单的扩展方法
  /// </summary>
  public static class FormExtensions
  {

    /// <summary>
    /// 尝试将一个HTML元素转换为表单
    /// </summary>
    /// <param name="element">要转换为表单的元素</param>
    /// <returns></returns>
    public static HtmlForm AsForm( this IHtmlElement element )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );


      return new HtmlForm( element );
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
      return control.Form.FindLabels( control.ElementId );
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
      var labels = Labels( control );
      if ( labels.IsSingle() )
        return labels.First().Text;

      else
        return null;
    }


    /// <summary>
    /// 尝试查找最小包含指定输入控件的容器
    /// </summary>
    /// <param name="inputControl">输入控件</param>
    /// <returns>找到的最小包含容器</returns>
    public static IHtmlContainer FindContainer( this IHtmlInputControl inputControl )
    {
      var inputText = inputControl as HtmlInputText;
      if ( inputText != null )
        return inputText.Element.Container;

      var textarea = inputControl as HtmlTextArea;
      if ( textarea != null )
        return textarea.Element.Container;

      var select = inputControl as HtmlSelect;
      if ( select != null )
        return select.Element.Container;

      var group = inputControl as HtmlButtonGroup;
      if ( group != null )
        return FindContainer( group );

      throw new NotSupportedException();

    }


    private static IHtmlContainer FindContainer( HtmlButtonGroup group )
    {
      var container = group.Items.Select( i => i.Element ).Aggregate( ( item1, item2 ) =>
        {
          return item1.AncestorsAndSelf().FirstOrDefault( e => e.IsAncestorOf( item2 ) || e.Equals( item2 ) );

        } );

      return container;//没有处理文档为公共容器的情况。
    }



  }
}
