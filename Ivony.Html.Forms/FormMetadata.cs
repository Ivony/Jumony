using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// 定义表单的元数据
  /// </summary>
  public class FormMetadata
  {


    /// <summary>
    /// 通过表单元数据提供程序，针对指定表单创建 FormMeatadata 对象
    /// </summary>
    /// <param name="form">要创建元数据的表单</param>
    /// <param name="provider">表单元数据提供程序</param>
    public FormMetadata( HtmlForm form, IFormMetadataProvider provider )
    {
      Form = form;
      MetadataProvider = provider;
    }


    /// <summary>
    /// 所属表单
    /// </summary>
    public HtmlForm Form
    {
      get;
      private set;
    }



    /// <summary>
    /// 表单元数据提供程序
    /// </summary>
    protected IFormMetadataProvider MetadataProvider
    {
      get;
      private set;
    }




    private string formhash;

    private FormFieldMetadata[] fieldMetadata;


    /// <summary>
    /// 获取所有字段的元数据
    /// </summary>
    /// <returns>所有字段的元数据</returns>
    protected FormFieldMetadata[] GetFieldMetadata()
    {
      lock ( Form.SyncRoot )
      {
        var hash = string.Join( ",", Form.Controls.ControlNames );//将所有控件名称串起来作为表单的特征字符串

        if ( fieldMetadata == null || formhash != hash )
        {
          formhash = hash;
          fieldMetadata = Form.Controls.ControlNames.Select( field => MetadataProvider.GetFieldMetadata( field ) ).ToArray();

          formValidator = null;//由于字段元数据改变，原有的表单验证器已经过时。
        }


        return fieldMetadata;
      }
    }


    private FormValidator formValidator;

    /// <summary>
    /// 根据元数据获取表单验证器
    /// </summary>
    /// <returns>表单验证器</returns>
    public IFormValidator GetFormValidator()
    {

      lock ( Form.SyncRoot )
      {
        if ( formValidator == null )
        {
          var validators = new FormFieldValidatorCollection( GetFieldMetadata().Select( metadata => metadata.GetValidator() ) );
          formValidator = new FormValidator( validators );
        }
        return formValidator;
      }
    }


  }
}
