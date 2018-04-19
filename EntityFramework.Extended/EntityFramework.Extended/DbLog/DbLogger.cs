using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.DbLog
{
    public static class DbLogger
    {
        public static readonly Lazy<DatabaseLogFormatter> LogFormatter = new Lazy<DatabaseLogFormatter>(() =>
            DbConfiguration.DependencyResolver.GetService<Func<DbContext, Action<string>, DatabaseLogFormatter>>()(null, s => { }));

        public static DbCommandWrapper CreateDbCommandWrapper(this DbCommand command)
        {
            return new DbCommandWrapper(command, false);
        }


        public static DbCommandWrapper CreateDbCommandWrapper(this DbConnection command)
        {
            return new DbCommandWrapper(command.CreateCommand(), true);
        }

        

    }
}
