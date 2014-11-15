using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ivony.Html.Binding
{
  public static class DynamicBinder
  {


    public static object GetMember( object obj, string memberName )
    {
      var callsite = CallSite<Func<CallSite, object, object>>.Create( new TrivialGetMemberBinder( memberName ) );
      return callsite.Target( callsite, obj );


    }

    public static object Test( dynamic obj )
    {
      return obj.A;
    }


    private class TrivialGetMemberBinder : GetMemberBinder
    {
      public TrivialGetMemberBinder( string propertyName )
        : base( propertyName, false )
      {
      }
      public override DynamicMetaObject FallbackGetMember( DynamicMetaObject target, DynamicMetaObject errorSuggestion )
      {
        return errorSuggestion;
      }
    }
  }
}
