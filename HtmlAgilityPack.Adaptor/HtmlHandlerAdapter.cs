using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Web;

namespace Ivony.Web.Html.HtmlAgilityPackAdaptor
{
  public abstract class HtmlHandlerAdapter : HtmlHandler
  {
    public override bool IsReusable
    {
      get { return false; }
    }

    public override void ProcessRequest()
    {
      HtmlDocument document = new HtmlDocument();

      document.LoadHtml( GetTemplateContent() );
      Document = document;

      using ( var bindingContext = HtmlBindingContext.EnterContext( DocumentNode ) )
      {
        Process();

        bindingContext.Commit();
      }

      document.Save( Response.Output );
    }

    protected IEnumerable<IHtmlElement> Find( string selector )
    {
      return DocumentNode.Find( selector );
    }

    protected HtmlDocument Document
    {
      get;
      private set;
    }

    protected IHtmlContainer DocumentNode
    {
      get { return Document.AsContainer(); }
    }

    protected abstract void Process();

  }
}
