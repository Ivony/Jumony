using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms.Validation
{


  /// <summary>
  /// HTML 表单字段验证器，承担一个表单输入字段的验证，一个表单输入字段（即 IHtmlInputControl 对象）只能有一个验证器。
  /// </summary>
  public class HtmlFieldValidator
  {

    private IHtmlInputControl _input;
    private string _fieldName;
    private List<IHtmlValueValidator> _validators = new List<IHtmlValueValidator>();



    public HtmlFieldValidator( IHtmlInputControl inputControl, string fieldName, params IHtmlValueValidator[] validators )
    {
      _input = inputControl;
      _fieldName = fieldName;
      _validators.AddRange( validators );

      if ( _fieldName == null )
      {
        var inputElement =_input as IHtmlFocusableControl;
        _fieldName = inputElement.LabelText();

        if ( _fieldName == null )
          _fieldName = _input.Name;
      }
    }



    public IHtmlInputControl InputControl
    {
      get { return _input; }
    }

    public IList<IHtmlValueValidator> Validators
    {
      get { return _validators; }
    }


    private bool validated = false;
    private bool valid;
    private IHtmlValueValidator failedValidator;

    public void Validate()
    {
      if ( validated )
        throw new InvalidOperationException( "验证已经被执行，无法再次执行" );


      var form = _input.Form;
      if ( form.SubmittedValues == null )
        throw new InvalidOperationException( "表单尚未提交，无法进行验证" );

      var value = _input.SubmittedValue();

      failedValidator = _validators.FirstOrDefault( v => v.Validate( value ) == false );

      validated = true;

      if ( failedValidator == null )
        valid = true;
      else
        valid = false;

    }

    public bool IsValid
    {
      get
      {
        if ( !validated )
          throw new InvalidOperationException( "尚未执行验证" );

        return valid;
      }
    }

    public virtual void ShowFailedMessage( IHtmlElement messageContainer )
    {

      if ( IsValid )
        return;

      var message = failedValidator.ErrorMessage.Replace( "<fieldname>", _fieldName );

      messageContainer.InnerText( message );
    }
  }

}
