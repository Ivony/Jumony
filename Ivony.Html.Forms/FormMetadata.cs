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


    public FormMetadata( HtmlForm form, IFormMetadataProvider provider )
    {
      Form = form;
      MetadataProvider = provider;
    }


    public HtmlForm Form
    {
      get;
      private set;
    }


    protected IFormMetadataProvider MetadataProvider
    {
      get;
      private set;
    }




    private string formhash;

    private FormFieldMetadata[] fieldMetadata;

    public FormFieldMetadata[] GetFieldMetadata()
    {
      lock ( Form.SyncRoot )
      {

        if ( fieldMetadata == null || formhash != string.Join( ",", Form.Controls.ControlNames ) )
          fieldMetadata = Form.Controls.ControlNames.Select( field => MetadataProvider.GetFieldMetadata( field ) ).ToArray();


        return fieldMetadata;
      }
    }


    private FormValidator formValidator;

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
