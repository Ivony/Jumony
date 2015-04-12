using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  internal interface IBindingExpressionValueObject
  {

    object GetValue( IBindingExpressionEvaluator evaluator );

  }

  internal class LiteralValue : IBindingExpressionValueObject
  {

    private string literalValue;

    public LiteralValue( string literal )
    {
      literalValue = literal;
    }


    object IBindingExpressionValueObject.GetValue( IBindingExpressionEvaluator evaluator )
    {
      return literalValue;
    }

    public override string ToString()
    {
      return literalValue;
    }
  }

}
