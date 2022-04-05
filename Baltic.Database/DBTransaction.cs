using System;
using System.Collections.Generic;
using System.Data.Common;
using Mighty;

namespace Baltic.Database
{
    public class DBTransaction : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbTransaction _internalTransaction;

        private DBTransaction()
        {
            var db = new MightyOrm(MightyOrm.GlobalConnectionString);
            _connection = db.OpenConnection();
            _internalTransaction = _connection.BeginTransaction();
        }

        public static DBTransaction BeginTransaction()
        {
            return new DBTransaction();
        }

        public void CommitTransaction()
        {
            _internalTransaction.Commit();
        }

        public void RollbackTransaction()
        {
            _internalTransaction.Rollback();
        }

        public IEnumerable<dynamic> Insert(MightyOrm table, IEnumerable<object> items)
        {
            return table.Insert(_connection, items);
        }

        public IEnumerable<dynamic> Insert(MightyOrm table, params object[] items)
        {
            return table.Insert(_connection, items);
        }

        public int Update(MightyOrm table, IEnumerable<object> items)
        {
            return table.Update(_connection, items);
        }

        public int Update(MightyOrm table, params object[] items)
        {
            return table.Update(_connection, items);
        }

        public void Dispose()
        {
            _internalTransaction?.Dispose();            
            _connection?.Dispose();
        }

        public int Delete(MightyOrm table, object[] items)
        {
            return table.Delete(_connection, items);
        }
    }
}