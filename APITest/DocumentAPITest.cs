using System;
using System.Linq;
using Ivony.Html;
using Ivony.Html.Styles;
using Ivony.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace APITest
{

  [TestClass]
  public class DocumentAPITest
  {

    [TestMethod]
    public void CompileTest()
    {
      var parser = new JumonyParser();
      var document = parser.LoadDocument( Path.Combine( Environment.CurrentDirectory, "Test1.html" ) );

      var method = document.Compile();

      var document2 = method( parser.DomProvider );

      Assert.IsTrue( document.DescendantNodes().SequenceEqual( document2.DescendantNodes(), new DomNodeComparer() ), "编译还原测试失败" );

    }

    [TestMethod]
    public void GenerateCodeTest()
    {
      var document = new JumonyParser().LoadDocument( "http://www.cnblogs.com" );

      var method = document.GenerateCodeMethod( "CreateDocument" );

      var unit = new CodeCompileUnit();
      unit.ReferencedAssemblies.Add( "Ivony.Html" );
      unit.ReferencedAssemblies.Add( "System" );
      var ns = new CodeNamespace( "TEST" );
      unit.Namespaces.Add( ns );

      var builder = new CodeTypeDeclaration( "HtmlBuilder" );
      ns.Types.Add( builder );

      builder.Members.Add( method );

      var provider = CodeDomProvider.CreateProvider( "C#" );
      using ( var writer = new StringWriter() )
      {
        provider.GenerateCodeFromCompileUnit( unit, writer, new CodeGeneratorOptions() );
        var compileOptions =  new CompilerParameters();
        compileOptions.ReferencedAssemblies.Add( Path.Combine( Environment.CurrentDirectory, "Ivony.Html.dll" ) );
        compileOptions.ReferencedAssemblies.Add( "System.dll" );
        var result = provider.CompileAssemblyFromSource( compileOptions, writer.ToString() );

        Assert.AreEqual( result.NativeCompilerReturnValue, 0, string.Join( "\n", result.Errors.Cast<CompilerError>().Select( e => e.ErrorText ) ) );
      }




    }


    private class DomNodeComparer : IEqualityComparer<IHtmlNode>
    {
      public bool Equals( IHtmlNode x, IHtmlNode y )
      {
        if ( x.GetType() != y.GetType() )
          return false;

        if ( x.OuterHtml() != y.OuterHtml() )
          return false;

        return true;

      }

      public int GetHashCode( IHtmlNode obj )
      {
        return obj.OuterHtml().GetHashCode();
      }
    }

  }
}
