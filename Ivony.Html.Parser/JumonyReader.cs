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

      CDataElement = null;

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



    protected int Index
    {
      get;
      private set;
    }


    private static readonly IDictionary<string, Regex> endTagRegexes = new Dictionary<string, Regex>( StringComparer.InvariantCultureIgnoreCase );

    private object _sync = new object();


    protected Regex GetEndTagRegex( string tagName )
    {
      tagName = tagName.ToLowerInvariant();

      lock ( _sync )
      {
        Regex regex;

        if ( !endTagRegexes.TryGetValue( tagName, out regex ) )
          endTagRegexes.Add( tagName, regex = new Regex( @"</#tagName\s*>".Replace( "#tagName", tagName ), RegexOptions.IgnoreCase | RegexOptions.Compiled ) );

        return regex;
      }
    }


    public IEnumerable<HtmlContentFragment> EnumerateContent()
    {

      Index = 0;

      while ( true )
      {

        //CData标签处理

        if ( CDataElement != null )//如果在CData标签内。
        {

          Regex endTagRegex = GetEndTagRegex( CDataElement );
          var endTagMatch = endTagRegex.Match( HtmlText, Index );


          if ( !endTagMatch.Success )
          {
            //处理末尾的文本
            if ( Index != HtmlText.Length )
              yield return CreateText( HtmlText.Length );

            yield break;
          }


          if ( endTagMatch.Index > Index )
            yield return CreateText( endTagMatch.Index );

          yield return new HtmlEndTag( CreateFragment( endTagMatch ), CDataElement );
        }


        var match = tagRegex.Match( HtmlText, Index );

        if ( !match.Success )//如果不再有标签的匹配
        {
          //处理末尾的文本
          if ( Index != HtmlText.Length )
            yield return CreateText( HtmlText.Length );

          yield break;
        }


        //处理文本节点
        if ( match.Index > Index )
          yield return CreateText( match.Index );


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

    protected virtual HtmlBeginTag CreateBeginTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;
      bool selfClosed = match.Groups["selfClosed"].Success;


      //处理所有属性
      var attributes = CreateAttributes( match );


      var fragment = CreateFragment( match );

      return new HtmlBeginTag( fragment, tagName, selfClosed, attributes );
    }


    protected virtual IEnumerable<HtmlAttributeSetting> CreateAttributes( Match match )
    {
      foreach ( Capture capture in match.Groups["attribute"].Captures )
      {
        string name = capture.FindCaptures( match.Groups["attrName"] ).Single().Value;
        string value = capture.FindCaptures( match.Groups["attrValue"] ).Select( c => c.Value ).SingleOrDefault();

        value = HtmlEncoding.HtmlDecode( value );


        yield return new HtmlAttributeSetting( CreateFragment( capture, false ), name, value );
      }
    }

    protected virtual HtmlEndTag CreateEndTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;

      var fragment = CreateFragment( match );
      return new HtmlEndTag( fragment, tagName );
    }

    protected virtual HtmlCommentContent CreateComment( Match match )
    {
      var commentText = match.Groups["commentText"].Value;

      var fragment = CreateFragment( match );
      return new HtmlCommentContent( fragment, commentText );
    }


    protected virtual HtmlSpecialTag CreateSpacial( Match match )
    {
      var raw = match.ToString();
      var symbol = raw.Substring( 1, 1 );
      var content = match.Groups["specialText"].Value;

      var fragment = CreateFragment( match );
      return new HtmlSpecialTag( fragment, content, symbol );
    }




    protected virtual HtmlTextContent CreateText( int endIndex )
    {
      var text = new HtmlTextContent( new HtmlContentFragment( this, Index, endIndex - Index ) );
      Index = endIndex;

      return text;
    }

    protected HtmlContentFragment CreateFragment( Capture capture )
    {
      return CreateFragment( capture, true );
    }

    protected virtual HtmlContentFragment CreateFragment( Capture capture, bool setIndex )
    {
      if ( setIndex )
        Index = capture.Index + capture.Length;

      return new HtmlContentFragment( this, capture.Index, capture.Length );
    }
  }
}
