using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html.Forms
{
  public class HtmlSelect : IHtmlInputGroup
  {

    public HtmlSelect( HtmlForm form, IHtmlElement element )
    {
      if ( !element.Name.Equals( "select", StringComparison.InvariantCultureIgnoreCase ) )
        throw new InvalidOperationException();

      Form = Form;

      select = element;

      options = element.Find( "option" ).Select( e => new HtmlOption( this, e ) ).ToArray();

      Name = element.Attribute( "name" ).AttributeValue;

    }


    private readonly IHtmlElement select;

    private readonly HtmlOption[] options;


    public HtmlForm Form
    {
      get;
      private set;
    }


    public string Name
    {
      get;
      private set;
    }

    public string[] Values
    {
      get { return (from item in options where item.Selected select item.Value).ToArray(); }
    }


    public bool AllowMultipleSelections
    {
      get { return select.Attribute( "multiple" ) != null; }
    }


    public IHtmlInputGroupItem[] Items
    {
      get { return options; }
    }


    string IHtmlInput.Value
    {
      get { return string.Join( ",", Values ); }
    }




    public class HtmlOption : IHtmlInputGroupItem
    {

      public HtmlOption( HtmlSelect select, IHtmlElement element )
      {
        Group = select;
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

      public bool Selected
      {
        get
        {
          return Element.Attribute( "selected" ) != null;
        }
        set
        {
          if ( value )
            Element.SetAttribute( "selected" ).Value( "selected" );
          else
            Element.Attribute( "selected" ).Remove();
        }
      }

      public string Value
      {
        get
        {
          var value = Element.Attribute( "value" ).Value();

          if ( value == null )
            return Element.Text();
          else
            return value;
        }
      }

      public string Text
      {
        get { return Element.Text(); }
      }

    }
  }
}
