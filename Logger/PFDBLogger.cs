using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PFDB
{
	namespace Logging
	{
		public class PFDBLogger
		{



			public IConfigurationRoot ConfigurationRoot { get; private set; }

			public PFDBLogger(string logFileName)
			{
				IConfigurationBuilder configuration = new ConfigurationBuilder();
				configuration.SetBasePath(Directory.GetCurrentDirectory()).AddEnvironmentVariables().AddJsonFile("appsettings.json");
				Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(configuration.Build())
					.Enrich.FromLogContext()
					.WriteTo.Console()
					.WriteTo.File(logFileName, shared: true)
					.CreateLogger();

				Log.Logger.Information("Application start. Logging has been activated.");

				ConfigurationRoot = configuration.Build();
			}

			public static void LogDebug(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
			{

				if (caller is null)
				{
					throw new ArgumentNullException(nameof(caller));
				}

				Assembly invokingAssembly = Assembly.GetCallingAssembly();
				var mth = new StackTrace().GetFrame(1)?.GetMethod();
				var cls = mth?.ReflectedType?.Name;
				Log.Debug($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \t [Parameter: {{parameter}}] \t Message: {message}", parameter);
			}

			public static void LogInformation(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
			{
				if (caller is null)
				{
					throw new ArgumentNullException(nameof(caller));
				}

				Assembly invokingAssembly = Assembly.GetCallingAssembly();
				var mth = new StackTrace().GetFrame(1)?.GetMethod();
				var cls = mth?.ReflectedType?.Name;
				Log.Information($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \t [Parameter: {{parameter}}] \t Message: {message}", parameter);
			}

			public static void LogWarning(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
			{
				if (caller is null)
				{
					throw new ArgumentNullException(nameof(caller));
				}

				Assembly invokingAssembly = Assembly.GetCallingAssembly();
				var mth = new StackTrace().GetFrame(1)?.GetMethod();
				var cls = mth?.ReflectedType?.Name;
				Log.Warning($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \t [Parameter: {{parameter}}] \t Message: {message}", parameter);
			}

			public static void LogError(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
			{
				if (caller is null)
				{
					throw new ArgumentNullException(nameof(caller));
				}

				Assembly invokingAssembly = Assembly.GetCallingAssembly();
				var mth = new StackTrace().GetFrame(1)?.GetMethod();
				var cls = mth?.ReflectedType?.Name;
				Log.Error($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \t [Parameter: {{parameter}}] \t Message: {message}", parameter);
			}

			public static void LogFatal(string message, [CallerMemberName] string caller = "", params object?[]? parameter)
			{
				if (caller is null)
				{
					throw new ArgumentNullException(nameof(caller));
				}

				Assembly invokingAssembly = Assembly.GetCallingAssembly();
				var mth = new StackTrace().GetFrame(1)?.GetMethod();
				var cls = mth?.ReflectedType?.Name;
				Log.Fatal($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] \t [Parameter: {{parameter}}] \t Message: {message}", parameter);
			}



		}
	}
}
