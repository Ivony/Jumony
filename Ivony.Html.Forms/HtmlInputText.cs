using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义一个文本输入框元素
  /// </summary>
  public class HtmlInputText : IHtmlTextControl
  {

    private readonly HtmlForm _form;
    private readonly IHtmlElement _element;
    private readonly string _type;

    private string[] allowTypes = new[] { "text", "password", "hidden" };


    internal HtmlInputText( HtmlForm form, IHtmlElement element )
    {
      if ( !element.Name.EqualsIgnoreCase( "input" ) )
        throw new InvalidOperationException( "只有input元素才能转换为HtmlTextInput对象" );


      _type = element.Attribute( "type" ).Value();
      if ( !allowTypes.Contains( _type, StringComparer.OrdinalIgnoreCase ) )
        throw new InvalidOperationException( "只有type为text、password或hidden的input元素才能转换为HtmlTextInput对象" );

      _form = form;
      _element = element;
    }


    /// <summary>
    /// 输入框所属的表单
    /// </summary>
    public HtmlForm Form
    {
      get { return _form; }
    }


    /// <summary>
    /// 描述输入框的元素
    /// </summary>
    public IHtmlElement Element
    {
      get { return _element; }
    }


    /// <summary>
    /// 输入框的名称
    /// </summary>
    public string Name
    {
      get { return _element.Attribute( "name" ).AttributeValue; }
    }


    /// <summary>
    /// 输入框的文本值
    /// </summary>
    public string TextValue
    {
      get { return _element.Attribute( "value" ).Value(); }
      set
      {
        if ( !_type.EqualsIgnoreCase( "password" ) )
          _element.SetAttribute( "value", value );
      }
    }


    #region IHtmlFocusableControl 成员

    string IHtmlFocusableControl.ElementId
    {
      get { return Element.Identity(); }
    }

    #endregion


  }
}
