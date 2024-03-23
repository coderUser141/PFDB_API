using PFDB.PythonExecution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using PFDB.WeaponUtility;
using System.Diagnostics;
using PFDB.Logging;

namespace PFDB
{
	namespace PythonFactory
	{

		/// <summary>
		/// Status counter for work items.
		/// </summary>
		public struct StatusCounter
		{
			/// <summary>
			/// Default constructor.
			/// </summary>
			/// <param name="SuccessCounter">Counter for successes.</param>
			/// <param name="FailCounter">Counter for failures.</param>
			public StatusCounter(int SuccessCounter, int FailCounter)
			{
				this.SuccessCounter = SuccessCounter;
				this.FailCounter = FailCounter;
			}

			/// <summary>
			/// The number of successes.
			/// </summary>
			public int SuccessCounter;
			/// <summary>
			/// The number of failures.
			/// </summary>
			public int FailCounter;
		}


		/// <summary>
		/// The number of logical processors the central processing unit (CPU) has.
		/// </summary>
		public enum Cores
		{
			/// <summary>
			/// Single core.
			/// </summary>
			Single = 1,
			/// <summary>
			/// Dual core.
			/// </summary>
			Dual,
			/// <summary>
			/// Quadruple core.
			/// </summary>
			Four = 4,
			/// <summary>
			/// Sextuple core.
			/// </summary>
			Six = 6,
			/// <summary>
			/// Octuple core.
			/// </summary>
			Eight = 8,
			/// <summary>
			/// Decuple core.
			/// </summary>
			Ten = 10,
			/// <summary>
			/// Duodecuple core.
			/// </summary>
			Twelve = 12,
			/// <summary>
			/// Quattordecuple core.
			/// </summary>
			Fourteen = 14,
			/// <summary>
			/// Sexdecuple core.
			/// </summary>
			Sixteen = 16,
			/// <summary>
			/// Octodecuple core.
			/// </summary>
			Eighteen = 18,
			/// <summary>
			/// Vigintuple core.
			/// </summary>
			Twenty = 20
		}

		/// <summary>
		/// Defines a factory class to execute various classes that inherit from <see cref="IPythonExecutable{IOutput}"/> via <see cref="IPythonExecutor"/>.
		/// </summary>
		/// <typeparam name="TPythonExecutable">A concrete type that implements <see cref="IPythonExecutable{IOutput}"/>.</typeparam>
		public sealed class PythonExecutionFactory<TPythonExecutable> /*: IPythonExecutionFactory<IOutput>*/ where TPythonExecutable : IPythonExecutable<IOutput>, new()
		{
			private readonly List<IPythonExecutor> _queue;
			private readonly int _coreCount;
			private readonly int _initialQueueCount;

			

			private void _constructorHelper(OutputDestination outputDestination, int categoryGroup, int weaponNumber, string path, PhantomForcesVersion version, string programDirectory, string? tessbinPath)
			{
				PythonExecutor py = new PythonExecutor(outputDestination);
				TPythonExecutable pythonExecutable = new();
				if (version.VersionString == "8.0.0" || version.VersionString == "8.0.1" || version.VersionString == "8.0.2")
				{
					if(pythonExecutable is PythonTesseractExecutable pyt) {
						_queue.Add(py
							.LoadOut(pyt
								.Construct($"{categoryGroup}_{weaponNumber}_1.png", path, version, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath)));
						_queue.Add(py
							.LoadOut(pyt
								.Construct($"{categoryGroup}_{weaponNumber}_2.png", path, version, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath)));
					}
					else
					{
						_queue.Add(py
							.LoadOut(pythonExecutable
								.Construct($"{categoryGroup}_{weaponNumber}_1.png", path, version, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory)));
						_queue.Add(py
							.LoadOut(pythonExecutable
								.Construct($"{categoryGroup}_{weaponNumber}_2.png", path, version, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory)));
					}

					
				}
				else
				{
					if (pythonExecutable is PythonTesseractExecutable pyt)
					{
						_queue.Add(py
							.LoadOut(pyt
								.Construct($"{categoryGroup}_{weaponNumber}.png", path, version, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath)));
					}
					else
					{
						_queue.Add(py
							.LoadOut(pythonExecutable
								.Construct($"{categoryGroup}_{weaponNumber}.png", path, version, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory)));
					}
				}
			}


			/// <summary>
			/// Constructs the factory from pre-defined values.
			/// </summary>
			/// <param name="weaponIDs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the category of the weapons, and <c>TValue</c> contains the weapons' numbers (in the abovementioned category).</param>
			/// <param name="versionAndPathPairs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the Phantom Forces version, and <c>TValue</c> refers to the absolute path of the <c>TKey</c>'s version.</param>
			/// <param name="programDirectory">The program directory of the Python executable.</param>
			/// <param name="outputDestination">Specifies the output destination.</param>
			/// <param name="tessbinPath">Specifies the absolute path of <c>/tessbin/</c> folder. If null, assumes such folder is in the same working directory.</param>
			/// <param name="coreCount">Specifies the core count manually. Default is dual (2) cores.</param>
			public PythonExecutionFactory(IDictionary<int,List<int>> weaponIDs, IDictionary<PhantomForcesVersion, string> versionAndPathPairs, string programDirectory, OutputDestination outputDestination, string? tessbinPath, Cores coreCount = Cores.Dual)
			{
				_coreCount = (int)coreCount;
				_queue = new List<IPythonExecutor>();
				foreach (PhantomForcesVersion version in versionAndPathPairs.Keys) {
					foreach (int categoryGroup in weaponIDs.Keys) {
						foreach (int weapon in weaponIDs[categoryGroup]) {
							_constructorHelper(outputDestination, categoryGroup, weapon, versionAndPathPairs[version], version, programDirectory, tessbinPath);
						}
					}
				}
				_initialQueueCount = _queue.Count;
			}

