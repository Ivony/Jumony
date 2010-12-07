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

      if ( reader == null )
        throw new ArgumentNullException( "reader" );

      if ( startIndex < 0 )
        throw new ArgumentOutOfRangeException( "startIndex" );

      if ( length <= 0 )
        throw new ArgumentOutOfRangeException( "length" );

      if ( startIndex >= reader.HtmlText.Length )
        throw new ArgumentOutOfRangeException( "startIndex" );

      if ( startIndex + length > reader.HtmlText.Length )
        throw new ArgumentOutOfRangeException( "length" );



      Reader = reader;
      StartIndex = startIndex;
      Length = length;
    }


    internal HtmlContentFragment( HtmlContentFragment info )
    {
      if ( info == null )
        throw new ArgumentNullException( "info" );

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

    public string Html
    {
      get { return Reader.HtmlText.Substring( StartIndex, Length ); }
    }

    public override string ToString()
    {
      return Html;
    }

  }
}
