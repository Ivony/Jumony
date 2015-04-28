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

    public static dynamic GetPropertyValue( object obj, string name )
    {

      var site = CallSite<Func<CallSite, object, object>>.Create( new HtmlBindingGetMemberBinder( name ) );
      return site.Target( site, obj );

    }



    public static dynamic GetIndexPropertyValue( object obj, string name )
    {

      var site = CallSite<Func<CallSite, object, object>>.Create( new HtmlBindingGetMemberBinder( name ) );
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
          return DynamicMetaObject.Create( null, Expression.Constant( null ) );

        else
          return errorSuggestion;
      }
    }



    private class HtmlBindingGetIndexBinder : GetIndexBinder
    {

      public HtmlBindingGetIndexBinder( CallInfo info ) : base( info ) { }

      public override DynamicMetaObject FallbackGetIndex( DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject errorSuggestion )
      {
        if ( errorSuggestion == null )
          return DynamicMetaObject.Create( null, Expression.Constant( null ) );

        else
          return errorSuggestion;
      }
    }




  }
}
