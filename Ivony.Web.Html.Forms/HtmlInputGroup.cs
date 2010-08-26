using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html.Forms
{

  /// <summary>
  /// 输入项组，是input type="checkbox"及input type="radio"的抽象。
  /// </summary>
  public class HtmlInputGroup : IHtmlInputGroup
  {


    private HtmlInputItem[] items;

    private string name;


    private HtmlInputGroup( IGrouping<string, IHtmlElement> inputGroup )
    {
      name = inputGroup.Key;

      items = inputGroup.Select( e => new HtmlInputItem( this, e ) ).ToArray();
    }


    public static IEnumerable<HtmlInputGroup> CaptureInputGroups( HtmlForm form )
    {

      var inputItems = form.Element.Find( "input[type=radio][name]", "input[type=checkbox][name]" );

      var groups = inputItems.GroupBy( item => item.Attribute( "name" ).Value() )
          .Select( item => new HtmlInputGroup( item ) );

      return groups;

    }


    public string Name
    {
      get { return name; }
    }

    public string[] Values
    {
      get { return (from item in items where item.Selected select item.Value).ToArray(); }
    }




    private enum GroupType
    {
      Unknow,
      Select,
      Checkbox,
      Radio
    }


    public class HtmlInputItem : IHtmlInputGroupItem
    {


      private IHtmlElement _element;

      public HtmlInputItem( HtmlInputGroup group, IHtmlElement element )
      {
        Group = group;

        if ( element.Name.Equals( "input", StringComparison.InvariantCultureIgnoreCase ) )
        {
          var type = element.Attribute( "name" ).Value();

          if ( !type.Equals( "radio", StringComparison.InvariantCultureIgnoreCase ) && !type.Equals( "checkbox", StringComparison.InvariantCultureIgnoreCase ) )
            throw new InvalidOperationException();

          if ( string.IsNullOrEmpty( element.Attribute( "name" ).Value() ) )
            throw new InvalidOperationException();
        }

        _element = element;
      }


      public IHtmlInputGroup Group
      {
        get;
        private set;
      }


      public string Value
      {
        get
        {
          return _element.Attribute( "value" ).Value();
        }
        set
        {
          _element.SetAttribute( "value" ).Value( value );
        }
      }

      public bool Selected
      {
        get
        {
          return _element.Attribute( "checked" ) != null;
        }
        set
        {
          if ( value )
            _element.SetAttribute( "checked" ).Value( "checked" );
          else
            _element.Attribute( "checked" ).Remove();
        }
      }

    }


    string IHtmlInput.Value
    {
      get
      {
        return string.Join( ",", Values );
      }
    }

  }
}
