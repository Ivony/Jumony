using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using Ivony.Html;
using Ivony.Fluent;
using mshtml;

namespace HtmlTranslator
{
  public class TranslationTerm : INotifyPropertyChanged
  {


    private IHtmlTextNode _textNode;

    public TranslationTerm( IHtmlTextNode textNode )
    {
      _textNode = textNode;

      _sourceTerm = _textNode.HtmlText;
      _translatedTerm = _sourceTerm;
    }

    private string _sourceTerm;
    public string SourceTerm
    {
      get { return _sourceTerm; }
      set
      {
        _sourceTerm = value;
        OnPropertyChanged( "SourceTerm" );
      }
    }


    private string _translatedTerm;
    public string TranslatedTerm
    {
      get { return _translatedTerm; }
      set
      {
        _translatedTerm = value;
        OnPropertyChanged( "TranslatedTerm" );

        _textNode.RawObject.CastTo<IHTMLDOMTextNode>().data = value;
      }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged( string propertyName )
    {
      if ( PropertyChanged != null )
        PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
    }
  }
}
