using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Forms.Validation;
using Ivony.Html.Forms;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 配合 ASP.NET MVC 验证模型使用的表单验证器
  /// </summary>
  public class MvcFormValidator : HtmlFormValidator
  {


    private ModelStateDictionary _modelStates;

    /// <summary>
    /// 创建 MvcFormValidator 的实例
    /// </summary>
    /// <param name="form">所针对的表单对象</param>
    /// <param name="modelStates">表单（模型）验证信息</param>
    public MvcFormValidator( HtmlForm form, ModelStateDictionary modelStates )
      : base( form )
    {
      _modelStates = modelStates;


      foreach ( var pair in _modelStates )
      {

        var input = form[pair.Key];
        if ( input == null )
          continue;

        var state = pair.Value;

        AddFieldValidator( new MvcFieldValidator( input, state ) );
      }

    }


    
    /// <summary>
    /// 配合 ASP.NET MVC 验证模型使用的字段验证器
    /// </summary>
    protected class MvcFieldValidator : IHtmlFieldValidator
    {

      ModelState _state;
      IHtmlInputControl _input;


      /// <summary>
      /// 创建 MvcFieldValidator 对象
      /// </summary>
      /// <param name="input"></param>
      /// <param name="state"></param>
      public MvcFieldValidator( IHtmlInputControl input, ModelState state )
      {
        _state = state;
        _input = input;
      }



      /// <summary>
      /// 获取验证失败的消息
      /// </summary>
      /// <returns></returns>
      public string[] FailedMessage()
      {
        return _state.Errors.Select( error => error.ErrorMessage ).ToArray();
      }


      /// <summary>
      /// 获取验证规则的描述
      /// </summary>
      /// <returns></returns>
      public string[] RuleDescription()
      {
        return new string[0];
      }


      /// <summary>
      /// 获取验证的输入控件
      /// </summary>
      public IHtmlInputControl InputControl
      {
        get { return _input; }
      }


      /// <summary>
      /// 确定是否验证通过
      /// </summary>
      public bool IsValid
      {
        get { return !_state.Errors.Any(); }
      }

      
      /// <summary>
      /// 执行字段验证
      /// </summary>
      public void Validate()
      {
        
      }
    }


  }
}
