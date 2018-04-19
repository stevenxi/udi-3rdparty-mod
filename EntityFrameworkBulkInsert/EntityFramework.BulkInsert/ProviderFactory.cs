using System;
using System.Collections.Generic;
using System.Data.Entity;
using EntityFramework.BulkInsert.Exceptions;
using EntityFramework.BulkInsert.Providers;

namespace EntityFramework.BulkInsert
{
    public static class ProviderFactory
    {
        static ProviderFactory()
        {
            _providers = new Dictionary<string, Func<IEfBulkInsertProvider>>();
            Register<EfSqlBulkInsertProviderWithMappedDataReader>("System.Data.SqlClient.SqlConnection");
        }

        private static readonly Dictionary<string, Func<IEfBulkInsertProvider>> _providers;

        /// <summary>
        /// Register new bulkinsert provider.
        /// Rplaces existing if name matches.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public static void Register<T>(string name) where T : IEfBulkInsertProvider, new()
        {
            _providers[name] = () => new T();
        }

        /*
        public static void Register(Type type, string name)
        {
            // todo - check if type is IEfBulkInsertProvider

            var body = Expression.New(type);

            Expression<Func<IEfBulkInsertProvider>> ex = Expression.Lambda<Func<IEfBulkInsertProvider>>(body);
            var f = ex.Compile();

            Providers[name] = f;
        }
        */

        /// <summary>
        /// Get bulkinsert porvider by connection used in context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEfBulkInsertProvider Get(DbContext context)
        {
            var connectionTypeName = context.Database.Connection.GetType().FullName;
            if (!_providers.ContainsKey(connectionTypeName))
                throw new BulkInsertProviderNotFoundException(connectionTypeName);
            return _providers[connectionTypeName]().SetContext(context);
        }
    }
}
