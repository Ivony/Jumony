using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  internal class HtmlInputButtonGroupItem : IHtmlInputGroupItem, IHtmlFocusableControl
  {

    public HtmlInputButtonGroupItem( HtmlButtonGroup group, IHtmlElement element )
    {
      _group = group;

      if ( !element.Name.EqualsIgnoreCase( "input" ) )
        throw new InvalidOperationException();

      var type = element.Attribute( "type" ).Value();

      if ( type.EqualsIgnoreCase( "radio" ) )
        radio = true;

      else if ( type.EqualsIgnoreCase( "checkbox" ) )
        radio = false;

      else
        throw new InvalidOperationException();

      if ( string.IsNullOrEmpty( element.Attribute( "name" ).Value() ) )
        throw new InvalidOperationException();

      _element = element;
    }


    private readonly bool radio;
    private readonly HtmlButtonGroup _group;
    private readonly IHtmlElement _element;


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
      get { return _group; }
    }


    public string Value
    {
      get
      {
        return Element.Attribute( "value" ).Value();
      }
      set
      {
        Element.SetAttribute( "value", value );
      }
    }

    public bool Selected
    {
      get
      {
        return Element.Attribute( "checked" ) != null;
      }
      set
      {
        if ( value )
        {
          if ( radio )//如果是单选按钮，那么只有一个可以被选中
            _group.items.Where( item => item.radio ).ForAll( item => item.Selected = false );

          Element.SetAttribute( "checked", "checked" );

          return;
        }


        var attribute = Element.Attribute( "checked" );
        if ( attribute != null )
          attribute.Remove();
      }
    }

    public string Text
    {
      get
      {
        var label = Group.Labels().FirstOrDefault();

        if ( label == null )
          return null;

        return label.Text;
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
