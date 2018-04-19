using System;
using System.Collections.Generic;
using System.Data.Entity;
using EntityFramework.MappingAPI.Mappings;

using System.Data.Entity.Infrastructure;
namespace EntityFramework.MappingAPI
{
    /// <summary>
    /// 
    /// </summary>
    internal class EfMap
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Dictionary<string, DbMapping> Mappings = new Dictionary<string, DbMapping>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEntityMap<T> Get<T>(DbContext context)
        {
            return (IEntityMap<T>)Get(context)[typeof(T)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEntityMap Get(DbContext context, Type type)
        {
            return Get(context)[type];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEntityMap Get(DbContext context, string typeFullName)
        {
            return Get(context)[typeFullName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DbMapping Get(DbContext context)
        {
            var cackeKey = context.GetType().FullName;
            lock (Mappings)
            {
	            var iDbModelCacheKeyProvider = context as IDbModelCacheKeyProvider;
	            if (iDbModelCacheKeyProvider != null)
	            {
	                cackeKey = iDbModelCacheKeyProvider.CacheKey;
	            }
	            if (Mappings.ContainsKey(cackeKey))
	            {
	                return Mappings[cackeKey];
	            }

	            var mapping = new DbMapping(context);
	            //var mapping = Map(context);

	            Mappings[cackeKey] = mapping;
	            return mapping;
            }
        }
    }
}