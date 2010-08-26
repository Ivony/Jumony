using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html.Forms
{

  /// <summary>
  /// 输入项组，是input type="checkbox"及input type="radio"的抽象。
  /// </summary>
  public class HtmlButtonGroup : IHtmlInputGroup
  {


    private HtmlInputItem[] items;

    private string name;


    private HtmlButtonGroup( IGrouping<string, IHtmlElement> inputGroup )
    {
      name = inputGroup.Key;

      items = inputGroup.Select( e => new HtmlInputItem( this, e ) ).ToArray();
    }


    public static IEnumerable<HtmlButtonGroup> CaptureInputGroups( HtmlForm form )
    {

      var inputItems = form.Element.Find( "input[type=radio][name]", "input[type=checkbox][name]" );

      var groups = inputItems.GroupBy( item => item.Attribute( "name" ).Value() )
          .Select( item => new HtmlButtonGroup( item ) );

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

    public bool AllowMultipleSelections
    {
      get
      {
        if ( items.Length < 2 )
          return false;

        //如果有任何一个复选框
        if ( items.Select( i => i.Element ).Any( e => e.Attribute( "type" ).Value().Equals( "checkbox", StringComparison.InvariantCultureIgnoreCase ) ) )
          return true;

        return false;
      }
    }


    public IHtmlInputGroupItem[] Items
    {
      get { return items; }
    }



    public class HtmlInputItem : IHtmlInputGroupItem
    {

      public HtmlInputItem( HtmlButtonGroup group, IHtmlElement element )
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

        Element = element;
      }



      public IHtmlElement Element
      {
        get;
        private set;
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
          return Element.Attribute( "value" ).Value();
        }
        set
        {
          Element.SetAttribute( "value" ).Value( value );
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
            Element.SetAttribute( "checked" ).Value( "checked" );
          else
            Element.Attribute( "checked" ).Remove();
        }
      }

      public string Text
      {
        get { throw new NotImplementedException(); }
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
