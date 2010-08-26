using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html.Forms
{
  public class HtmlSelect : IHtmlInputGroup
  {

    private readonly IHtmlElement select;

    private readonly HtmlOption[] options;

    private string name;



    public HtmlSelect( IHtmlElement element )
    {
      if ( !element.Name.Equals( "select", StringComparison.InvariantCultureIgnoreCase ) )
        throw new InvalidOperationException();

      select = element;

      options = element.Find( "option" ).Select( e => new HtmlOption( this, e ) ).ToArray();

      name = element.Attribute( "name" ).AttributeValue;

    }



    public class HtmlOption : IHtmlInputGroupItem
    {

      private IHtmlElement _element;


      public HtmlOption( HtmlSelect select, IHtmlElement element )
      {
        Group = select;
        _element = element;
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
          return _element.Attribute( "selected" ) != null;
        }
        set
        {
          if ( value )
            _element.SetAttribute( "selected" ).Value( "selected" );
          else
            _element.Attribute( "selected" ).Remove();
        }
      }

      public string Value
      {
        get
        {
          var value = _element.Attribute( "value" ).Value();

          if ( value == null )
            return _element.Text();
          else
            return value;
        }
      }

    }



    public string Name
    {
      get { return name; }
    }

    public string[] Values
    {
      get { return (from item in options where item.Selected select item.Value).ToArray(); }
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
