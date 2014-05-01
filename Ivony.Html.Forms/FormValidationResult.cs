using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// IFormValidationResult 的标准实现
  /// </summary>
  public class FormValidationResult : IFormValidationResult
  {

    /// <summary>
    /// 创建 FormValidationResult 对象
    /// </summary>
    /// <param name="form">所验证的表单</param>
    /// <param name="errors">验证错误信息</param>
    public FormValidationResult( HtmlForm form, IEnumerable<FormValidationError> errors )
    {

      if ( form == null )
        throw new ArgumentNullException( "form" );

      
      Form = form;

      Errors = new FormValidationErrorCollection();

      if ( errors != null )
      {
        errors = errors.NotNull();

        if ( errors.Any() )
        {
          HasError = true;
          foreach ( var e in errors )
            Errors.Add( e );
        }
      }

      else
        HasError = false;
    }



    /// <summary>
    /// 所验证的表单
    /// </summary>
    public HtmlForm Form
    {
      get;
      private set;
    }



    /// <summary>
    /// 是否存在验证错误
    /// </summary>
    public bool HasError { get; private set; }


    /// <summary>
    /// 验证错误信息
    /// </summary>
    public FormValidationErrorCollection Errors { get; private set; }


  }
}
