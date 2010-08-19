#define v2

#pragma warning disable 1591
#warning 1591 disabled

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections;
using Ivony.Data.SqlDom;

namespace Ivony.Data
{
  [Serializable]
  public class SqlDbUtility : DbUtility
  {

    string _connectionString;
    SqlConnection _connection;

    TransactionUtility _transaction;



    public SqlDbUtility( string connectionString ) : this( connectionString, false ) { }

    public SqlDbUtility( string connectionString, bool hideConnection )
    {
      _connectionString = connectionString;
      HideConnection = hideConnection;
    }

    private SqlConnection CreateConnection()
    {
      if ( _transaction != null )
        throw new NotSupportedException( "在事务中执行时，禁止创建新连接" );

      //这里不能关闭原来的连接，因为它有可能正在被DataReader所使用。
      return _connection = InternalCreateConnection();
    }

    protected virtual SqlConnection InternalCreateConnection()
    {
      return new SqlConnection( _connectionString );
    }

    protected override DbExpressionPaser GetExpressionParser()
    {
      return new ExpressionParser( null );
    }


    /// <summary>
    /// 执行命令，并返回DataReader对象，请注意数据库连接将在DataReader关闭的同时关闭。
    /// </summary>
    /// <param name="commandText">命令模版</param>
    /// <param name="parameters">参数列表</param>
    /// <returns></returns>
    public SqlDataReader ExecuteReader( string commandText, params object[] parameters )
    {


      IDbCommand command = null;

      try
      {
        return (SqlDataReader) DbUtility.ExecuteReader( command = CreateCommand( commandText, parameters ) );
      }
      catch ( System.Data.Common.DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }

    #region DbUtility实现

    protected override IDbCommand CreateCommand( bool createConnection )
    {

      if ( _transaction == null )
      {

        if ( createConnection )
          CreateConnection();

        SqlCommand command = _connection.CreateCommand();

        return command;
      }
      else
        return _transaction.CreateCommand();
    }

    protected override IDataAdapter CreateDataAdapter( IDbCommand selectCommand )
    {
      return new SqlDataAdapter( (SqlCommand) selectCommand );
    }

    protected override IDataParameter CreateParameter( string name, object value )
    {
      return new SqlParameter( name, value );
    }

    public static string GetParameterName( string name )
    {
      if ( name.StartsWith( "@" ) )
        return name;
      return "@" + name;
    }

    #endregion


    #region 存储过程支持

    private Dictionary<string, SqlParameter[]> spParametersCollection = new Dictionary<string, SqlParameter[]>();

    /// <summary>
    /// 根据存储过程名称，创建存储过程对象协助执行存储过程。
    /// </summary>
    /// <param name="name">存储过程名</param>
    /// <returns>存储过程对象</returns>
    public SqlStoredProcedure CreateStoredProcedure( string expression, params object[] parameters )
    {
      SqlCommand command = (SqlCommand) CreateCommand( AlwaysCreateConnection );

      string name;
      IDictionary<string, object> parameterDictionary = ParseStoredProcedureExpression( out name, expression, parameters );

      command.CommandText = name;
      command.CommandType = CommandType.StoredProcedure;

      if ( !spParametersCollection.ContainsKey( name ) )
      {
        DeriveParameters( command );

        SqlParameter[] parameterCollection = new SqlParameter[command.Parameters.Count];
        command.Parameters.CopyTo( parameterCollection, 0 );

        spParametersCollection[name] = CloneParameters( parameterCollection );
      }
      else
        command.Parameters.AddRange( CloneParameters( spParametersCollection[name] ) );

      SqlStoredProcedure storedProcedure = new SqlStoredProcedure( command, HideConnection );

      if ( parameterDictionary != null )
      {
        foreach ( KeyValuePair<string, object> pair in parameterDictionary )
          storedProcedure.SetValue( pair.Key, pair.Value );
      }

      return storedProcedure;

    }

    protected override StoredProcedureBase CreateStoredProcedureProtected( string name )
    {
      return CreateStoredProcedure( name );
    }


    internal void DeriveParameters( SqlCommand command )
    {
      if ( command.Connection.State == ConnectionState.Closed )
      {
        try
        {
          command.Connection.Open();
          SqlCommandBuilder.DeriveParameters( command );
        }
        finally
        {
          command.Connection.Close();
        }
      }
      else
        SqlCommandBuilder.DeriveParameters( command );
    }

    private SqlParameter[] CloneParameters( SqlParameter[] parameters )
    {
      SqlParameter[] clonedParamters = new SqlParameter[parameters.Length];

      for ( int i = 0; i < parameters.Length; i++ )
        clonedParamters[i] = (SqlParameter) ((ICloneable) parameters[i]).Clone();

      return clonedParamters;
    }

    #endregion



    /// <summary>
    /// 开启新事务
    /// </summary>
    /// <returns>支持事务的SqlDbUtility</returns>
    public ITransactionUtility BeginTransaction()
    {
      TransactionUtility transaction = new TransactionUtility( this );
      transaction.Begin();
      return transaction;
    }

    /// <summary>
    /// 创建一个事务
    /// </summary>
    /// <returns></returns>
    public override ITransactionUtility CreateTransaction()
    {
      return new TransactionUtility( this );
    }

    protected virtual SqlDbUtility InternalClone()
    {
      SqlDbUtility dbUtility = new SqlDbUtility( _connectionString, this.HideConnection );
      dbUtility.spParametersCollection = this.spParametersCollection;

      //复制参数
      dbUtility.AlwaysCreateConnection = this.AlwaysCreateConnection;
      dbUtility.AutoConvertNull = this.AutoConvertNull;
      dbUtility.ParserOptions = this.ParserOptions;

      return dbUtility;

    }

    protected SqlDbUtility Clone()
    {
      return InternalClone();
    }


    private class TransactionUtility : ITransactionUtility<SqlDbUtility>
    {
      private SqlDbUtility _dbUtility;
      private SqlConnection _connection;
      private SqlTransaction _transaction;
      private bool _disposed = false;

      public TransactionUtility( SqlDbUtility origin )
      {
        _dbUtility = origin.Clone();
        _dbUtility.AlwaysCreateConnection = false;
        _dbUtility._transaction = this;
      }

      public void Begin()
      {
        if ( _disposed )
          throw new InvalidOperationException();

        if ( _transaction == null )
        {
          _connection = new SqlConnection( _dbUtility._connectionString );
          _connection.Open();
          _transaction = _connection.BeginTransaction();
        }
      }

      public void Commit()
      {
        if ( _disposed )
          throw new InvalidOperationException();

        if ( _transaction == null )
          throw new InvalidOperationException();

        _transaction.Commit();
        _connection.Close();
        _disposed = true;
      }

      public void Rollback()
      {
        if ( _disposed )
          throw new InvalidOperationException();

        if ( _transaction == null )
          throw new InvalidOperationException();

        _transaction.Rollback();
        _connection.Close();
        _disposed = true;
      }

      public SqlDbUtility DbUtility
      {
        get { return _dbUtility; }
      }

      DbUtility ITransactionUtility.DbUtility
      {
        get { return DbUtility; }
      }

      public void Dispose()
      {

        if ( _connection != null )
          _connection.Dispose();

        if ( _transaction != null )
          _transaction.Dispose();

        _disposed = true;
      }


      internal SqlCommand CreateCommand()
      {
        if ( _disposed )
          throw new InvalidOperationException();

        if ( _transaction == null )
          Begin();


        SqlCommand command = _connection.CreateCommand();
        command.Transaction = _transaction;

        return command;
      }

    }

    public class ExpressionParser : DbExpressionPaser
    {

      public ExpressionParser( IDictionary options ) : base( options ) { }


      protected override string GetDataParameterName( string name )
      {
        return GetParameterName( name );
      }
    }


  }
}
