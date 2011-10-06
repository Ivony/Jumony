using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web;
using System.CodeDom;
using System.Text.RegularExpressions;
using Ivony.Html.Parser;
using System.Collections;

namespace Ivony.Html.Web
{

  public class HtmlBuildProvider : BuildProvider
  {


    public override void GenerateCode( AssemblyBuilder assemblyBuilder )
    {

      if ( _type == null )
      {
        _type = CreateDocumentProviderType();
      }

      var @namespace = new CodeNamespace( "HTML" );
      @namespace.Types.Add( _type );

      var unit = new CodeCompileUnit();
      unit.Namespaces.Add( @namespace );

      assemblyBuilder.AddCodeCompileUnit( this, unit );
      assemblyBuilder.AddAssemblyReference( GetType().Assembly );
      assemblyBuilder.AddAssemblyReference( typeof( DomProvider ).Assembly );

    }

    public override CompilerType CodeCompilerType
    {
      get
      {
        return GetDefaultCompilerTypeForLanguage( "C#" );
      }
    }


    public override Type GetGeneratedType( System.CodeDom.Compiler.CompilerResults results )
    {
      var typeName = CreateName();
      return results.CompiledAssembly.GetType( "HTML." + typeName );
    }


    private CodeTypeDeclaration _type;

    private CodeTypeDeclaration CreateDocumentProviderType()
    {
      var parser = new WebParser();
      IHtmlDocument document;
      using ( var reader = OpenReader() )
      {
        document = parser.Parse( reader.ReadToEnd() );
      }


      var name = CreateName();
      var type = new CodeTypeDeclaration( name );
      type.BaseTypes.Add( new CodeTypeReference( typeof( IHtmlDocumentProvider ) ) );

      type.Members.Add( document.GenerateCodeMethod( "CreateDocument" ) );

      var createDocument = new CodeMemberMethod();
      createDocument.Name = "CreateDocument";
      createDocument.ReturnType = new CodeTypeReference( typeof( IHtmlDocument ) );

      var createDomProvider = new CodeObjectCreateExpression( new CodeTypeReference( typeof( DomProvider ) ) );
      var invokeCreateDocument = new CodeMethodInvokeExpression( new CodeMethodReferenceExpression( null, "CreateDocument" ), createDomProvider );
      createDocument.Statements.Add( new CodeMethodReturnStatement( invokeCreateDocument ) );

      createDocument.PrivateImplementationType = new CodeTypeReference( typeof( IHtmlDocumentProvider ) );

      type.Members.Add( createDocument );
      return type;
    }


    private string CreateName()
    {
      return Regex.Replace( VirtualPath, @"\W", "_" );
    }

  }
}
