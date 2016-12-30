using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ivony.Html
{
  public class HtmlParseProcess
  {

    public HtmlParseProcess( IHtmlReader reader, Uri url )
    {

      var document = CreateDocument( url );


    }

    protected virtual HtmlDocument CreateDocument( Uri url )
    {
      return new HtmlDocument( url );
    }
  }
}
