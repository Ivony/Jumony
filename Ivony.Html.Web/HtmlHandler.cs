using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{
  public abstract class HtmlHandler : HtmlHandlerBase, IHtmlHandler, IHttpHandler
  {


    public void ProcessScope( HtmlRequestContext context, IHtmlContainer scope )
    {
      _context = context;
      _scope = scope;

      ProcessScope();
    }

    protected abstract void ProcessScope();





    private IHtmlContainer _scope;

    public override IHtmlContainer Scope
    {
      get { return _scope; }
    }


    private HtmlRequestContext _context;
    protected HtmlRequestContext Context
    {
      get { return _context; }
    }

    public override string VirtualPath
    {
      get { return Context.VirtualPath; }
    }


    protected override System.Web.HttpContextBase HttpContext
    {
      get { return Context.HttpContext; }
    }

    public virtual void Dispose()
    {
    }

    public bool IsReusable
    {
      get { throw new NotImplementedException(); }
    }

    public void ProcessRequest( HttpContext context )
    {
      throw JumonyHandler.DirectVisitError();
    }
  }
}
