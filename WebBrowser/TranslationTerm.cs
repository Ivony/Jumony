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

namespace HtmlTranslator
{
  [DataContract]
  public class TranslationTerm
  {


    public TranslationTerm( IHtmlTextNode textNode )
    {
      TextNode = textNode;
      SourceTerm = textNode.HtmlText;
      TranslatedTerm = textNode.HtmlText;
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