			/// <summary>
			/// Constructs the factory from pre-defined values.
			/// </summary>
			/// <param name="weaponIDs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the category of the weapons, and <c>TValue</c> contains the weapons' numbers (in the abovementioned category).</param>
			/// <param name="versionAndPathPairs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the Phantom Forces version, and <c>TValue</c> refers to the absolute path of the <c>TKey</c>'s version.</param>
			/// <param name="programDirectory">The program directory of the Python executable.</param>
			/// <param name="outputDestination">Specifies the output destination.</param>
			/// <param name="tessbinPath">Specifies the absolute path of <c>/tessbin/</c> folder. If null, assumes such folder is in the same working directory.</param>
			/// <param name="coreCount">Specifies the core count manually. Default is dual (2) cores.</param>
			public PythonExecutionFactory(IDictionary<int, Collection<int>> weaponIDs, IDictionary<PhantomForcesVersion, string> versionAndPathPairs, string programDirectory, OutputDestination outputDestination, string? tessbinPath, Cores coreCount = Cores.Dual)
			{
				_coreCount = (int)coreCount;
				_queue = new List<IPythonExecutor>();
				foreach (PhantomForcesVersion version in versionAndPathPairs.Keys) {
					foreach (int categoryGroup in weaponIDs.Keys) {
						foreach (int weapon in weaponIDs[categoryGroup]) {
							_constructorHelper(outputDestination, categoryGroup, weapon, versionAndPathPairs[version], version, programDirectory, tessbinPath);
						}
					}
				}
				_initialQueueCount = _queue.Count;
			}

			/// <summary>
			/// Loads an <see cref="IQueryable{IPythonExecutor}"/> into the factory, with an optional core count.
			/// </summary>
			/// <param name="pythonExecutors">A queryable list of <see cref="IPythonExecutor"/>.</param>
			/// <param name="coreCount">Core count to manually specify. The default value for this parameter is dual (2) core.</param>
			public PythonExecutionFactory(IQueryable<IPythonExecutor> pythonExecutors, Cores coreCount = Cores.Dual)
			{
				_coreCount = (int)coreCount;
				_queue = [.. pythonExecutors];
				_initialQueueCount = _queue.Count;
			}

			/// <summary>
			/// Loads an <see cref="IQueryable{IPythonExecutor}"/> into the factory. Core count is specified based on the processor's logical processor count.
			/// </summary>
			/// <param name="pythonExecutors">A queryable list of <see cref="IPythonExecutor"/>.</param>
			public PythonExecutionFactory(IQueryable<IPythonExecutor> pythonExecutors)
			{
				_coreCount = Environment.ProcessorCount;
				_queue = [.. pythonExecutors];
				_initialQueueCount = _queue.Count;
			}

			/// <summary>
			/// Loads an <see cref="IEnumerable{IPythonExecutor}"/> into the factory, with an optional core count.
			/// </summary>
			/// <param name="pythonExecutors">An enumerable list of <see cref="IPythonExecutor"/>.</param>
			/// <param name="coreCount">Core count to manually specify. The default value for this parameter is dual (2) core.</param>
			public PythonExecutionFactory(IEnumerable<IPythonExecutor> pythonExecutors, Cores coreCount = Cores.Dual)
			{
				_coreCount = (int)coreCount;
				_queue = new List<IPythonExecutor>(pythonExecutors);
				_initialQueueCount = _queue.Count;
			}

			/// <summary>
			/// Loads an <see cref="IEnumerable{IPythonExecutor}"/> into the factory. Core count is specified based on the processor's logical processor count.
			/// </summary>
			/// <param name="pythonExecutors">An enumerable list of <see cref="IPythonExecutor"/>.</param>
			public PythonExecutionFactory(IEnumerable<IPythonExecutor> pythonExecutors)
			{
				_coreCount = Environment.ProcessorCount;
				_queue = new List<IPythonExecutor>(pythonExecutors);
				_initialQueueCount = _queue.Count;
			}

