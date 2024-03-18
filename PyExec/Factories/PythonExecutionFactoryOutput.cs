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
		/// <typeparam name="TPythonExecutable"></typeparam>
		public sealed class PythonExecutionFactoryOutput<TPythonExecutable> : IPythonExecutionFactoryOutput where TPythonExecutable : IPythonExecutable<IOutput>, new()
		{
			/// <summary>
			/// The list of internal <see cref="IPythonExecutor"/> objects.
			/// </summary>
			public IEnumerable<IPythonExecutor> PythonExecutors { get;  }

			/// <summary>
			/// The counter for the number of items that pass or fail <see cref="PythonExecutionFactory{TPythonExecutable}.CheckFactory"/>.
			/// </summary>
			public StatusCounter CheckStatusCounter { get; }

			/// <summary>
			/// The counter for the number of items that pass or fail being queued via <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)"/>.
			/// </summary>
			public StatusCounter QueueStatusCounter { get; }

			/// <summary>
			/// The counter for the number of items that pass or fail during execution from <see cref="IPythonExecutor.Execute(object?)"/>.
			/// </summary>
			public StatusCounter ExecutionStatusCounter { get; }

			/// <summary>
			/// Total parallel execution time of the entire factory (across all threads). Calculated with <see cref="DateTime"/>.
			/// </summary>
			public TimeSpan TotalParallelExecutionTimeFromDateTime { get; }

            /// <summary>
            /// Total parallel execution time of the entire factory (across all threads) in milliseconds. Calculated with <see cref="Stopwatch"/>.
            /// </summary>
            public long TotalParallelExecutionTimeFromStopwatchInMilliseconds { get; }


            /// <summary>
            /// Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="DateTime"/>.
            /// </summary>
            public TimeSpan ActualExecutionTimeFromDateTime { get; }

            /// <summary>
            /// Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="Stopwatch"/>.
            /// </summary>
            public long ActualExecutionTimeFromStopwatchInMilliseconds { get; }

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="pythonExecutors">The list of internal <see cref="IPythonExecutor"/> objects.</param>
            /// <param name="checkStatusCounter">The counter for the number of items that pass or fail <see cref="PythonExecutionFactory{TPythonExecutable}.CheckFactory"/>.</param>
            /// <param name="queueStatusCounter">The counter for the number of items that pass or fail being queued via <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)"/>.</param>
            /// <param name="executionStatusCounter">The counter for the number of items that pass or fail during execution from <see cref="IPythonExecutor.Execute(object?)"/>.</param>
            /// <param name="totalParallelExecutionTimeFromDateTime">Total parallel execution time of the entire factory (across all threads). Calculated with <see cref="DateTime"/>.</param>
            /// <param name="totalParallelExecutionTimeFromStopwatchInMilliseconds">Total parallel execution time of the entire factory (across all threads) in milliseconds. Calculated with <see cref="Stopwatch"/>.</param>
            /// <param name="actualElapsedExecutionTimeFromDateTime">Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="DateTime"/>.</param>
            /// <param name="actualExecutionTimeFromStopwatchInMilliseconds">Actual serial execution time of the entire factory (on main thread). Calculated with <see cref="Stopwatch"/>.</param>
            public PythonExecutionFactoryOutput(IEnumerable<IPythonExecutor> pythonExecutors, StatusCounter checkStatusCounter, StatusCounter queueStatusCounter, StatusCounter executionStatusCounter, TimeSpan totalParallelExecutionTimeFromDateTime, long totalParallelExecutionTimeFromStopwatchInMilliseconds, TimeSpan actualElapsedExecutionTimeFromDateTime, long actualExecutionTimeFromStopwatchInMilliseconds)
			{
				PythonExecutors = pythonExecutors;
				CheckStatusCounter = checkStatusCounter;
				QueueStatusCounter = queueStatusCounter;
				ExecutionStatusCounter = executionStatusCounter;
				TotalParallelExecutionTimeFromDateTime = totalParallelExecutionTimeFromDateTime;
				TotalParallelExecutionTimeFromStopwatchInMilliseconds = totalParallelExecutionTimeFromStopwatchInMilliseconds;
				ActualExecutionTimeFromDateTime = actualElapsedExecutionTimeFromDateTime;
				ActualExecutionTimeFromStopwatchInMilliseconds = actualExecutionTimeFromStopwatchInMilliseconds;
			}
		}
	}
}
