using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// IFormPresenter 的一个标准实现
  /// </summary>
  public class FormPresenter : IFormPresenter
  {
    public void ShowValidationResult( IFormValidationResult result )
    {

      if ( !result.HasError )//若没有验证失败的消息，则什么也不做
        return;


      foreach ( var error in result.Errors )
        ShowError( error );


      if ( RemoveErrorMessageContainer )
      {
        foreach ( var fieldName in result.Form.Controls.ControlNames.Except( result.Errors.Select( e => e.Name ) ) )
          RemoveErrorMessage( fieldName );
      }


      ShowErrorSummary( result.Errors );

    }

    /// <summary>
    /// 显示表单验证的错误的摘要信息
    /// </summary>
    /// <param name="result"></param>
    protected void ShowErrorSummary( FormValidationErrorCollection errors )
    {
    }



    /// <summary>
    /// 派生类重写此属性指示当某字段没有错误信息时，是否移除该错误信息呈现容器。
    /// </summary>
    protected virtual bool RemoveErrorMessageContainer
    {
      get { return false; }
    }

    /// <summary>
    /// 显示字段验证错误信息
    /// </summary>
    /// <param name="error">错误信息</param>
    protected virtual void ShowError( FormValidationError error )
    {
      var container = FindErrorMessageContainer( error.Name );

      ShowErrorMessage( container, error.Messages );
    }

    /// <summary>
    /// 找到指定字段的错误信息显示容器
    /// </summary>
    /// <param name="fieldName">字段名称</param>
    /// <returns>错误信息显示容器</returns>
    protected virtual IHtmlElement FindErrorMessageContainer( string fieldName )
    {
      throw new NotImplementedException();
    }


    /// <summary>
    /// 在指定元素呈现字段的所有错误信息
    /// </summary>
    /// <param name="container">要呈现错误信息的元素</param>
    /// <param name="messages">错误信息</param>
    protected virtual void ShowErrorMessage( IHtmlElement container, string[] messages )
    {
      var list = container.AddElement( "ul" );

      foreach ( var m in messages )
        list.AddElement( "li" ).InnerText( m );
    }


    /// <summary>
    /// 移除没有错误信息的字段的错误信息呈现容器
    /// </summary>
    /// <param name="fieldName">字段名</param>
    protected virtual void RemoveErrorMessage( string fieldName )
    {
      FindErrorMessageContainer( fieldName ).Remove();
    }


    /// <summary>
    /// 呈现表单的元数据
    /// </summary>
    /// <param name="metadata">表单元数据</param>
    public void ShowMetadata( FormMetadata metadata )
    {
      throw new NotImplementedException();
    }
  }
}
