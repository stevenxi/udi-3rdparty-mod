using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.DbLog
{
    public class DbCommandWrapper : DbCommand
    {
        private readonly DbCommand _command;
        private readonly bool _ownCommand;

        public DbCommandWrapper(DbCommand command, bool ownCommand)
        {
            _command = command;
            _ownCommand = ownCommand;
        }

        #region No log

        public override void Cancel()
        {
            _command.Cancel();
        }

        public override string CommandText
        {
            get { return _command.CommandText; }
            set { _command.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get { return _command.CommandTimeout; }
            set { _command.CommandTimeout = value; }
        }

        public override System.Data.CommandType CommandType
        {
            get { return _command.CommandType; }
            set { _command.CommandType = value; }
        }

        protected override DbParameter CreateDbParameter()
        {
            return _command.CreateParameter();
        }

        protected override DbConnection DbConnection
        {
            get { return _command.Connection; }
            set { _command.Connection = value; }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _command.Parameters; }
        }

        protected override DbTransaction DbTransaction
        {
            get { return _command.Transaction; }
            set { _command.Transaction = value; }
        }

        public override bool DesignTimeVisible
        {
            get { return _command.DesignTimeVisible; }
            set { _command.DesignTimeVisible = value; }
        }

        public override System.Data.UpdateRowSource UpdatedRowSource
        {
            get { return _command.UpdatedRowSource; }
            set { _command.UpdatedRowSource = value; }
        }

        #endregion 
        

        public DbCommand OrigionalDbCommand { get { return _command; } }

        protected override DbDataReader ExecuteDbDataReader(System.Data.CommandBehavior behavior)
        {
            var context = new DbCommandInterceptionContext<DbDataReader>();
            DbLogger.LogFormatter.Value.ReaderExecuting(this, context);
            try
            {
                return context.Result = _command.ExecuteReader(behavior);
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                throw;
            }
            finally
            {
                DbLogger.LogFormatter.Value.ReaderExecuted(this, context);
            }
            
        }

        public override int ExecuteNonQuery()
        {
            var context = new DbCommandInterceptionContext<int>();
            DbLogger.LogFormatter.Value.NonQueryExecuting(this, context);
            try
            {
                return context.Result = _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                throw;
            }
            finally
            {
                DbLogger.LogFormatter.Value.NonQueryExecuted(this, context);
            }
        }

        public override object ExecuteScalar()
        {
            var context = new DbCommandInterceptionContext<object>();
            DbLogger.LogFormatter.Value.ScalarExecuting(this, context);
            try
            {
                return context.Result = _command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                throw;
            }
            finally
            {
                DbLogger.LogFormatter.Value.ScalarExecuted(this, context);
            }
            
        }

        public override void Prepare()
        {
            _command.Prepare();
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && _ownCommand)
            {
                _command.Dispose();
            }
        }

    }
}
