using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Data;

namespace Ivony.Html.Binding
{
  public class PagingDataSource<T> : IPagingDataSource<T>
  {





    #region IPagingDataSource<T> 成员

    public IPagingData<T> CreatePaging( int pageSize )
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IPagingDataSource 成员

    IPagingData IPagingDataSource.CreatePaging( int pageSize )
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
