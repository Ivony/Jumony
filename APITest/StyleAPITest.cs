using System;
using System.Linq;
using Ivony.Html;
using Ivony.Html.Styles;
using Ivony.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APITest
{
  [TestClass]
  public class StyleAPITest
  {
    [TestMethod]
    public void SetStyleTest()
    {

      var element = new JumonyParser().Parse( "<div></div>" ).Elements().First();

      element.Style( "display", "none" );
      Assert.AreEqual( element.Attribute( "style" ).Value(), "display:none", ".Style( name, value ) 测试不通过" );

      element.Style().SetValue( "color", "red" );
      Assert.AreEqual( element.Attribute( "style" ).Value(), "display:none;color:red", ".Style().SetValue( name, value ) 测试不通过" );

      element.Style().SetValue( "display", "block" );
      Assert.AreEqual( element.Attribute( "style" ).Value(), "display:block;color:red", ".Style().SetValue( name, value ) 测试不通过" );

      element.Style().SetValue( "display", null );
      Assert.AreEqual( element.Attribute( "style" ).Value(), "color:red", ".Style().SetValue( name, null ) 测试不通过" );

      element.Style().Clear();
      Assert.AreEqual( element.Attribute( "style" ).Value(), "", ".Style().Clear() 测试不通过" );


      element.Style().SetValue( "padding", "10px" );
      Assert.AreEqual( element.Style().GetValue( "padding-left" ), "10px", "shorthand 展开测试不通过" );

      element.Style().SetValue( "padding-left", "0px" );
      Assert.AreEqual( element.Style().GetValue( "padding-left" ), "0px", "shorthand 展开测试不通过" );
      Assert.AreEqual( element.Style().GetValue( "padding-top" ), "10px", "shorthand 展开测试不通过" );

      element.Style().SetValue( "margin", "5px" );
      Assert.AreEqual( element.Style().GetValue( "margin-left" ), "5px", "margin shorthand 展开测试不通过" );

    }

    [TestMethod]
    public void SetClassTest()
    {
      var element = new JumonyParser().Parse( "<div></div>" ).Elements().First();

      element.Class( "test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "test", ".Class( name ) 测试不通过" );

      element.Class( "-test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "", ".Class( -name ) 测试不通过" );

      element.Class( "~test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "test", ".Class( ~name ) 测试不通过" );

      element.Class( "~test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "", ".Class( ~name ) 测试不通过" );

      element.Class( "~test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "test", ".Class( ~name ) 测试不通过" );

      element.Class().Toggle( "test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "", ".Class().Toggle( name ) 测试不通过" );

      element.Class().Toggle( "test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "test", ".Class().Toggle( name ) 测试不通过" );

      element.Class().Toggle( "test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "", ".Class().Toggle( name ) 测试不通过" );

      element.Class( "+deleted", "+completed" );
      Assert.IsTrue( CssParser.Create( element.Document, ".deleted.completed" ).IsEligible( element ), ".Class( +name, +name )" );

      element.Class( "+deleted", "~completed" );
      Assert.IsFalse( CssParser.Create( element.Document, ".deleted.completed" ).IsEligible( element ), ".Class( +name, ~name )" );
      Assert.IsTrue( CssParser.Create( element.Document, ".deleted" ).IsEligible( element ), ".Class( +name, ~name )" );

      element.Class( "~deleted", "~completed" );
      Assert.IsFalse( CssParser.Create( element.Document, ".deleted.completed" ).IsEligible( element ), ".Class( ~name, ~name )" );
      Assert.IsTrue( CssParser.Create( element.Document, ".completed" ).IsEligible( element ), ".Class( ~name, ~name )" );

    }

  }
}
