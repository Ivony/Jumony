using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace Ivony.Html.Web.Mvc
{
  public class ViewWrapper : IView
  {

    public IView WrappedView
    {
      get;
      private set;
    }

    public IHtmlHandler Handler
    {
      get;
      private set;
    }

    public ViewWrapper( IView view, IHtmlHandler handler )
    {
      WrappedView = view;
      Handler = handler;
    }



    public void Render( ViewContext viewContext, System.IO.TextWriter writer )
    {
      var innerWriter = new StringWriter();

      WrappedView.Render( viewContext, writer );

      var document = HtmlProviders.ParseDocument( viewContext.HttpContext, null, innerWriter.ToString() );

      Handler.ProcessDocument( viewContext.HttpContext, document );

      document.Render( writer );
    }
  }
}
