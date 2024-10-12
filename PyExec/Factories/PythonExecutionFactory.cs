using PFDB.PythonExecution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using PFDB.WeaponUtility;
using System.Diagnostics;
using PFDB.Logging;
using PFDB.SQLite;
using PFDB.PythonExecutionUtility;
using PFDB.PythonFactoryUtility;

namespace PFDB
{
	namespace PythonFactory
	{


		/// <summary>
		/// Defines a factory class to execute various classes that inherit from <see cref="IPythonExecutable"/> via <see cref="IPythonExecutor"/>.
		/// </summary>
		/// <typeparam name="TPythonExecutable">A concrete type that implements <see cref="IPythonExecutable"/>.</typeparam>
		public sealed class PythonExecutionFactory<TPythonExecutable> : IPythonExecutionFactory<TPythonExecutable> where TPythonExecutable : IPythonExecutable, new()
		{
			private readonly List<IPythonExecutor> _queue;
			private readonly int _coreCount;
			private readonly int _initialQueueCount;
			private bool _isDefaultConversion;
			private IPythonExecutionFactoryOutput? _factoryOutput = null;

			/// <inheritdoc/>
			public IPythonExecutionFactoryOutput? FactoryOutput { get { return _factoryOutput; } }

			/// <inheritdoc/>
			public bool IsDefaultConversion { get { return _isDefaultConversion; } }

			private void _constructorHelper(OutputDestination outputDestination, Categories categoryGroup, int weaponNumber, string path, PhantomForcesVersion version, string programDirectory, string? tessbinPath)
			{
				PythonExecutor py = new PythonExecutor(outputDestination);
				TPythonExecutable pythonExecutable = new();
				PFDBLogger.LogDebug("PythonExecutionFactory information. Is current object a PythonTesseractExecutable?", parameter: pythonExecutable is PythonTesseractExecutable);

				for(int index = 0; index < version.MultipleScreenshotsCheck(); index++)
				{
					if(pythonExecutable is PythonTesseractExecutable pytessexec)
					{
						_queue.Add(
							py.LoadOut(
								pytessexec.Construct(
									$"{(int)categoryGroup}_{weaponNumber}_{(version.IsLegacy ? index + 1 : index)}.png",
									path,
									WeaponTable.WeaponIDCache[version].First(
										x => x.weaponNumber == weaponNumber ||
										(Categories)x.categoryNumber == categoryGroup
										).weaponID,
									WeaponUtilityClass.GetWeaponType(categoryGroup),
									programDirectory,
									tessbinPath,
									_isDefaultConversion
								)
							)
						);
					}
					else
					{
						_queue.Add(
							py.LoadOut(
								pythonExecutable.Construct(
									$"{(int)categoryGroup}_{weaponNumber}_{(version.IsLegacy ? index + 1 : index)}.png",
									path,
									WeaponTable.WeaponIDCache[version].First(
										x => x.weaponNumber == weaponNumber ||
										(Categories)x.categoryNumber == categoryGroup
										).weaponID,
									WeaponUtilityClass.GetWeaponType(categoryGroup),
									programDirectory,
									_isDefaultConversion
								)
							)
						);
					}
				}



				/*
				if (version.IsLegacy)
				{
					if(pythonExecutable is PythonTesseractExecutable pyt) {
						_queue.Add(py
							.LoadOut(pyt
								.Construct($"{(int)categoryGroup}_{weaponNumber}_1.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
						_queue.Add(py
							.LoadOut(pyt
								.Construct($"{(int)categoryGroup}_{weaponNumber}_2.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
					}
					else 
					{
						_queue.Add(py
							.LoadOut(pythonExecutable
								.Construct($"{(int)categoryGroup}_{weaponNumber}_1.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
						_queue.Add(py
							.LoadOut(pythonExecutable
								.Construct($"{(int)categoryGroup}_{weaponNumber}_2.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
					}

					
				}else if(version.MultipleScreenshotsCheck() == 2 && !version.IsLegacy)
				{
					if (pythonExecutable is PythonTesseractExecutable pyt)
					{
						_queue.Add(py
							.LoadOut(pyt
								.Construct($"{(int)categoryGroup}_{weaponNumber}_0.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
						_queue.Add(py
							.LoadOut(pyt
								.Construct($"{(int)categoryGroup}_{weaponNumber}_1.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
					}
					else
					{
						_queue.Add(py
							.LoadOut(pythonExecutable
								.Construct($"{(int)categoryGroup}_{weaponNumber}_0.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
						_queue.Add(py
							.LoadOut(pythonExecutable
								.Construct($"{(int)categoryGroup}_{weaponNumber}_1.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
					}
				}
				else
				{
					if (pythonExecutable is PythonTesseractExecutable pyt)
					{
						_queue.Add(py
							.LoadOut(pyt
								.Construct($"{(int)categoryGroup}_{weaponNumber}.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, tessbinPath, _isDefaultConversion)));
					}
					else
					{
						_queue.Add(py
							.LoadOut(pythonExecutable
								.Construct($"{(int)categoryGroup}_{weaponNumber}.png", path, WeaponTable.WeaponIDCache[version].First(x => x.weaponNumber == weaponNumber || (Categories)x.categoryNumber == categoryGroup).weaponID, WeaponUtilityClass.GetWeaponType(categoryGroup), programDirectory, _isDefaultConversion)));
					}
				}*/
			}


