using System;
namespace Ivony.Html.Styles
{
  interface ICssStyle
  {
    string Get( string name );
    ICssStyle Set( string name, string value );
  }
}
