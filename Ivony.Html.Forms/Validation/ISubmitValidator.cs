using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Ivony.Html.Forms.Validation
{
  public interface ISubmitValidator
  {

    bool isValid( NameValueCollection submitData );

  }
}
