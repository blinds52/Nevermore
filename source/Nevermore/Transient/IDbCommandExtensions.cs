using System;
using System.Data;
using Microsoft.WindowsAzure.Common.TransientFaultHandling;

namespace Nevermore.Transient
{
    // ReSharper disable once InconsistentNaming
    static class IDbCommandExtensions
    {
        public static int ExecuteNonQueryWithRetry(this IDbCommand command, string operationName = "ExcecuteNonQuery")
        {
            var commandPolicy = RetryManager.Instance.GetDefaultSqlCommandRetryPolicy();
            return command.ExecuteNonQueryWithRetry(commandPolicy);
        }

        public static int ExecuteNonQueryWithRetry(this IDbCommand command, RetryPolicy retryPolicy, string operationName = "ExcecuteNonQuery")
        {
            return command.ExecuteNonQueryWithRetry(retryPolicy, RetryPolicy.NoRetry);
        }

        public static int ExecuteNonQueryWithRetry(this IDbCommand command, RetryPolicy commandRetryPolicy, RetryPolicy connectionRetryPolicy, string operationName = "ExcecuteNonQuery")
        {
            GuardConnectionIsNotNull(command);
            var effectiveCommandRetryPolicy = (commandRetryPolicy ?? RetryPolicy.NoRetry).LoggingRetries(operationName);
            return effectiveCommandRetryPolicy.ExecuteAction(() =>
            {
                var weOwnTheConnectionLifetime = EnsureValidConnection(command, connectionRetryPolicy);
                try
                {
                    return command.ExecuteNonQuery();
                }
                finally
                {
                    if (weOwnTheConnectionLifetime && command.Connection != null && command.Connection.State == ConnectionState.Open)
                        command.Connection.Close();
                }
            });
        }

        public static IDataReader ExecuteReaderWithRetry(this IDbCommand command, string operationName = "ExecuteReader")
        {
            return command.ExecuteReaderWithRetry(RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        public static IDataReader ExecuteReaderWithRetry(this IDbCommand command, RetryPolicy retryPolicy, string operationName = "ExecuteReader")
        {
            return command.ExecuteReaderWithRetry(retryPolicy, RetryPolicy.NoRetry);
        }

        public static IDataReader ExecuteReaderWithRetry(this IDbCommand command, RetryPolicy commandRetryPolicy, RetryPolicy connectionRetryPolicy, string operationName = "ExecuteReader")
        {
            GuardConnectionIsNotNull(command);
            var effectiveCommandRetryPolicy = (commandRetryPolicy ?? RetryPolicy.NoRetry).LoggingRetries(operationName);
            return effectiveCommandRetryPolicy.ExecuteAction(() =>
            {
                var weOwnTheConnectionLifetime = EnsureValidConnection(command, connectionRetryPolicy);
                try
                {
                    return command.ExecuteReader();
                }
                catch (Exception)
                {
                    if (weOwnTheConnectionLifetime && command.Connection != null && command.Connection.State == ConnectionState.Open)
                        command.Connection.Close();
                    throw;
                }
            });
        }

        public static IDataReader ExecuteReaderWithRetry(this IDbCommand command, CommandBehavior behavior, string operationName = "ExecuteReader")
        {
            return command.ExecuteReaderWithRetry(behavior, RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        public static IDataReader ExecuteReaderWithRetry(this IDbCommand command, CommandBehavior behavior, RetryPolicy retryPolicy, string operationName = "ExecuteReader")
        {
            return command.ExecuteReaderWithRetry(behavior, retryPolicy, RetryPolicy.NoRetry);
        }

        public static IDataReader ExecuteReaderWithRetry(this IDbCommand command, CommandBehavior behavior, RetryPolicy commandRetryPolicy, RetryPolicy connectionRetryPolicy, string operationName = "ExecuteReader")
        {
            GuardConnectionIsNotNull(command);
            var effectiveCommandRetryPolicy = (commandRetryPolicy ?? RetryPolicy.NoRetry).LoggingRetries(operationName);
            return effectiveCommandRetryPolicy.ExecuteAction(() =>
            {
                var weOwnTheConnectionLifetime = EnsureValidConnection(command, connectionRetryPolicy);
                try
                {
                    return command.ExecuteReader(behavior);
                }
                catch (Exception)
                {
                    if (weOwnTheConnectionLifetime && command.Connection != null && command.Connection.State == ConnectionState.Open)
                        command.Connection.Close();
                    throw;
                }
            });
        }

        public static object ExecuteScalarWithRetry(this IDbCommand command, string operationName = "ExecuteScalar")
        {
            return command.ExecuteScalarWithRetry(RetryManager.Instance.GetDefaultSqlCommandRetryPolicy());
        }

        public static object ExecuteScalarWithRetry(this IDbCommand command, RetryPolicy retryPolicy, string operationName = "ExecuteScalar")
        {
            return ExecuteScalarWithRetry(command, retryPolicy, RetryPolicy.NoRetry);
        }

        public static object ExecuteScalarWithRetry(this IDbCommand command, RetryPolicy commandRetryPolicy, RetryPolicy connectionRetryPolicy, string operationName = "ExecuteScalar")
        {
            GuardConnectionIsNotNull(command);
            var effectiveCommandRetryPolicy = (commandRetryPolicy ?? RetryPolicy.NoRetry).LoggingRetries(operationName);
            return effectiveCommandRetryPolicy.ExecuteAction(() =>
            {
                var weOwnTheConnectionLifetime = EnsureValidConnection(command, connectionRetryPolicy);
                try
                {
                    return command.ExecuteScalar();
                }
                finally
                {
                    if (weOwnTheConnectionLifetime && command.Connection != null && command.Connection.State == ConnectionState.Open)
                        command.Connection.Close();
                }
            });
        }

        // ReSharper disable once UnusedParameter.Local
        private static void GuardConnectionIsNotNull(IDbCommand command)
        {
            if (command.Connection == null)
                throw new InvalidOperationException("Connection property has not been initialized.");
        }

        /// <summary>
        /// Ensures the command either has an existing open connection, or we will open one for it.
        /// </summary>
        /// <returns>True if we opened the connection (indicating we own its lifetime), False if the connection was already open (indicating someone else owns its lifetime)</returns>
        private static bool EnsureValidConnection(IDbCommand command, RetryPolicy retryPolicy)
        {
            if (command == null) return false;

            GuardConnectionIsNotNull(command);

            if (command.Connection.State == ConnectionState.Open) return false;

            command.Connection.OpenWithRetry(retryPolicy);
            return true;
        }
    }
}