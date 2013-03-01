using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Ivony.Html.Dynamic
{
  public class DynamicElement : DynamicObject
  {

    protected IHtmlElement Element
    {
      get;
      private set;
    }

    public DynamicElement( IHtmlElement element )
    {
      Element = element;
    }


    public override bool TryGetMember( GetMemberBinder binder, out object result )
    {

      if ( binder.ReturnType.IsAssignableFrom( typeof( string ) ) )
      {

        switch ( binder.Name )
        {
          case "tagName":
            result = Element.Name;
            return true;

          case "innerText":
            result = Element.InnerText();
            return true;

          case "innerHTML":
            result = Element.InnerHtml();
            return true;
        }

        if ( FindAttribute( binder.Name, out result ) )
          return true;

      }

      result = null;
      return true;
    }

    public override bool TrySetMember( SetMemberBinder binder, object value )
    {

      string str = value as string;

      if ( str != null )
      {

        switch ( binder.Name )
        {
          case "tagName":
            throw new NotSupportedException( "元素名不能被修改" );

          case "innerText":
            Element.InnerText( str );
            return true;

          case "innerHTML":
            Element.InnerHtml( str );
            return true;
        }

        Element.SetAttribute( binder.Name, str );

      }

      return false;
    }

    private bool FindAttribute( string name, out object result )
    {
      var attribute = Element.Attribute( name );
      if ( attribute != null )
      {
        result = attribute.AttributeValue;
        return true;
      }

      result = null;
      return false;
    }

  }
}
