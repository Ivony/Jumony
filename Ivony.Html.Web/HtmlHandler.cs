using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{
  public class HtmlHandler : HtmlHandlerBase, IHtmlHandler
  {


    public virtual void ProcessScope( HtmlRequestContext context, IHtmlContainer scope )
    {
      _context = context;
      _scope = scope;
    }




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
  }
}
