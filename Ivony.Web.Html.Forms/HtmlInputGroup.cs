using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html.Forms
{

  /// <summary>
  /// 输入项组，是select或是input type="checkbox"及input type="radio"的抽象。
  /// </summary>
  public class HtmlInputGroup
  {

    private IHtmlElement _selector;

    private HtmlInputItem[] _items;

    private string _name;

    private HtmlInputGroup( IHtmlElement selector )
    {
      if ( !selector.Name.Equals( "select", StringComparison.InvariantCultureIgnoreCase ) )
        throw new InvalidOperationException();

      _selector = selector;

      _items = _selector.Find( "option" ).Select( e => new HtmlInputItem( e ) ).ToArray();

      _name = selector.Attribute( "name" ).Value;
    }

    private HtmlInputGroup( IGrouping<string, IHtmlElement> inputGroup )
    {
      _name = inputGroup.Key;

      _items = inputGroup.Select( e => new HtmlInputItem( e ) ).ToArray();
    }


    public static HtmlInputGroup[] CaptureInputGroups( HtmlForm form )
    {

      var groups = form.Element.Find( "select[name]" ).Select( element => new HtmlInputGroup( element ) );

      var inputItems = form.Element.Find( "input[type=radio][name]" );
      inputItems = inputItems.Union( form.Element.Find( "input[type=checkbox][name]" ) );

      groups = groups.Union(
        inputItems.GroupBy( item => item.Attribute( "name" ).Value() )
          .Select( item => new HtmlInputGroup( item ) )
        );

      return groups.ToArray();
    }


    public string Name
    {
      get { return _name; }
    }

    public string[] Values
    {
      get { return (from item in _items where item.Selected select item.Value).ToArray(); }
    }




    private enum GroupType
    {
      Unknow,
      Select,
      Checkbox,
      Radio
    }


    public class HtmlInputItem
    {
      private string selectedAttributeName;

      private IHtmlElement _element;

      public HtmlInputItem( IHtmlElement element )
      {
        if ( element.Name.Equals( "option", StringComparison.InvariantCultureIgnoreCase ) )
          selectedAttributeName = "selected";

        if ( element.Name.Equals( "input", StringComparison.InvariantCultureIgnoreCase ) )
        {
          var type = element.Attribute( "name" ).Value();

          if ( !type.Equals( "radio", StringComparison.InvariantCultureIgnoreCase ) && !type.Equals( "checkbox", StringComparison.InvariantCultureIgnoreCase ) )
            throw new InvalidOperationException();

          if ( string.IsNullOrEmpty( element.Attribute( "name" ).Value() ) )
            throw new InvalidOperationException();

          selectedAttributeName = "checked";
        }

        _element = element;
      }


      public string Value
      {
        get { return _element.Attribute( "value" ).Value(); }
      }

      public bool Selected
      {
        get
        {
          if ( _element.Name.Equals( "option", StringComparison.InvariantCultureIgnoreCase ) )
            return _element.Attribute( "selected" ) != null;
          else if ( _element.Name.Equals( "input", StringComparison.InvariantCultureIgnoreCase ) )
            return _element.Attribute( "checked" ) != null;
          else
            throw new InvalidOperationException();
        }
      }

    }

  }
}
