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



    public void OnActionExecuted( ActionExecutedContext filterContext )
    {
      var viewResult = filterContext.Result as ViewResultBase;

      if ( viewResult != null )
        filterContext.Result = new ViewResultWrapper( viewResult, CreateHandler( filterContext.RequestContext ) );
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


    private class ViewResultWrapper : ActionResult
    {

      private ViewResultBase _result;

      private IHtmlHandler _handler;

      public ViewResultWrapper( ViewResultBase result, IHtmlHandler handler )
      {
        _result = result;
        _handler = handler;
      }

      public override void ExecuteResult( ControllerContext context )
      {
        var response = context.HttpContext.Response;
        var responseWriter = response.Output;          //先保存下标准输出


        string content;

        using ( var writer = new StringWriter() )      //创建StringWriter截获标准输出
        {
          response.Output = writer;

          _result.ExecuteResult( context );            //执行操作

          content = writer.ToString();
        }


        var parser = HtmlProviders.GetParser( context.HttpContext, context.HttpContext.Request.AppRelativeCurrentExecutionFilePath, content );

        var document = parser.Parse( content );

        _handler.ProcessDocument( document );          //处理文档

        response.Output = responseWriter;              //将标准输出复原

        document.Render( responseWriter );             //输出处理后的结果
      }
    }


    private class ViewWrapper : IView
    {

      private IView _view;
      private IHtmlHandler _handler;


      public IView View
      {
        get;
        private set;
      }

      public ViewWrapper( IView view, IHtmlHandler handler )
      {

        if ( view == null )
          throw new ArgumentNullException( "view" );

        if ( handler == null )
          throw new ArgumentNullException( "handler" );



        _view = view;
        _handler = handler;
      }

      public void Render( ViewContext viewContext, TextWriter writer )
      {
        var innerWriter = new StringWriter();

        _view.Render( viewContext, innerWriter );

        string content = innerWriter.ToString();

        var parser =HtmlProviders.GetParser( viewContext.HttpContext, viewContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath, content );

        var document = parser.Parse( content );

        _handler.ProcessDocument( document );

        document.Render( writer );
      }
    }
  }
}
