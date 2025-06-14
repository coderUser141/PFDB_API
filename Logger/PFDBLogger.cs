using Microsoft.Extensions.Configuration;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

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

				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] ");
				stringBuilder.Append($"\t Message: {message} ");
				if(parameter != null)stringBuilder.Append($"\t [Parameter: {{parameter}}]");
				
				Log.Debug(stringBuilder.ToString(), parameter);
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
				
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] ");
				stringBuilder.Append($"\t Message: {message} ");
				if(parameter != null)stringBuilder.Append($"\t [Parameter: {{parameter}}]");

				Log.Information(stringBuilder.ToString(), parameter);
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
				
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] ");
				stringBuilder.Append($"\t Message: {message} ");
				if(parameter != null)stringBuilder.Append($"\t [Parameter: {{parameter}}]");

				Log.Warning(stringBuilder.ToString(), parameter);
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
				
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] ");
				stringBuilder.Append($"\t Message: {message} ");
				if(parameter != null)stringBuilder.Append($"\t [Parameter: {{parameter}}]");

				Log.Error(stringBuilder.ToString(), parameter);
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
				
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append($"[Source: {invokingAssembly.GetName().Name}.{cls}.{caller}] ");
				stringBuilder.Append($"\t Message: {message} ");
				if(parameter != null)stringBuilder.Append($"\t [Parameter: {{parameter}}]");

				Log.Fatal(stringBuilder.ToString(), parameter);
			}



		}
	}
}
