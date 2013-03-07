using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace HtmlTranslator
{
  public class CandidateTermsConverter : IValueConverter
  {

    public CandidateTermsConverter()
    {

    }


    public TranslateTask Task
    {
      get;
      set;
    }


    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      var sourceTerm = value as string;
      if ( sourceTerm == null )
        return Enumerable.Empty<string>();

      return Task.Dictionary[sourceTerm];
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      throw new NotSupportedException();
    }
  }
}
