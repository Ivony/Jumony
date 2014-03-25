using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public interface IFormMetadataProvider
  {

    FormFieldMetadata GetFieldMetadata( string name );
  }
}
