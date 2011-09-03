using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms.Validation
{


  public interface IHtmlFieldValidator
  {
    string[] FailedMessage();
    IHtmlInputControl InputControl { get; }
    bool IsValid { get; }
    string[] RuleDescription();
    void Validate();
  }



  /// <summary>
  /// HTML 表单字段验证器，承担一个表单输入字段的验证，一个表单输入字段（即 IHtmlInputControl 对象）只能有一个验证器。
  /// </summary>
  public abstract class HtmlFieldValidator : IHtmlFieldValidator
  {
    private readonly IHtmlInputControl _input;
    private readonly string _fieldName;

    public IHtmlInputControl InputControl
    {
      get { return _input; }
    }

    protected string FieldName
    {
      get { return _fieldName; }
    }



    protected HtmlFieldValidator( IHtmlInputControl inputControl, string fieldName )
    {
      _input = inputControl;
      _fieldName = fieldName;
    }


    private bool validated = false;
    private bool valid;


    public bool IsValid
    {
      get
      {
        if ( !validated )
          throw new InvalidOperationException( "尚未执行验证" );

        return valid;
      }
    }

    public virtual void Validate()
    {

      if ( validated )
        throw new InvalidOperationException( "验证已经被执行，无法再次执行" );


      var form = InputControl.Form;
      if ( form.SubmittedValues == null )
        throw new InvalidOperationException( "表单尚未提交，无法进行验证" );


      var value = InputControl.SubmittedValue();

      valid = ExecuteValidate( value );
      validated = true;
    }

    protected abstract bool ExecuteValidate( string value );

    public abstract string[] FailedMessage();

    public abstract string[] RuleDescription();

  }

}
