using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using Ivony.Html;
using Ivony.Fluent;
using mshtml;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace HtmlTranslator
{
  [DataContract]
  public class TranslationTerm
  {


    private static readonly Regex whitespaceRegex = new Regex( @"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant );


    public TranslationTerm( IHtmlTextNode textNode )
    {
      TextNode = textNode;
      if ( !textNode.Ancestors().Any( e => HtmlSpecification.preformatedElements.Contains( e.Name, StringComparer.OrdinalIgnoreCase ) ) )
      {
        SourceTerm = whitespaceRegex.Replace( textNode.HtmlText, " " );
        TranslatedTerm = whitespaceRegex.Replace( textNode.HtmlText, " " );
      }
      else
      {
        SourceTerm = textNode.HtmlText;
        TranslatedTerm = textNode.HtmlText;
      }
    }


    internal IHtmlTextNode TextNode
    {
      get;
      set;
    }

    [DataMember]
    public string SourceTerm
    {
      get;
      private set;
    }

    [DataMember]
    public string TranslatedTerm
    {
      get;
      set;
    }

  }
}
