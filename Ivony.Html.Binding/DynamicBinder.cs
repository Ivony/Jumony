using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Ivony.Html.Binding
{
  public static class DynamicBinder
  {

    public static object Eval( dynamic obj, string expression )
    {

      throw new NotImplementedException();

    }


    public static dynamic EvalDyanmic( dynamic obj, string expression )
    {

      throw new NotImplementedException();

    }

    public static dynamic GetMember( object obj, string memberName )
    {

      var site = CallSite<Func<CallSite, object, object>>.Create( new HtmlBindingGetMemberBinder( memberName ) );
      return site.Target( site, obj );

    }



    private sealed class DynamicMemberGetter
    {


      public string MemberName { get; private set; }


      private CallSite<Func<CallSite, object, object>> site;

      public DynamicMemberGetter( string memberName )
      {
        site = CallSite<Func<CallSite, object, object>>.Create( new HtmlBindingGetMemberBinder( memberName ) );
      }


      public object GetMemberValue( object target )
      {
        return site.Target( site, target );
      }
    }


    private class HtmlBindingGetMemberBinder : GetMemberBinder
    {
      public HtmlBindingGetMemberBinder( string name ) : base( name, false ) { }

      public override DynamicMetaObject FallbackGetMember( DynamicMetaObject target, DynamicMetaObject errorSuggestion )
      {

        if ( errorSuggestion == null )
          return DynamicMetaObject.Create( "[dynamic binding error]", Expression.Constant( "[dynamic binding error]" ) );

        else
          return errorSuggestion;
      }
    }



    private class HtmlBindingGetIndexBinder : GetIndexBinder
    {

      public HtmlBindingGetIndexBinder( CallInfo info ) : base( info ) { }

      public override DynamicMetaObject FallbackGetIndex( DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject errorSuggestion )
      {
        throw new NotImplementedException();
      }
    }



  }
}
