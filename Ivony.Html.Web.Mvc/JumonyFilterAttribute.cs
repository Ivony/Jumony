using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using Ivony.Html;
using System.Web.Routing;
using System.Web;

namespace Ivony.Html.Web.Mvc
{

  [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false )]
  public class JumonyFilterAttribute : FilterAttribute, IActionFilter
  {


    private Type _handlerType;
    private IHtmlHandlerProvider _provider;


    public JumonyFilterAttribute( Type handlerType )
    {
      if ( handlerType == null )
        throw new ArgumentNullException( "handlerType" );



      if ( typeof( IHtmlHandlerProvider ).IsAssignableFrom( handlerType ) )
        _provider = (IHtmlHandlerProvider) Activator.CreateInstance( handlerType );

      else if ( typeof( IHtmlHandler ).IsAssignableFrom( handlerType ) )
        _handlerType = handlerType;

      else
        throw new InvalidOperationException();
    }



    void IActionFilter.OnActionExecuted( ActionExecutedContext filterContext )
    {
      var viewResult = filterContext.Result as ViewResultBase;

      if ( viewResult != null )
      {
        var handler = CreateHandler( filterContext.HttpContext );

        if ( handler != null )
          filterContext.Result = new ViewResultWrapper( viewResult, handler );
      }
    }

    private IHtmlHandler CreateHandler( HttpContextBase context )
    {
      if ( _handlerType != null )
        return (IHtmlHandler) Activator.CreateInstance( _handlerType );
      else
        return _provider.GetHandler( context, null );
    }

    void IActionFilter.OnActionExecuting( ActionExecutingContext filterContext )
    {
    }

  }
}
