using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace HtmlTranslator
{
  [Serializable]
  public class TranslateDictionary
  {

    private Dictionary<string, HashSet<string>> _dictionary = new Dictionary<string, HashSet<string>>();

    private object _sync = new object();


    public void AddTerm( TranslationTerm term )
    {
      AddTerm( term.SourceTerm, term.TranslatedTerm );
    }


    public void AddTerm( string sourceTerm, string[] results )
    {
      foreach ( var r in results )
      {
        AddTerm( sourceTerm, r );
      }
    }

    public void AddTerm( string sourceTerm, string result )
    {
      lock ( _sync )
      {
        HashSet<string> resultTerms;
        if ( !_dictionary.TryGetValue( sourceTerm, out resultTerms ) )
        {
          resultTerms = new HashSet<string>();
          _dictionary.Add( sourceTerm, resultTerms );
        }

        resultTerms.Add( result );
      }
    }

    public string[] this[string term]
    {
      get
      {
        lock ( _sync )
        {
          HashSet<string> results;
          if ( !_dictionary.TryGetValue( term, out results ) )
            return new string[0];

          return results.ToArray();
        }
      }
    }

    public ICollection<string> SourceTerms
    {
      get { return _dictionary.Keys; }
    }


    public static TranslateDictionary Merge( TranslateDictionary dictionary1, TranslateDictionary dictionary2 )
    {

      var dictionary = new TranslateDictionary();

      if ( dictionary1 != null )
      {
        foreach ( var term in dictionary1.SourceTerms )
          dictionary.AddTerm( term, dictionary1[term] );
      }

      if ( dictionary2 != null )
      {
        foreach ( var term in dictionary2.SourceTerms )
          dictionary.AddTerm( term, dictionary2[term] );
      }

      return dictionary;
    }


  }

  public class CandidateTranslations
  {

    internal CandidateTranslations( string term )
    {
      Term = term;

      _translated = new HashSet<string>();
    }

    private HashSet<string> _translated = new HashSet<string>();

    public string Term
    {
      get;
      private set;
    }

    public string[] TranslatedTerms
    {
      get { return _translated.ToArray(); }
    }
  }

}
