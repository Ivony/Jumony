using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public abstract class HtmlSpecificationBase
  {

    public bool IsCData( string name, IDictionary<string, string> attributes );

    public bool IsInline( string name, IDictionary<string, string> attributes );
    
    public bool IsBlock( string name, IDictionary<string, string> attributes );
    
    public bool IsSelfClose( string name, IDictionary<string, string> attributes );
    
    public bool IsOptionalClose( string name, IDictionary<string, string> attributes );
    
    public bool IsPhrase( string name, IDictionary<string, string> attributes );


  }
}
