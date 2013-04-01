using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using Ivony.Html;
using Ivony.Fluent;
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
      var specification = textNode.Document.HtmlSpecification;

      if ( !textNode.Ancestors().Any( e => e.ElementTextMode() == TextMode.Preformated ) )
        SourceTerm = whitespaceRegex.Replace( textNode.HtmlText, " " );

      else
        SourceTerm = textNode.HtmlText;
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
