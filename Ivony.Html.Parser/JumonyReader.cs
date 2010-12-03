using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser.ContentModels;
using System.Text.RegularExpressions;

namespace Ivony.Html.Parser
{
  public class JumonyReader : IHtmlReader
  {

    private static readonly string tagPattern = string.Format( @"(?<beginTag>{0})|(?<endTag>{1})|(?<comment>{2})|(?<special>{3})", Regulars.beginTagPattern, Regulars.endTagPattern, Regulars.commentPattern, Regulars.specialTagPattern );

    protected static readonly Regex tagRegex = new Regex( tagPattern, RegexOptions.Compiled );



    public JumonyReader( string htmlText )
    {
      if ( htmlText == null )
        throw new ArgumentNullException( "htmlText" );

      HtmlText = htmlText;
    }

    public string HtmlText
    {
      get;
      private set;
    }


    public string CDataElement
    {
      get;
      set;
    }



    private static readonly IDictionary<string, Regex> endTagRegexes = new Dictionary<string, Regex>( StringComparer.InvariantCultureIgnoreCase );

    private object _sync = new object();


    protected Regex GetEndTagRegex( string tagName )
    {
      lock ( _sync )
      {
        var regex = endTagRegexes[tagName];

        if ( regex == null )
          endTagRegexes.Add( tagName, regex = new Regex( @"</#tagName\s*>".Replace( "#tagName", tagName ), RegexOptions.IgnoreCase | RegexOptions.Compiled ) );

        return regex;
      }
    }


    public IEnumerable<HtmlContentFragment> EnumerateContent()
    {

      int index = 0;

      while ( true )
      {

        //CData标签处理

        if ( CDataElement != null )//如果在CData标签内。
        {

          Regex endTagRegex = GetEndTagRegex( CDataElement );
          var endTagMatch = endTagRegex.Match( HtmlText, index );


          yield return CreateText( index, endTagMatch.Index );
          yield return new HtmlEndTag( CreateFragment( endTagMatch, ref index ), CDataElement );
        }


        var match = tagRegex.Match( HtmlText, index );

        if ( !match.Success )//如果不再有标签的匹配
        {
          //处理末尾的文本
          if ( index != HtmlText.Length )
            yield return CreateText( index, HtmlText.Length );

          yield break;
        }

        //处理文本节点

        yield return CreateText( index, match.Index );


        if ( match.Groups["beginTag"].Success )
          yield return CreateBeginTag( match );

        else if ( match.Groups["endTag"].Success )
          yield return CreateEndTag( match );

        else if ( match.Groups["comment"].Success )
          yield return CreateComment( match );

        else if ( match.Groups["special"].Success )
          yield return CreateSpacial( match );

        else
          throw new InvalidOperationException();
      }
    }

    private HtmlBeginTag CreateBeginTag( Match match )
    {
      throw new NotImplementedException();
    }

    private HtmlEndTag CreateEndTag( Match match )
    {
      throw new NotImplementedException();
    }

    private HtmlCommentContent CreateComment( Match match )
    {
      throw new NotImplementedException();
    }

    private HtmlSpecialTag CreateSpacial( Match match )
    {
      throw new NotImplementedException();
    }




    protected virtual HtmlTextContent CreateText( int startIndex, int endIndex )
    {
      return new HtmlTextContent( new HtmlContentFragment( this, startIndex, endIndex - startIndex ) );
    }

    protected virtual HtmlContentFragment CreateFragment( Match match, ref int index )
    {
      index = match.Index + match.Length;
      return new HtmlContentFragment( this, match.Index, match.Length );
    }
  }
}
