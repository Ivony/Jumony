using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Parser;
using System.Runtime.Serialization.Json;

namespace HtmlTranslator
{
  public class TranslateTask : IHtmlAdapter
  {
    private   IHtmlDocument document;

    public TranslateTask( IHtmlDocument document, TranslationTerm[] terms )
    {
      this.document = document;
      Terms = terms;
    }

    public static TranslateTask LoadTranslateTask( string filepath )
    {

      if ( filepath == null )
        throw new ArgumentNullException( "filepath" );

      if ( !File.Exists( filepath ) )
        throw new InvalidOperationException( "文件不存在" );


      var document = new JumonyParser().LoadDocument( File.OpenText( filepath ), new Uri( filepath ) );

      var terms = EnsureTermsData( document );

      return new TranslateTask( document, terms );

    }

    private static TranslationTerm[] EnsureTermsData( IHtmlDocument document )
    {
      var uri = document.DocumentUri;
      if ( !uri.IsFile )
        throw new InvalidOperationException();

      var path = uri.LocalPath + ".translation";




      var terms = LoadTerms( path );

      if ( !ValidateTerms( document, terms ) )
        return CreateTermsData( document, path );

      return terms;
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

    private static TranslationTerm[] CreateTermsData( IHtmlDocument document, string path )
    {
      var terms = ExtractTerms( document );


      Save( path, terms );

      return terms;
    }

    private static TranslationTerm[] ExtractTerms( IHtmlDocument document )
    {
      return document.DescendantNodes()
        .OfType<IHtmlTextNode>()
        .Where( IsTranslatable )
        .Select( t => new TranslationTerm( t ) )
        .ToArray();
    }



    public TranslationTerm[] Terms
    {
      get;
      private set;
    }


    public void Save()
    {
      var uri = document.DocumentUri;
      if ( !uri.IsFile )
        throw new InvalidOperationException();

      var path = uri.LocalPath + ".translation";

      Save( path, Terms );
    }


    private static void Save( string path, TranslationTerm[] terms )
    {
      using ( var stream = File.Create( path ) )
      {
        var serializer = new DataContractJsonSerializer( typeof( TranslationTerm[] ) );
        serializer.WriteObject( stream, terms );
      }
    }



    public string Translate()
    {

      Save();


      var uri = document.DocumentUri;
      if ( !uri.IsFile )
        throw new InvalidOperationException();

      var path = uri.LocalPath + ".translated";

      using ( var writer = File.CreateText( path ) )
      {
        document.Render( writer, this );
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

      if ( textNode.Parent() != null && HtmlSpecification.nonTextElements.Contains( textNode.Parent().Name, StringComparer.OrdinalIgnoreCase ) )
        return false;

      return true;
    }

    bool IHtmlAdapter.Render( IHtmlNode node, TextWriter writer )
    {
      var term = Terms.FirstOrDefault( t => t.TextNode.Equals( node ) );
      if ( term != null )
      {
        writer.Write( term.TranslatedTerm );
        return true;
      }

      return false;

    }
  }
}
