using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace Ivony.Html.Web.Mvc
{
  public class JumonyTemplate : IDisposable
  {


    private ViewContext _context;

    private IHtmlHandler _handler;

    private TextWriter originWriter;

    private StringWriter interceptor = new StringWriter();


    internal JumonyTemplate( ViewContext context, IHtmlHandler handler )
    {

      _context = context;

      _handler = handler;

      originWriter = _context.Writer;

      _context.Writer = interceptor;

    }

    #region IDisposable 成员

    public void Dispose()
    {

      var html = interceptor.ToString();

      var document = HtmlProviders.LoadDocument( _context.HttpContext, null );

      _handler.ProcessDocument( document );

      document.Render( originWriter );

      _context.Writer = originWriter;
    }

    #endregion
  }
}
