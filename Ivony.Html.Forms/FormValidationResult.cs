using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 实现 IFormValidationResult
  /// </summary>
  public class FormValidationResult : IFormValidationResult
  {

    /// <summary>
    /// 创建 FormValidationResult 对象
    /// </summary>
    /// <param name="validator">表单验证器</param>
    /// <param name="errors">验证错误信息</param>
    public FormValidationResult( FormValidator validator, IEnumerable<FormValidationError> errors )
    {

      if ( validator == null )
        throw new ArgumentNullException( "validator" );

      Validator = validator;
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
    /// 获取执行验证的验证器
    /// </summary>
    public IFormValidator Validator { get; private set; }


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
