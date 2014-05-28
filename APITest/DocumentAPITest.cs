using System;
using System.Linq;
using Ivony.Html;
using Ivony.Html.Styles;
using Ivony.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

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
