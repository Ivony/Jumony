using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web;
using System.CodeDom;
using System.Text.RegularExpressions;
using Ivony.Html.Parser;

namespace Ivony.Html.Web
{

  public class HtmlBuildProvider : BuildProvider
  {

    protected override CodeCompileUnit GetCodeCompileUnit( out System.Collections.IDictionary linePragmasTable )
    {
      linePragmasTable = null;


      if ( _type == null )
      {
        _type = CreateDocumentProviderType();
      }

      var @namespace = new CodeNamespace( "HTML" );
      @namespace.Types.Add( _type );

      var unit = new CodeCompileUnit();
      unit.Namespaces.Add( @namespace );

      return unit;

    }


    public override Type GetGeneratedType( System.CodeDom.Compiler.CompilerResults results )
    {
      var typeName = CreateName();
      return results.CompiledAssembly.GetType( typeName );
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

      var createDomProvider = new CodeObjectCreateExpression( new CodeTypeReference( typeof( DomProvider ) ) );
      var invokeCreateDocument = new CodeMethodInvokeExpression( new CodeMethodReferenceExpression( new CodeThisReferenceExpression(), "CreateDocument" ), createDomProvider );
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
