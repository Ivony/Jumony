using Ivony.Html.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Ivony.Fluent;

using Ivony.Fluent;

namespace Ivony.Html.Web
{
  public class ActionUrlBinder : IHtmlElementBinder
  {



    public ActionUrlBinder( JumonyUrlHelper urlHelper )
    {

      if ( urlHelper == null )
        throw new ArgumentNullException( "urlHelper" );

      UrlHelper = urlHelper;

    }


    public JumonyUrlHelper UrlHelper
    {
      get;
      private set;
    }


    protected RouteData RouteData
    {
      get { return UrlHelper.RequestContext.RouteData; }
    }




    private HashSet<string> applyElements = new HashSet<string>( StringComparer.OrdinalIgnoreCase ) { "a", "img", "script", "form" };


    public void BindElement( HtmlBindingContext context, IHtmlElement element )
    {
      ProcessActionLink( element );
    }


    private bool ProcessActionLink( IHtmlElement element )
    {
      if ( element.Attribute( "action" ) == null )
        return false;

      string attributeName;

      if ( element.Name.EqualsIgnoreCase( "a" ) )
        attributeName = "href";

      else if ( element.Name.EqualsIgnoreCase( "img" ) || element.Name.EqualsIgnoreCase( "script" ) )
        attributeName = "src";

      else if ( element.Name.EqualsIgnoreCase( "form" ) && element.Attribute( "controller" ) != null )
        attributeName = "action";

      else
        return false;



      var action = element.Attribute( "action" ).Value() ?? RouteData.Values["action"].CastTo<string>();
      var controller = element.Attribute( "controller" ).Value() ?? RouteData.Values["controller"].CastTo<string>();


      var routeValues = UrlHelper.GetRouteValues( element );


      element.RemoveAttribute( "action" );
      element.RemoveAttribute( "controller" );
      element.RemoveAttribute( "inherits" );


      var url = UrlHelper.Action( action, controller, routeValues );

      if ( url == null )
        element.Attribute( attributeName ).Remove();

      else
        element.SetAttribute( attributeName, url );


      return true;
    }
  }
}
