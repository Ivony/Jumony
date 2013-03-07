using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Html;
using Ivony.Html.Parser;
using System.IO;
using System.Linq;
using Ivony.Html.Parser.ContentModels;

namespace SpeficationTest
{
  [TestClass]
  public class Test1
  {



    [TestInitialize]
    public void TestInitialize()
    {
    }

    [TestMethod]
    public void SpeficationTest1()
    {
      //测试孤立的'<'能否被正确解析
      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "SpeficationTest1.html" ) );
      var element = document.FindSingle( "a" );//需要找到一个<a>元素
      Assert.AreEqual( element.InnerHtml(), "abc" );//并且内容是"abc"
      Assert.AreEqual( element.Attributes().Count(), 1 );//有且只有一个属性
      Assert.AreEqual( element.Attribute( "abc" ).AttributeValue, "abc" );//属性值为"abc"
      var textNode = document.Nodes().ElementAt( 0 ) as IHtmlTextNode;
      Assert.IsNotNull( textNode );
      Assert.IsTrue( textNode.HtmlText.Contains( '<' ) );//第一个文本节点包含了那个孤立的 '<'

    }

    [TestMethod]
    public void SpeficationTest2()
    {
      //测试各种属性表达式能否被正确解析
      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "SpeficationTest2.html" ) );
      var element = document.FindSingle( "A" );

      Assert.AreEqual( element.Attribute( "a" ).AttributeValue, "abc" );//双引号情况
      Assert.AreEqual( element.Attribute( "b" ).AttributeValue, "123" );//单引号情况
      Assert.AreEqual( element.Attribute( "c" ).AttributeValue, "d=x" );//
      Assert.IsNull( element.Attribute( "d" ) );//属性值前面有空白的情况
      Assert.AreEqual( element.Attribute( "e" ).AttributeValue, null );//没有等号的情况
      Assert.AreEqual( element.Attribute( "f" ).AttributeValue, "" );//标签末尾的情况

      element = document.FindSingle( "B" );
      Assert.AreEqual( element.Attribute( "a" ).AttributeValue, "abc" );//等号前有空格的情况
      Assert.AreEqual( element.Attribute( "b" ).AttributeValue, "" );//空属性情况
      Assert.AreEqual( element.Attribute( "c" ).AttributeValue, null );//无值属性在标签末尾的情况

    }

    [TestMethod]
    public void SpeficationTest3()
    {
      //测试各种异常标签是否不被识别
      var str = File.ReadAllText( Path.Combine( Environment.CurrentDirectory, "SpeficationTest3.html" ) );

      var contents = new JumonyReader( str ).EnumerateContent().ToArray();
      var tag = contents.OfType<HtmlBeginTag>().FirstOrDefault();
      Assert.IsNull( tag, string.Format( "找到不应当解析出来的标签 {0}", tag ) );

    }

    [TestMethod]
    public void SpeficationTest4()
    {
      //测试中文标签和中文属性是否能够被识别
      var str = File.ReadAllText( Path.Combine( Environment.CurrentDirectory, "SpeficationTest4.html" ) );

      var contents = new JumonyReader( str ).EnumerateContent().ToArray();
      var beginTags = contents.OfType<HtmlBeginTag>().ToArray();
      var endTags = contents.OfType<HtmlEndTag>().ToArray();

      Assert.IsTrue( beginTags.Length >= 1, "无法解析中文名称标签" );
      Assert.IsTrue( beginTags.Length == 2, "无法解析中文名称属性" );
      Assert.AreEqual( endTags.Length, 2, "无法解析中文名称结束标签" );

    }


    [TestMethod]
    public void SpeficationTest5()
    {
      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "SpeficationTest5.html" ) );

      //Assert.AreEqual( document.DocumentDeclaration, "<!DOCTYPE html>", "HTML 声明解析失败" );


      var specials = document.DescendantNodes().OfType<IHtmlSpecial>().ToArray();

      Assert.AreEqual( specials.Count(), 4, "特殊标签解析数量不对" );

    }


  }
}
