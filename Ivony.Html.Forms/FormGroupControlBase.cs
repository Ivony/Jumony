using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 为下拉列表、单选复选框等表单控件组提供基类型
  /// </summary>
  public abstract class FormGroupControlBase : FormControl
  {


    /// <summary>
    /// 创建表单控件组的实例
    /// </summary>
    /// <param name="form">所属表单</param>
    protected FormGroupControlBase( HtmlForm form )
      : base( form )
    {

    }

    /// <summary>
    /// 候选值，即可以被设置的值
    /// </summary>
    public abstract string[] CandidateValues
    {
      get;
    }


    /// <summary>
    /// 指示输入组控件是否允许多选
    /// </summary>
    public abstract bool AllowMultiple
    {
      get;
    }


    /// <summary>
    /// 获取目前所设置的值
    /// </summary>
    public abstract string[] Values
    {
      get;
    }



    /// <summary>
    /// 实现 CanSetValue 方法，检查要设置的值是否存在于候选值列表中
    /// </summary>
    /// <param name="value">所要设置的值</param>
    /// <returns>是否可以设置</returns>
    public override bool CanSetValue( string value )
    {
      var values = value.Split( ',' );
      return values.All( v => CandidateValues.Contains( v ) );
    }


    /// <summary>
    /// 获取目前所设置的值的字符串表达形式（多个值以逗号分隔）。
    /// </summary>
    public override string Value
    {
      get { return string.Join( ",", Values ); }
      set { SetValue( value ); }
    }


    /// <summary>
    /// 设置控件的值
    /// </summary>
    /// <param name="valuesExpression">值表达式，可以是单个值，也可以是多个用逗号分隔的值</param>
    protected virtual void SetValue( string valuesExpression )
    {

      valuesExpression = valuesExpression ?? "";

      var values = valuesExpression.Split( ',' );


      var invalidValue = values.Except( CandidateValues );

      if ( invalidValue.Any() )//如果有一个设置的值不在候选值列表
      {
        if ( !Form.Configuration.IgnoreInvailidValuesInGroupControl )
          throw new InvalidOperationException( string.Format( "不能对控件设置值 \"{0}\"", invalidValue.First() ) );
      }

      SetValues( new HashSet<string>( values.Except( invalidValue ) ) );
    }

    /// <summary>
    /// 设置控件值
    /// </summary>
    /// <param name="values">要设置的值列表</param>
    protected abstract void SetValues( HashSet<string> values );
  }
}
