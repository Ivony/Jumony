using System;
using System.Linq;
using Ivony.Html;
using Ivony.Html.Styles;
using Ivony.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
      Assert.AreEqual( element.Attribute( "class" ).Value() ?? "", "", ".Class( -name ) 测试不通过" );

      element.Class( "~test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "test", ".Class( ~name ) 测试不通过" );

      element.Class( "~test" );
      Assert.AreEqual( element.Attribute( "class" ).Value() ?? "", "", ".Class( ~name ) 测试不通过" );

      element.Class( "~test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "test", ".Class( ~name ) 测试不通过" );

      element.Class().Toggle( "test" );
      Assert.AreEqual( element.Attribute( "class" ).Value() ?? "", "", ".Class().Toggle( name ) 测试不通过" );

      element.Class().Toggle( "test" );
      Assert.AreEqual( element.Attribute( "class" ).Value(), "test", ".Class().Toggle( name ) 测试不通过" );

      element.Class().Toggle( "test" );
      Assert.AreEqual( element.Attribute( "class" ).Value() ?? "", "", ".Class().Toggle( name ) 测试不通过" );

      element.Class( "+deleted", "+completed" );//class="deleted completed"
      Assert.IsTrue( CssParser.Create( element.Document, ".deleted.completed" ).IsEligible( element ), ".Class( +name, +name )" );

      element.Class( "+deleted", "~completed" );//class="deleted"
      Assert.IsFalse( CssParser.Create( element.Document, ".deleted.completed" ).IsEligible( element ), ".Class( +name, ~name )" );
      Assert.IsTrue( CssParser.Create( element.Document, ".deleted" ).IsEligible( element ), ".Class( +name, ~name )" );

      element.Class( "~deleted", "~completed" );//class="completed"
      Assert.IsFalse( CssParser.Create( element.Document, ".deleted.completed" ).IsEligible( element ), ".Class( ~name, ~name )" );
      Assert.IsTrue( CssParser.Create( element.Document, ".completed" ).IsEligible( element ), ".Class( ~name, ~name )" );

      element.Class( "~deleted ~completed" );//class="deleted"
      Assert.IsFalse( CssParser.Create( element.Document, ".deleted.completed" ).IsEligible( element ), ".Class( ~name ~name )" );
      Assert.IsTrue( CssParser.Create( element.Document, ".deleted" ).IsEligible( element ), ".Class( ~name ~name )" );

      element.Class( "deleted completed" );//class="deleted completed"
      Assert.IsTrue( CssParser.Create( element.Document, ".deleted.completed" ).IsEligible( element ), ".Class( name name )" );

      element.Class( "+deleted ~completed" );//class="deleted"
      Assert.IsFalse( CssParser.Create( element.Document, ".deleted.completed" ).IsEligible( element ), ".Class( +name, ~name )" );
      Assert.IsTrue( CssParser.Create( element.Document, ".deleted" ).IsEligible( element ), ".Class( +name, ~name )" );

    }

    [TestMethod]
    public void StyleParseTest()
    {
      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "CssStyleSettingTest1.html" ) );

      Assert.AreEqual( document.FindFirst( "div" ).Style().GetValue( "display" ), "none", "无法正确解析丢失分号的属性值" );
      Assert.AreEqual( document.FindFirst( "a" ).Style().GetValue( "display" ), null, "无法正确解析没有style属性的元素" );
      Assert.AreEqual( document.FindFirst( "p" ).Style().GetValue( "width" ), "12px", "无法正确解析空白样式设置值后随的正确表达式" );
      Assert.AreEqual( document.FindFirst( "p" ).Style().GetValue( "display" ), null, "无法正确解析空白样式设置值" );

      var element = document.FindFirst( "div#test1" );
      Assert.AreEqual( element.Style().GetValue( "display" ), "none", "CSS 属性设置不应区分大小写" );
    }

  }
}
