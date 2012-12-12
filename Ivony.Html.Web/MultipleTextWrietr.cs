using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ivony.Html.Web
{
  class MultipleTextWriter : TextWriter
  {
    public override Encoding Encoding
    {
      get { throw new NotImplementedException(); }
    }
  }
}