			private StatusCounter _checkFactory()
			{
				if (_queue.Count == 0)
				{
					//Log.
					PFDBLogger.LogError("The internal list was empty.", parameter: nameof(_queue));
					throw new Exception("_queue was empty.");
				}
				StatusCounter checkStatus = new StatusCounter();
				for (int i =0; i < _queue.Count; ++i)
				{
					try
					{
						_queue[i].Input.CheckInput();
						checkStatus.SuccessCounter++;
					}
					catch (Exception ex)
					{
						//check.Output = new FailedPythonOutput(ex.Message);
						//log

						PFDBLogger.LogError($"An exception was raised while checking the object. Internal Message: {ex.Message}", parameter: $"{_queue[i].Input.Filename}");
						_queue.Remove(_queue[i]);
						checkStatus.FailCounter++;
						continue; //skip
					}
				}
				return checkStatus;
			}

			/// <summary>
			/// Starts execution of the factory.
			/// </summary>
			public PythonExecutionFactoryOutput<TPythonExecutable> Start()
			{


				StatusCounter CheckStatus = new StatusCounter();
				try
				{
					CheckStatus = _checkFactory();
				}
				catch
				{
					//error
					return new PythonExecutionFactoryOutput<TPythonExecutable>(_queue, CheckStatus, new StatusCounter(SuccessCounter:0,FailCounter:_queue.Count), new StatusCounter(SuccessCounter: 0, FailCounter: _queue.Count), new TimeSpan(0), 0, new TimeSpan(0), 0);
				}
				StatusCounter QueueStatus = new StatusCounter();
				StatusCounter ExecutionStatus = new StatusCounter();

				PFDBLogger.LogInformation("Factory has started");

				ThreadPool.SetMinThreads(_coreCount, _coreCount);
				ThreadPool.SetMaxThreads(_coreCount, _coreCount);
				
				List<List<IPythonExecutor>> listForQueue = new List<List<IPythonExecutor>>();
				if(_queue.Count < _coreCount) {
					listForQueue.Add(new List<IPythonExecutor>(_queue));
				}

				for(int i = 0; i < _queue.Count/ _coreCount; i++)
				{
					List<IPythonExecutor> temp = new List<IPythonExecutor>();

					for (int j = 0; j < _coreCount; j++)
					{
						temp.Add(_queue[i* _coreCount + j]);
					}
					//listForQueue.Add(temp.ToList());
					listForQueue.Add([..temp]);
				}

				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				DateTime start = DateTime.Now;
				TimeSpan totalParallelTimeElapsedDateTime = new TimeSpan(0);
				long totalParallelTimeElapsedInMilliseconds = 0;


				for (int j = 0; j < listForQueue.Count; ++j)
				{
					//Console.WriteLine($"Thread Count: {ThreadPool.ThreadCount}, Completed Threads: {ThreadPool.CompletedWorkItemCount}, Pending Threads: {ThreadPool.PendingWorkItemCount}");

					int size = listForQueue[j].Count;
					ManualResetEvent[] manualEvents = new ManualResetEvent[size];
					for(int k = 0; k < size; k++)
					{
						manualEvents[k] = listForQueue[j][k].manualEvent;

						try
						{
							ThreadPool.QueueUserWorkItem(new WaitCallback(listForQueue[j][k].Execute));
							QueueStatus.SuccessCounter++;
						}
						catch (NotSupportedException e)
						{
							PFDBLogger.LogError(e.Message);
							QueueStatus.FailCounter++;
						}
					}


					if(WaitHandle.WaitAll(manualEvents,new TimeSpan(0, 5, 0)))
					{
						PFDBLogger.LogInformation("Current parallel asynchronous awaiter has finished.");
						//Console.WriteLine("success, all have been completed!");
						foreach(IPythonExecutor executor in listForQueue[j])
						{
							if(executor.Output is Benchmark benchmark)
							{
								ExecutionStatus.SuccessCounter++;
								totalParallelTimeElapsedDateTime += benchmark.StopwatchDateTime;
								totalParallelTimeElapsedInMilliseconds += benchmark.StopwatchNormal.ElapsedMilliseconds;

							}else if(executor.Output is FailedPythonOutput failedOutput)
							{
								ExecutionStatus.FailCounter++;
								PFDBLogger.LogError($"Execution failed.", parameter: failedOutput.OutputString);
							}
							else
							{
								ExecutionStatus.SuccessCounter++;

							}
						}
					}
					else
					{
						PFDBLogger.LogError("The current parallel asynchronous awaiter timed out.");
						//Console.WriteLine("oh no, part 2 electric boogaloo");
						ExecutionStatus.FailCounter++;
					}
					//idk if this does anything, but i'm thinking it's a good idea to free resources
					foreach(ManualResetEvent u in manualEvents)
					{
						u.Dispose();
					}


					PFDBLogger.LogInformation($"Thread Count: {ThreadPool.ThreadCount}, Completed Threads: {ThreadPool.CompletedWorkItemCount}, Pending Threads: {ThreadPool.PendingWorkItemCount} {Environment.NewLine} Completed/Total Parallel Jobs: {j + 1} / {listForQueue.Count}");

				}
				DateTime end = DateTime.Now;
				stopwatch.Stop();
				return new PythonExecutionFactoryOutput<TPythonExecutable>(_queue, CheckStatus, QueueStatus, ExecutionStatus, totalParallelTimeElapsedDateTime, totalParallelTimeElapsedInMilliseconds, end - start, stopwatch.ElapsedMilliseconds);
				
			}


		}
	}
}
