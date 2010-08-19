using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Web;
using Ivony.Fluent;

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

      ApplyBindingSheets();

      using ( var bindingContext = HtmlBindingContext.EnterContext( DocumentNode ) )
      {
        Process();

        bindingContext.Commit();
      }


      var meta = Document.CreateElement( "meta" );
      meta.SetAttributeValue( "name", "generator" );
      meta.SetAttributeValue( "content", "Jumony" );

      Document.Find( "html head" ).First().AppendChild( meta );

      document.Save( Response.Output );
    }

    private void ApplyBindingSheets()
    {
      var bindingSheets = Find( "link[rel=Bindingsheet]" )
        .Select( link => link.Attribute( "href" ) )
        .Where( href => href != null )
        .Select( href => MapPath( href.Value ) )
        .Select( physicalPath => HtmlBindingSheet.Load( physicalPath ) );

      HtmlBindingContext.EnterContext( DocumentNode );

      bindingSheets
        .ForAll( sheet => sheet.Apply() );

      HtmlBindingContext.ExitContext();


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