			/// <summary>
			/// Constructs the factory from pre-defined values.
			/// </summary>
			/// <param name="weaponIDs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the category of the weapons, and <c>TValue</c> contains the weapons' numbers (in the abovementioned category).</param>
			/// <param name="versionAndPathPairs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the Phantom Forces version, and <c>TValue</c> refers to the absolute path of where <c>TKey</c>'s version's images can be found.</param>
			/// <param name="programDirectory">The program directory of the Python executable.</param>
			/// <param name="outputDestination">Specifies the output destination.</param>
			/// <param name="tessbinPath">Specifies the absolute path of <c>/tessbin/</c> folder. If null, assumes such folder is in the same working directory.</param>
			/// <param name="coreCount">Specifies the core count manually. Default is dual (2) cores.</param>
			/// <param name="isDefaultConversion">Specifies if the images supplied are for default conversion.</param>
			public PythonExecutionFactory(IDictionary<Categories,List<int>> weaponIDs, IDictionary<PhantomForcesVersion, string> versionAndPathPairs, string programDirectory, OutputDestination outputDestination, string? tessbinPath, Cores coreCount = Cores.Dual, bool isDefaultConversion = true)
			{
				_coreCount = (int)coreCount;
				_queue = new List<IPythonExecutor>();
				_isDefaultConversion = isDefaultConversion;
				foreach (PhantomForcesVersion version in versionAndPathPairs.Keys)
				{
					foreach (Categories categoryGroup in weaponIDs.Keys) {
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
			/// <param name="versionAndPathPairs">An <see cref="IDictionary{TKey, TValue}"/> object where <c>TKey</c> refers to the Phantom Forces version, and <c>TValue</c> refers to the absolute path of where <c>TKey</c>'s version's images can be found.</param>
			/// <param name="programDirectory">The program directory of the Python executable.</param>
			/// <param name="outputDestination">Specifies the output destination.</param>
			/// <param name="tessbinPath">Specifies the absolute path of <c>/tessbin/</c> folder. If null, assumes such folder is in the same working directory.</param>
			/// <param name="coreCount">Specifies the core count manually. Default is dual (2) cores.</param>
			/// <param name="isDefaultConversion">Specifies if the images supplied are for default conversion.</param>
			public PythonExecutionFactory(IDictionary<Categories, Collection<int>> weaponIDs, IDictionary<PhantomForcesVersion, string> versionAndPathPairs, string programDirectory, OutputDestination outputDestination, string? tessbinPath, Cores coreCount = Cores.Dual, bool isDefaultConversion = true)
			{
				_coreCount = (int)coreCount;
				_queue = new List<IPythonExecutor>();
				_isDefaultConversion = isDefaultConversion;
				foreach (PhantomForcesVersion version in versionAndPathPairs.Keys)
				{
					foreach (Categories categoryGroup in weaponIDs.Keys) {
						foreach (int weapon in weaponIDs[categoryGroup]) {
							_constructorHelper(outputDestination, categoryGroup, weapon, versionAndPathPairs[version], version, programDirectory, tessbinPath);
						}
					}
				}
				_initialQueueCount = _queue.Count;
			}
			
			private void _queueChecker()
			{
				if(_queue.Any() == false)
				{

				}
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
			/// Starts execution of the factory. Checks the factory to see if it is valid.
			/// <list type="bullet">
			///		<item>If invalid, this function will return a <see cref="IPythonExecutionFactoryOutput"/> with no successes and all fails for both checking and queuing.</item>
			///		<item>If valid, it continues as usual.</item>
			/// </list>
			/// Regardless of either case, <see cref="FactoryOutput"/> <b>will</b> be populated.
			/// <para>It then begins executing the list of executors that have been supplied. Execution is synchronous and multithreaded. Number of objects executed synchronously depends on the number of logical processors (cores) the machine has. It waits for the synchronized objects to succeed. If any objects have not succeeded in 5 minutes, the factory will bypass and continue to execute.</para>
			/// </summary>
			/// <inheritdoc/>
			public IPythonExecutionFactoryOutput Start()
			{


				StatusCounter CheckStatus = new StatusCounter();
				try
				{
					CheckStatus = _checkFactory();
				}
				catch
				{
					//error

					_factoryOutput = new PythonExecutionFactoryOutput(_queue,_isDefaultConversion, CheckStatus, new StatusCounter(SuccessCounter:0,FailCounter:_queue.Count), new StatusCounter(SuccessCounter: 0, FailCounter: _queue.Count), new TimeSpan(0), 0, new TimeSpan(0), 0);
					return _factoryOutput;
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
						if (listForQueue[j][k] is IAwaitable awaitable)
						{
							manualEvents[k] = awaitable.ManualEvent;
						}
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
				_factoryOutput = new PythonExecutionFactoryOutput(_queue,_isDefaultConversion, CheckStatus, QueueStatus, ExecutionStatus, totalParallelTimeElapsedDateTime, totalParallelTimeElapsedInMilliseconds, end - start, stopwatch.ElapsedMilliseconds);
				return _factoryOutput;
				
			}


		}
	}
}
