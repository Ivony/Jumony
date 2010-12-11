using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using Ivony.Html;
using System.Web.Routing;

namespace Ivony.Html.Web.Mvc
{

  [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false )]
  public class JumonyFilterAttribute : FilterAttribute, IActionFilter
  {


    private Type _handlerType;
    private IHtmlHandlerFactory _factory;


    public JumonyFilterAttribute( Type handlerType )
    {
      if ( handlerType == null )
        throw new ArgumentNullException( "handlerType" );



      if ( typeof( IHtmlHandlerFactory ).IsAssignableFrom( handlerType ) )
        _factory = (IHtmlHandlerFactory) Activator.CreateInstance( handlerType );

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
        var handler = CreateHandler( filterContext.RequestContext );

        if ( handler != null )
          filterContext.Result = new JumonyViewResult( viewResult, handler );
      }
    }

    private IHtmlHandler CreateHandler( RequestContext context )
    {
      if ( _handlerType != null )
        return (IHtmlHandler) Activator.CreateInstance( _handlerType );
      else
        return _factory.CreateHandler( context );
    }

    void IActionFilter.OnActionExecuting( ActionExecutingContext filterContext )
    {
    }

  }
}
