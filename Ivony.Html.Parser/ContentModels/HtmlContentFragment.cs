using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser.ContentModels
{
  public class HtmlContentFragment
  {

    public HtmlContentFragment( IHtmlReader reader, int startIndex, int length )
    {
      Reader = reader;
      StartIndex = startIndex;
      Length = length;
    }

    
    internal HtmlContentFragment( HtmlContentFragment info )
    {
      Reader = info.Reader;
      StartIndex = info.StartIndex;
      Length = info.Length;
    }


    public IHtmlReader Reader
    {
      get;
      private set;
    }

    public int StartIndex
    {
      get;
      private set;
    }

    public int Length
    {
      get;
      private set;
    }

    public string NodeHtml
    {
      get { return Reader.HtmlText.Substring( StartIndex, Length ); }
    }

  }
}
