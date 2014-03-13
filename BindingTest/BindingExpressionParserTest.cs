using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Html.Binding;
using System.Linq;

namespace BindingTest
{
  [TestClass]
  public class BindingExpressionParserTest
  {
    [TestMethod]
    public void Test1()
    {

      TestEvaluator evaluator = new TestEvaluator();

      var expression = BindingExpression.ParseExpression( evaluator, "{eval path=Name}" );
      Assert.AreEqual( expression.Name, "eval", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.Arguments.ContainsKey( "path" ), "测试解析绑定表达式失败" );
      Assert.AreEqual( expression.Arguments["path"], "Name", "测试解析绑定表达式失败" );


      expression = BindingExpression.ParseExpression( evaluator, "{eval path=Name , key==@#$}" );
      Assert.AreEqual( expression.Name, "eval", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.Arguments.ContainsKey( "path" ), "测试解析绑定表达式失败" );
      Assert.AreEqual( expression.Arguments["path"], "Name ", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.Arguments.ContainsKey( "key" ), "测试解析绑定表达式失败" );
      Assert.AreEqual( expression.Arguments["key"], "=@#$", "测试解析绑定表达式失败" );

      expression = BindingExpression.ParseExpression( evaluator, "{eval path={{abc,,}},key=abc}" );
      Assert.AreEqual( expression.Name, "eval", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.Arguments.ContainsKey( "path" ), "测试解析绑定表达式失败" );
      Assert.AreEqual( expression.Arguments["path"], "{abc,}", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.Arguments.ContainsKey( "key" ), "测试解析绑定表达式失败" );
      Assert.AreEqual( expression.Arguments["key"], "abc", "测试解析绑定表达式失败" );

      expression = BindingExpression.ParseExpression( evaluator, "{eval path={},,key=abc}" );
      Assert.IsNull( expression, "错误的解析了错误的表达式" );

      expression = BindingExpression.ParseExpression( evaluator, "{eval path={eval a=b, c=d},key=abc}" );
      Assert.AreEqual( expression.Name, "eval", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.Arguments.ContainsKey( "path" ), "测试解析绑定表达式失败" );
      Assert.AreEqual( expression.Arguments["path"], "a:b,c:d", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.Arguments.ContainsKey( "key" ), "测试解析绑定表达式失败" );
      Assert.AreEqual( expression.Arguments["key"], "abc", "测试解析绑定表达式失败" );


    }

    private class TestEvaluator : IBindingExpressionEvaluator
    {
      public string GetValue( BindingExpression bindingExpression )
      {

        if ( bindingExpression == null )
          return null;

        return string.Join( ",", bindingExpression.Arguments.Select( p => p.Key + ":" + p.Value ) );
      }
    }

  }
}
