using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 为标准的表单控件组提供基础实现
  /// </summary>
  public abstract class FormGroupControl : FormGroupControlBase
  {


    /// <summary>
    /// 创建标准表单控件组实例
    /// </summary>
    /// <param name="form">控件所属表单</param>
    protected FormGroupControl( HtmlForm form ) : base( form ) { }


    /// <summary>
    /// 实现 CandidateValues 方法，获取可以设置的候选值
    /// </summary>
    public override string[] CandidateValues
    {
      get { return Items.Select( i => i.Value ).ToArray(); }
    }


    /// <summary>
    /// 实现 Values 属性，获取当前所设置的值
    /// </summary>
    protected override string[] GetValues()
    {
      return Items.Where( i => i.Selected ).Select( i => i.Value ).ToArray();
    }


    /// <summary>
    /// 获取输入组项
    /// </summary>
    protected abstract FormGroupControlItem[] Items
    {
      get;
    }



    /// <summary>
    /// 获取具有指定值的项
    /// </summary>
    /// <param name="value">指定的值</param>
    /// <returns>具有这个值的输入项</returns>
    protected FormGroupControlItem this[string value]
    {
      get { return Items.FirstOrDefault( o => o.Value == value ); }
    }



    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="values">要设置的值列表</param>
    protected override void SetValues( HashSet<string> values )
    {
      Items.ForAll( i => i.Selected = values.Contains( i.Value ) );
    }

  }
}
