﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using EntityFramework.BulkInsert.Extensions;
using EntityFramework.BulkInsert.Helpers;
using Microsoft.SqlServer.Types;

namespace EntityFramework.BulkInsert.Providers
{
    public class EfSqlBulkInsertProviderWithMappedDataReader : ProviderBase<SqlConnection, SqlTransaction>
    {
 
        public override void Run<T>(IEnumerable<T> entities, SqlConnection connection, SqlTransaction transaction)
        {
            var keepIdentity = (SqlBulkCopyOptions.KeepIdentity & Options.SqlBulkCopyOptions) > 0;
            using (var reader = new MappedDataReader<T>(entities, this))
            {
                using (var sqlBulkCopy = new SqlBulkCopy(connection, Options.SqlBulkCopyOptions, transaction))
                {
                    sqlBulkCopy.BulkCopyTimeout = Options.TimeOut;
                    sqlBulkCopy.BatchSize = Options.BatchSize;
                    sqlBulkCopy.DestinationTableName = string.Format("[{0}].[{1}]", reader.SchemaName, reader.TableName);
                    sqlBulkCopy.EnableStreaming = Options.EnableStreaming;

                    sqlBulkCopy.NotifyAfter = Options.NotifyAfter;
                    if (Options.Callback != null)
                    {
                        sqlBulkCopy.SqlRowsCopied += Options.Callback;
                    }

                    foreach (var kvp in reader.Cols)
                    {
                        if (kvp.Value.IsIdentity && !keepIdentity)
                        {
                            continue;
                        }
                        sqlBulkCopy.ColumnMappings.Add(kvp.Value.ColumnName, kvp.Value.ColumnName);
                    }

                    sqlBulkCopy.WriteToServer(reader);
                }
            }
        }


        /// <summary>
        /// Get sql grography object from well known text
        /// </summary>
        /// <param name="wkt">Well known text representation of the value</param>
        /// <param name="srid">The identifier associated with the coordinate system.</param>
        /// <returns></returns>
        public override object GetSqlGeography(string wkt, int srid)
        {
            var chars = new SqlChars(wkt);
            return SqlGeography.STGeomFromText(chars, srid);
        }

        /// <summary>
        /// Get sql geometry object from well known text
        /// </summary>
        /// <param name="wkt">Well known text representation of the value</param>
        /// <param name="srid">The identifier associated with the coordinate system.</param>
        /// <returns></returns>
        public override object GetSqlGeometry(string wkt, int srid)
        {
            var chars = new SqlChars(wkt);
            return SqlGeometry.STGeomFromText(chars, srid);
        }


        /// <summary>
        /// Create new sql connection
        /// </summary>
        /// <returns></returns>
        protected override SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}