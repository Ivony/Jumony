using Ivony.Html.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Fluent;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 将 MVC 框架的 ModelState 包装为 IFormValidationReult 对象的实现类。
  /// </summary>
  public sealed class MvcFormValidationResult : IFormValidationResult
  {

    /// <summary>
    /// 创建 MvcFormValidationResult 对象
    /// </summary>
    /// <param name="form">所验证的表单</param>
    /// <param name="modelStates">要包装的模型验证状态</param>
    public MvcFormValidationResult( HtmlForm form, ModelStateDictionary modelStates )
    {
      if ( modelStates == null )
        throw new ArgumentNullException( "modelState" );

      Errors = new FormValidationErrorCollection();
      Form = form;


      modelStates.Select( CreateValidationError ).NotNull().ForAll( e => Errors.Add( e ) );

      if ( Errors.Any() )
        HasError = true;
    }

    private FormValidationError CreateValidationError( KeyValuePair<string, ModelState> modelState )
    {
      if ( modelState.Value.Errors.Any() )
        return new FormValidationError( modelState.Key, modelState.Value.Errors.Select( e => e.ErrorMessage ).ToArray() );

      else
        return null;
    }


    /// <summary>
    /// 是否存在表单验证错误
    /// </summary>
    public bool HasError
    {
      get;
      private set;
    }


    /// <summary>
    /// 表单验证错误
    /// </summary>
    public FormValidationErrorCollection Errors
    {
      get;
      private set;
    }

    /// <summary>
    /// 所验证的表单
    /// </summary>
    public HtmlForm Form
    {
      get;
      private set;
    }
  }
}
