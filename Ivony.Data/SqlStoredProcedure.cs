
#pragma warning disable 1591
#warning 1591 disabled

using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.SqlClient;

namespace Ivony.Data
{
  public class SqlStoredProcedure : StoredProcedureBase
  {

    private SqlCommand _command;

    internal SqlStoredProcedure( SqlCommand command, bool hideConnection )
      : base( hideConnection )
    {
      _command = command;
      AddReturnValueParameter( _command );
    }


    protected override System.Data.IDataParameter GetParameter( string name )
    {
      if ( !name.StartsWith( "@" ) )
        name = "@" + name;

      return (IDataParameter) InnerCommand.Parameters[name];
    }

    protected override IDataParameter CreateParameter( string name, object value )
    {
      return new SqlParameter( name, value );
    }

    public DataTable ExecuteData( DataSet dataset, string tablename )
    {
      return DbUtility.ExecuteData( new SqlDataAdapter( (SqlCommand) InnerCommand ), dataset, tablename );
    }

    public DataTable ExecuteData()
    {
      return ExecuteData( null, null );
    }

    protected override IDbCommand InnerCommand
    {
      get { return _command; }
    }

    protected virtual void AddReturnValueParameter( IDbCommand command )
    {
      if ( !command.Parameters.Contains( "@RETURN_VALUE" ) )
      {
        SqlParameter returnValueParameter = new SqlParameter( "@RETURN_VALUE", null );

        returnValueParameter.Direction = ParameterDirection.ReturnValue;
        command.Parameters.Add( returnValueParameter );
      }
    }

    public override int ReturnValue
    {
      get { return (int) this["RETURN_VALUE"]; }
    }

  }
}
