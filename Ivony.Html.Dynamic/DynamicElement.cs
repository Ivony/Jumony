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


    public override bool TryInvokeMember( InvokeMemberBinder binder, object[] args, out object result )
    {
      return base.TryInvokeMember( binder, args, out result );
    }

    public override bool TryGetMember( GetMemberBinder binder, out object result )
    {

      result = null;

      if ( binder.ReturnType.IsAssignableFrom( typeof( string ) ) )
      {

        switch ( binder.Name.ToLowerInvariant() )
        {
          case "tagName":
            result = Element.Name;
            return true;
        }

        if ( FindAttribute( binder.Name, ref result ) )
          return true;

      }

      return true;

    }

    private bool FindAttribute( string name, ref object result )
    {
      var attribute = Element.Attribute( name );
      if ( attribute != null )
      {
        result = attribute.AttributeValue;
        return true;
      }

      return false;
    }

  }
}
