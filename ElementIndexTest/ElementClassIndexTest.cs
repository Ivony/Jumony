using Ivony.Html.Indexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Ivony.Html;
using Ivony.Html.Parser;
using System.Linq;

namespace ElementIndexTest
{


  /// <summary>
  ///这是 ElementClassIndexTest 的测试类，旨在
  ///包含所有 ElementClassIndexTest 单元测试
  ///</summary>
  [TestClass()]
  public class ElementClassIndexTest
  {


    private TestContext testContextInstance;

    /// <summary>
    ///获取或设置测试上下文，上下文提供
    ///有关当前测试运行及其功能的信息。
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region 附加测试特性
    // 
    //编写测试时，还可使用以下特性:
    //
    //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //使用 TestInitialize 在运行每个测试前先运行代码
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //使用 TestCleanup 在运行完每个测试后运行代码
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///ElementClassIndex 构造函数 的测试
    ///</summary>
    [TestMethod()]
    public void TestBuildIndex()
    {
      IHtmlDocument document = CreateDocument();
      ElementClassIndex index = new ElementClassIndex( document );

      index.Rebuild();

      Assert.IsTrue( index.ClassNames.Any(), "索引为空" );

      foreach ( var className in index.ClassNames )
      {
        var set1 = document.Find( "." + className );
        var set2 = index[className];

        Assert.AreEqual( set1.Intersect( set2 ).Count(), set1.Count(), "对于样式类 \"{0}\" 索引结果和选择器结果不一致", className );
        TestContext.WriteLine( "样式类 \"{0}\" 检测成功，共有 \"{1}\" 个元素", className, set1.Count() );
      }

    }

    private IHtmlDocument CreateDocument()
    {
      return new JumonyParser().LoadDocument( "http://www.qq.com/" );
    }

  }
}
