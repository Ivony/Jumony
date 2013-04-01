using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Parser;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace HtmlTranslator
{
  public class TranslateTask : IHtmlRenderAdapter
  {
    private IHtmlDocument _document;
    private string _documentPath;


    public TranslateTask( IHtmlDocument document )
    {
      _document = document;
    }

    public void Initialize()
    {
      var uri = _document.DocumentUri;
      if ( !uri.IsFile )
        throw new InvalidOperationException();


      _documentPath = uri.LocalPath;

      {
        var path = _documentPath + ".dictionary";
        Dictionary = LoadDictionary( path ) ?? new TranslateDictionary();
      }


      {
        var path = _documentPath + ".translation";

        var terms = LoadTerms( path );

        if ( !ValidateTerms( _document, terms ) )
        {
          Dictionary = TranslateDictionary.Merge( Dictionary, CreateDictionary( terms ) );
          terms = ExtractTerms( _document );

          foreach ( var t in terms )
          {
            var condidateTerms = Dictionary[t.SourceTerm];

            string translatedResult;
            if ( condidateTerms.IsSingle( out translatedResult ) )
              t.TranslatedTerm = translatedResult;

            else
              t.TranslatedTerm = "";

          }

          SaveTerms( path, terms );
        }

        Terms = terms;
      }
    }

    public TranslationTerm[] Terms
    {
      get;
      private set;
    }



    public TranslateDictionary Dictionary
    {
      get;
      private set;
    }



    public static TranslateTask LoadTranslateTask( string filepath )
    {

      if ( filepath == null )
        throw new ArgumentNullException( "filepath" );

      if ( !File.Exists( filepath ) )
        throw new InvalidOperationException( "文件不存在" );


      var document = new JumonyParser().LoadDocument( File.OpenText( filepath ), new Uri( filepath ) );

      var task = new TranslateTask( document );

      task.Initialize();

      return task;

    }


    private static TranslationTerm[] ExtractTerms( IHtmlDocument document )
    {
      return document.DescendantNodes()
        .OfType<IHtmlTextNode>()
        .Where( IsTranslatable )
        .Select( t => new TranslationTerm( t ) )
        .ToArray();
    }


    private static TranslationTerm[] LoadTerms( string path )
    {
      if ( !File.Exists( path ) )
        return null;

      var serializer = new DataContractJsonSerializer( typeof( TranslationTerm[] ) );
      using ( var stream = File.OpenRead( path ) )
      {
        return serializer.ReadObject( stream ) as TranslationTerm[];
      }
    }


    private static void SaveTerms( string path, TranslationTerm[] terms )
    {
      using ( var stream = File.Create( path ) )
      {
        var serializer = new DataContractJsonSerializer( typeof( TranslationTerm[] ) );
        serializer.WriteObject( stream, terms );
      }
    }

    private static bool ValidateTerms( IHtmlDocument document, TranslationTerm[] terms )
    {
      if ( terms == null )
        return false;

      var terms2 = ExtractTerms( document );

      if ( terms.Length != terms2.Length )
        return false;

      for ( int i = 0; i < terms.Length; i++ )
      {
        if ( terms[i].SourceTerm != terms2[i].SourceTerm )
          return false;

        terms[i].TextNode = terms2[i].TextNode;
      }

      return true;

    }



    private static TranslateDictionary LoadDictionary( string path )
    {
      if ( !File.Exists( path ) )
        return null;

      var serializer = new BinaryFormatter();
      using ( var stream = File.OpenRead( path ) )
      {
        return serializer.Deserialize( stream ) as TranslateDictionary;
      }
    }

    private static void SaveDictionary( string path, TranslateDictionary dictionary )
    {
      using ( var stream = File.Create( path ) )
      {
        var serializer = new BinaryFormatter();
        serializer.Serialize( stream, dictionary );
      }
    }



    private static TranslateDictionary CreateDictionary( TranslationTerm[] terms )
    {

      if ( terms == null )
        return null;

      var dictionary = new TranslateDictionary();

      foreach ( var t in terms )
        dictionary.AddTerm( t );

      return dictionary;
    }



    public void Save()
    {

      {
        var path = _documentPath + ".translation";
        SaveTerms( path, Terms );
      }

      {
        var path = _documentPath + ".dictionary";
        SaveDictionary( path, Dictionary );
      }

    }



    public string Translate()
    {

      Save();


      var uri = _document.DocumentUri;
      if ( !uri.IsFile )
        throw new InvalidOperationException();

      var path = Path.ChangeExtension( uri.LocalPath, ".translated" ) + ".html";

      using ( var writer = File.CreateText( path ) )
      {
        _document.Render( writer, this );
      }

      return path;
    }




    private static bool IsTranslatable( IHtmlTextNode textNode )
    {
      if ( textNode.IsWhiteSpace() )
        return false;

      if ( textNode is IHtmlSpecial )
        return false;

      if ( textNode.Ancestors().Any( e => e.Name.EqualsIgnoreCase( "partial" ) || e.Name.EqualsIgnoreCase( "head" ) ) )
        return false;

      if ( textNode.Parent() != null && textNode.ElementTextMode() == TextMode.NonText )
        return false;

      return true;
    }

    bool IHtmlRenderAdapter.Render( IHtmlNode node, HtmlRenderContext context )
    {
      var term = Terms.FirstOrDefault( t => t.TextNode.Equals( node ) );
      if ( term != null )
      {
        context.Write( term.TranslatedTerm );
        return true;
      }

      return false;

    }
  }
}
