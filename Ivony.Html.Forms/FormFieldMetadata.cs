using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义表单字段元数据
  /// </summary>
  public abstract class FormFieldMetadata
  {

    /// <summary>
    /// 字段名称
    /// </summary>
    public abstract string Name { get; }


    /// <summary>
    /// 字段显示名称
    /// </summary>
    public abstract string DisplayName { get; }


    /// <summary>
    /// 字段描述
    /// </summary>
    public abstract string FieldDescription { get; }

    /// <summary>
    /// 填写规则描述
    /// </summary>
    public abstract IFormValidationRule[] Rules { get; }

    /// <summary>
    /// 获取验证器
    /// </summary>
    /// <returns>字段验证器</returns>
    public IFormFieldValidator GetValidator()
    {
      return new FormFieldMeatadataValidator( this );
    }


    private class FormFieldMeatadataValidator : IFormFieldValidator
    {

      public FormFieldMeatadataValidator( FormFieldMetadata metadata )
      {
        Metadata = metadata;
      }

      public FormFieldMetadata Metadata
      {
        get;
        private set;
      }

      public string Name
      {
        get { return Metadata.Name; }
      }

      public FormValidationError Validate( string value )
      {
        var messages = Metadata.Rules.Where( rule => !rule.IsValid( value ) ).Select( rule => string.Format( CultureInfo.InvariantCulture, rule.ErrorMessageTemplate, Metadata.DisplayName ) ).ToArray();

        if ( messages.Any() )
          return new FormValidationError( Name, messages );

        else
          return null;


      }
    }





    /// <summary>
    /// 创建表单字段元数据
    /// </summary>
    /// <param name="fieldName">字段名称</param>
    /// <param name="displayName">字段显示名称</param>
    /// <param name="description">字段规则描述</param>
    /// <param name="rules">字段验证规则</param>
    /// <returns>表单字段元数据</returns>
    public static FormFieldMetadata CreateMetaData( string fieldName, string displayName, string description, IFormValidationRule[] rules )
    {
      return new GenericFormFieldMetadata( fieldName, displayName, description, rules );
    }

    private class GenericFormFieldMetadata : FormFieldMetadata
    {


      private string _fieldName;
      private string _displayName;
      private string _description;
      private IFormValidationRule[] _rules;


      public GenericFormFieldMetadata( string fieldName, string displayName, string description, IFormValidationRule[] rules )
      {
        _fieldName = fieldName;
        _displayName = displayName;
        _description = description;
        _rules = rules;
      }


      public override string Name
      {
        get { return _fieldName; }
      }

      public override string DisplayName
      {
        get { return _displayName; }
      }

      public override string FieldDescription
      {
        get { return _description; }
      }

      public override IFormValidationRule[] Rules
      {
        get { return _rules; }
      }

    }

  }
}
