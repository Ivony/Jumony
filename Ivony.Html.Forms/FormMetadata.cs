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


    public IFormValidator GetFormValidator()
    {
      throw new NotImplementedException();
    }

    
  }
}
