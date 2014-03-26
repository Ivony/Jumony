using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public class DataAnnotationMeatadataProvider : IFormMetadataProvider
  {



    public DataAnnotationMeatadataProvider( Type modelType )
    {
      ModelType = modelType;
    }

    protected Type ModelType
    {
      get;
      private set;
    }


    private ICustomTypeDescriptor _typeDescriptor;

    protected virtual ICustomTypeDescriptor GetTypeDescriptor()
    {
      return new AssociatedMetadataTypeTypeDescriptionProvider( ModelType ).GetTypeDescriptor( ModelType );
    }


    protected ICustomTypeDescriptor TypeDescrptor
    {
      get
      {
        if ( _typeDescriptor == null )
          _typeDescriptor = GetTypeDescriptor();

        return _typeDescriptor;
      }
    }


    public FormFieldMetadata GetFieldMetadata( string name )
    {

      
      throw new NotImplementedException();
    }


    protected FormFieldMetadata GetFieldMetadata( PropertyDescriptor property )
    {
      throw new NotImplementedException();
    }



  }
}
