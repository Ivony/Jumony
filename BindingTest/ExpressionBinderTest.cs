using Ivony.Html;
using Ivony.Html.Binding;
using Ivony.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;

namespace BindingTest
{
  [TestClass]
  public class ExpressionBinderTest
  {
    [TestMethod]
    public void AttributeTest1()
    {
      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "Test1.html" ) );

      var dataValues = new Dictionary<string, object>() { 
        { "StyleClass", "Test" },
        { "ThisTime", new DateTime( 2000,1,2 ) },
        { "ScriptValue1", null }
      };

      HtmlBinding.Create( document, dataValues ).DataBind();

      Assert.AreEqual( document.FindFirst( "body" ).Attribute( "class" ).Value(), "Test", "针对属性的表达式绑定不成功" );
      Assert.AreEqual( document.FindFirst( "body" ).Attribute( "test" ).Value(), "this time is 2000-01-02 #", "格式表达式测试失败" );


    }
  }
}
