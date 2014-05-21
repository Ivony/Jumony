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
    /// 按照逗号拆分控件值表达式
    /// </summary>
    /// <param name="valuesExpression">要拆分的值表达式</param>
    /// <returns>拆分后的值集合</returns>
    protected static HashSet<string> SplitValues( string valuesExpression )
    {
      return new HashSet<string>( valuesExpression.Split( ',' ) );
    }




    /// <summary>
    /// 实现 CanSetValue 方法，检查要设置的值是否存在于候选值列表中
    /// </summary>
    /// <param name="value">所要设置的值</param>
    /// <param name="message">若不能设置，获取错误信息</param>
    /// <returns>是否可以设置</returns>
    protected override bool CanSetValue( string value, out string message )
    {
      return CanSetValues( SplitValues( value ), out message );
    }


    /// <summary>
    /// 检查要设置的值是否存在于候选值列表中
    /// </summary>
    /// <param name="values">所要设置的值</param>
    /// <param name="message">若不能设置，获取错误信息</param>
    /// <returns>是否可以设置</returns>
    protected virtual bool CanSetValues( HashSet<string> values, out string message )
    {
      var invalidValue = values.Except( CandidateValues ).FirstOrDefault();

      if ( invalidValue != null && !Form.Configuration.IgnoreInvailidValuesInGroupControl )//如果有一个设置的值不在候选值列表
      {
        message = string.Format( "不能对控件设置值 \"{0}\"", invalidValue.First() );
        return false;
      }

      message = null;
      return true;
    }


    /// <summary>
    /// 设置控件的值
    /// </summary>
    /// <param name="valuesExpression">值表达式，可以是单个值，也可以是多个用逗号分隔的值</param>
    protected override void SetValue( string valuesExpression )
    {
      SetValues( SplitValues( valuesExpression ) );
    }


    /// <summary>
    /// 设置控件值
    /// </summary>
    /// <param name="values">要设置的值列表</param>
    protected abstract void SetValues( HashSet<string> values );





    /// <summary>
    /// 获取目前所设置的值
    /// </summary>
    /// <returns></returns>
    protected override string GetValue()
    {
      return string.Join( ",", GetValues() );
    }


    /// <summary>
    /// 获取目前所设置的值
    /// </summary>
    protected abstract string[] GetValues();



    /// <summary>
    /// 获取控件目前所设置的值列表
    /// </summary>
    public string[] Values
    {
      get { return GetValues(); }
      set
      {
        var values = new HashSet<string>( value );
        string message;
        if ( !CanSetValues( values, out message ) )
          throw new FormValueFormatException( this, message );

        SetValues( values );
      }
    }


  }
}
