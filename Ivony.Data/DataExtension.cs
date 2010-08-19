using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace Ivony.Data
{
  public static class DataExtension
  {

    public static T IfNull<T>( this T value, T defaultValue )
    {
      if ( value == null || Convert.IsDBNull( value ) )
        return defaultValue;
      else
        return value;
    }




  }
}
