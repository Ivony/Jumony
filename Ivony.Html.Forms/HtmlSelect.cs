using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 表示一个 &gt;select&lt; 元素
  /// </summary>
  public class HtmlSelect : IHtmlGroupControl, IHtmlFocusableControl
  {

    public HtmlSelect( HtmlForm form, IHtmlElement element )
    {
      if ( !element.Name.EqualsIgnoreCase( "select" ) )
        throw new InvalidOperationException();

      _form = form;

      _element = element;

      options = element.Find( "option" ).Select( e => new HtmlOption( this, e ) ).ToArray();

    }



    private readonly HtmlForm _form;
    private readonly IHtmlElement _element;

    private readonly HtmlOption[] options;


    /// <summary>
    /// 获取 DOM 上的 &gt;select&lt; 元素
    /// </summary>
    public IHtmlElement Element
    {
      get { return _element; }
    }


    /// <summary>
    /// 获取所属的表单对象
    /// </summary>
    public HtmlForm Form
    {
      get { return _form; }

    }


    /// <summary>
    /// 获取输入控件名
    /// </summary>
    public string Name
    {
      get { return _element.Attribute( "name" ).AttributeValue; }

    }


    /// <summary>
    /// 是否允许多选
    /// </summary>
    public bool AllowMultipleSelections
    {
      get { return _element.Attribute( "multiple" ) != null; }
    }


    /// <summary>
    /// 获取输入组项
    /// </summary>
    public IHtmlInputGroupItem[] Items
    {
      get { return options; }
    }



    /// <summary>
    /// 表示一个 &gt;option&lt; 元素
    /// </summary>
    public class HtmlOption : IHtmlInputGroupItem
    {


      private IHtmlElement _element;
      private HtmlSelect _select;

      /// <summary>
      /// 创建 HtmlOption 对象
      /// </summary>
      /// <param name="select">所属的 HtmlSelect 对象</param>
      /// <param name="element">DOM 上对应的 &gt;option&lt; 元素</param>
      public HtmlOption( HtmlSelect select, IHtmlElement element )
      {
        _select = select;
        _element = element;
      }


      public IHtmlElement Element
      {
        get { return _element; }
      }

      public HtmlForm Form
      {
        get { return Group.Form; }
      }

      public IHtmlGroupControl Group
      {
        get { return _select; }
      }

      public bool Selected
      {
        get { return Element.Attribute( "selected" ) != null; }
        set
        {
          if ( value )
          {
            Element.SetAttribute( "selected", "selected" );
          }
          else
          {
            var attribute = Element.Attribute( "selected" );
            if ( attribute != null )
              attribute.Remove();
          }
        }
      }

      public string Value
      {
        get
        {
          var value = Element.Attribute( "value" ).Value();

          if ( value == null )
            return Element.InnerText();
          else
            return value;
        }
      }

      public string Text
      {
        get { return Element.InnerText(); }
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
