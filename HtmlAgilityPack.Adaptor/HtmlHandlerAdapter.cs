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

    protected override void ProcessDocument()
    {

      ApplyBindingSheets();

      using ( var bindingContext = HtmlBindingContext.EnterContext( Document, "global" ) )
      {
        Process();

        bindingContext.Commit();
      }


      var meta = HtmlDocument.CreateElement( "meta" );
      meta.SetAttributeValue( "name", "generator" );
      meta.SetAttributeValue( "content", "Jumony; HtmlAgilityPack" );

      var header = HtmlDocument.Find( "html head" ).FirstOrDefault();

      if ( header != null )
      {
        if ( header.HasChildNodes )
          header.InsertBefore( meta, header.ChildNodes[0] );
        else
          header.AppendChild( meta );
      }


      Trace.Write( "Core", "Begin Write Response" );
      HtmlDocument.Save( Response.Output );
      Trace.Write( "Core", "End Write Response" );
    }


    private void ApplyBindingSheets()
    {
      var bindingSheets = Find( "link[rel=Bindingsheet]" )
        .Select( link => link.Attribute( "href" ) )
        .Where( href => href != null )
        .Select( href => MapPath( href.Value ) )
        .Select( physicalPath => HtmlBindingSheet.Load( physicalPath ) );

      HtmlBindingContext.EnterContext( Document, "ApplyBindingSheet" );

      bindingSheets
        .ForAll( sheet => sheet.Apply() );

      HtmlBindingContext.ExitContext();


    }


    protected override IHtmlDocument LoadDocument( string documentContent )
    {
      HtmlDocument = new HtmlDocument();

      HtmlDocument.LoadHtml( documentContent );

      return HtmlDocument.AsDocument();
    }

    protected HtmlDocument HtmlDocument
    {
      get;
      private set;
    }

    protected abstract void Process();

  }
}
