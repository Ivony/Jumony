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

      var expression = BindingExpression.ParseExpression( "{eval path=Name}" );
      Assert.AreEqual( expression.Name, "eval", "测试解析绑定表达式失败" );

      object value;
      Assert.IsTrue( expression.TryGetValue( evaluator, "path", out value ), "测试解析绑定表达式失败" );
      Assert.AreEqual( value, "Name", "测试解析绑定表达式失败" );


      expression = BindingExpression.ParseExpression( "{eval path=Name , key==@#$}" );
      Assert.AreEqual( expression.Name, "eval", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.TryGetValue( evaluator, "path", out value ), "测试解析绑定表达式失败" );
      Assert.AreEqual( value, "Name ", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.TryGetValue( evaluator, "key", out value ), "测试解析绑定表达式失败" );
      Assert.AreEqual( value, "=@#$", "测试解析绑定表达式失败" );

      expression = BindingExpression.ParseExpression( "{eval path={{abc,,}},key=abc}" );
      Assert.AreEqual( expression.Name, "eval", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.TryGetValue( evaluator, "path", out value ), "测试解析绑定表达式失败" );
      Assert.AreEqual( value, "{abc,}", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.TryGetValue( evaluator, "key", out value ), "测试解析绑定表达式失败" );
      Assert.AreEqual( value, "abc", "测试解析绑定表达式失败" );

      expression = BindingExpression.ParseExpression( "{eval path={},,key=abc}" );
      Assert.IsNull( expression, "错误的解析了错误的表达式" );

      expression = BindingExpression.ParseExpression( "{eval path={eval a=b, c=d},key=abc}" );
      Assert.AreEqual( expression.Name, "eval", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.TryGetValue( evaluator, "path", out value ), "测试解析绑定表达式失败" );
      Assert.AreEqual( value, "a:b,c:d", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.TryGetValue( evaluator, "key", out value ), "测试解析绑定表达式失败" );
      Assert.AreEqual( value, "abc", "测试解析绑定表达式失败" );


      expression = BindingExpression.ParseExpression( "{eval a, b=}" );
      Assert.AreEqual( expression.Name, "eval", "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.TryGetValue( evaluator, "a", out value ), "测试解析绑定表达式失败" );
      Assert.IsNull( value, "测试解析绑定表达式失败" );
      Assert.IsTrue( expression.TryGetValue( evaluator, "b", out value ), "测试解析绑定表达式失败" );
      Assert.AreEqual( value, "", "测试解析绑定表达式失败" );


    }

    private class TestEvaluator : IBindingExpressionEvaluator
    {
      public object GetValue( BindingExpression bindingExpression )
      {

        if ( bindingExpression == null )
          return null;

        var values = bindingExpression.GetValues( this );

        return string.Join( ",", values.Select( pair => pair.Key + ":" + pair.Value ) );
      }

      public bool TryConvertValue<T>( object obj, out T value )
      {
        value = (T) obj;
        return true;
      }
    }



  }
}
