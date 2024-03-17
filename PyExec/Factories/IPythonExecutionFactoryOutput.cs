using PFDB.PythonExecution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PFDB
{
    namespace PythonFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public interface IPythonExecutionFactoryOutput
        {

            /// <summary>
            /// The list of internal <see cref="IPythonExecutor"/> objects.
            /// </summary>
            IEnumerable<IPythonExecutor> PythonExecutors { get; }

            /// <summary>
            /// The counter for the number of items that pass or fail <see cref="PythonExecutionFactory{TPythonExecutable}.CheckFactory"/>.
            /// </summary>
            StatusCounter CheckStatusCounter { get; }

            /// <summary>
            /// The counter for the number of items that pass or fail being queued via <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)"/>.
            /// </summary>
            StatusCounter QueueStatusCounter { get; }

            /// <summary>
            /// The counter for the number of items that pass or fail during execution from <see cref="IPythonExecutor.Execute(object?)"/>
            /// </summary>
            StatusCounter ExecutionStatusCounter { get; }

            /// <summary>
            /// Total parallel execution time of the entire factory (across all threads). Calculated with <see cref="DateTime"/>.
            /// </summary>
            TimeSpan TotalParallelExecutionTimeFromDateTime { get; }

            /// <summary>
            /// Total parallel execution time of the entire factory (across all threads) in milliseconds. Calculated with <see cref="Stopwatch"/>.
            /// </summary>
            long TotalParallelExecutionTimeFromStopwatchInMilliseconds { get; }

            /// <summary>
            /// Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="DateTime"/>.
            /// </summary>
            TimeSpan ActualExecutionTimeFromDateTime { get; }

            /// <summary>
            /// Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="Stopwatch"/>.
            /// </summary>
            long ActualExecutionTimeFromStopwatchInMilliseconds { get; }
        }
    }
}
