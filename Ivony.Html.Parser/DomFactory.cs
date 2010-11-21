using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomFactory : IHtmlNodeFactory
  {


    internal DomFactory( DomDocument document )
    {
      Document = document;
    }


    internal DomDocument Document
    {
      get;
      private set;
    }


    #region IHtmlNodeFactory 成员

    public IFreeElement CreateElement( string name )
    {
      return new DomFreeElement( this, name );
    }

    public IFreeTextNode CreateTextNode( string htmlText )
    {
      return new DomFreeTextNode( this, htmlText );
    }

    public IFreeComment CreateComment( string comment )
    {
      return new DomFreeComment( this, comment );
    }

    public IHtmlDocument CreateDocument()
    {
      return new DomDocument();
    }


    public HtmlFragment ParseHtml( string html )
    {
      return this.MakeFragment( new JumonyParser().Parse( html ) );
    }

    #endregion
  }


}
