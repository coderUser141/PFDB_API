using System;
using System.IO;
using PFDB.Logging;
using PFDB.PythonExecution;
using PFDB.PythonExecutionUtility;
using System.Runtime.CompilerServices;

namespace PFDB{
    namespace PythonTesting{

        /// <summary>
        /// Defines a class that tests functions within PyExec.
        /// </summary>
        public static class PythonTest{
            /// <summary>
            /// Main entry point.
            /// </summary>
            public static void Main(){
                PFDBLogger logger =  new PFDBLogger(".pfdblog");
                Test();
            }

            /// <summary>
            /// Main testing function.
            /// </summary>
            public static void Test(){
                PythonInitExecutableTest();
                PythonExecutorInitExecutableConsoleTest();
                PythonExecutorInitExecutableFileTest();
            }

            /// <summary>
            /// Tests if <see cref="InitExecutable"/> returns expected "init object"  output. 
            /// </summary>
            /// <returns>Whether this test passes.</returns>
            public static bool PythonInitExecutableTest(){
                IPythonExecutable executable = new InitExecutable();
                bool pass = (executable.ReturnOutput().OutputString == "init object");
                return TestingOutput("Init Executable detection test", pass, "True", pass.ToString());
            }
            
            /// <summary>
            /// Tests if the console output option works. I do not know how to test for past console output, so this test is the responsibility of the person reading the logs.
            /// </summary>
            public static void PythonExecutorInitExecutableConsoleTest(){
                IPythonExecutor executor = new PythonExecutor(OutputDestination.Console);
                PFDBLogger.LogInformation("Below this message there should be \"init object\".");
                executor.Execute(null);
            }
            public static void PythonExecutorInitExecutableFileTest(){
                IPythonExecutor executor = new PythonExecutor(OutputDestination.File);
                executor.Execute(null);
                bool outputfolderexists = Directory.Exists(Directory.GetCurrentDirectory()+"/"+PythonExecutor.OutputFolderName+"/0");
                bool logfolderexists = Directory.Exists(Directory.GetCurrentDirectory()+"/"+PythonExecutor.LogFolderName+"/0");
                PFDBLogger.LogInformation($"Did it make an output directory? {outputfolderexists}");
                PFDBLogger.LogInformation($"Did it make a log directory? {logfolderexists}");
                if(logfolderexists)Directory.Delete(Directory.GetCurrentDirectory()+"/"+PythonExecutor.LogFolderName+"/0",true);
                if(outputfolderexists)Directory.Delete(Directory.GetCurrentDirectory()+"/"+PythonExecutor.OutputFolderName+"/0",true);
                PFDBLogger.LogInformation("Deleted output and log folders (if they even existed)");
            }

            /// <summary>
            /// Standardized way of outputting pass/fail condition for various tests.
            /// </summary>
            /// <param name="testName">Name of the test being performed.</param>
            /// <param name="pass">Whether the test passed or failed.</param>
            /// <param name="expectedOutput">Expected output (in string format).</param>
            /// <param name="actualOutput">Actual output (in string format).</param>
            /// <param name="caller">Leave blank unless you wish to override the original test function name.</param>
            /// <returns>Whether the test passed or failed (equivalent to the value of "pass".)</returns>
            public static bool TestingOutput(string testName, bool pass, string expectedOutput, string actualOutput, [CallerMemberName] string caller = ""){
                string originalCaller = caller ?? "";
                if(pass){
                    PFDBLogger.LogInformation($"{testName} passed. Expected: {expectedOutput}. Got: {actualOutput}",originalCaller);
                    return true;
                }else{
                    PFDBLogger.LogError($"{testName} failed. Expected: {expectedOutput}. Got: {actualOutput}",originalCaller);
                    return false;
                }
            }
        }
    }
}
/*    
	  <PreBuildEvent Condition="Exists('C:/')"><!--Windows Build-->
		  $(SolutionDir)bin/python -m PyInstaller -path=[C:\Users\Aethelhelm\AppData\Local\Programs\Python\Python312\Scripts] --workpath $(SolutionDir)ImageParserForAPI\build --distpath $(SolutionDir)ImageParserForAPI\dist -F $(SolutionDir)ImageParserForAPI\impa.py
		  xcopy $(SolutionDir)ImageParserForAPI\dist\impa.exe $(SolutionDir)ImageParserForAPI\ /y /f /v
		  mkdir $(SolutionDir)PyExec\bin\$(Configuration)\$(TargetFramework)\tessbin
		  xcopy $(SolutionDir)ImageParserForAPI\tessbin\ $(SolutionDir)PyExec\bin\$(Configuration)\$(TargetFramework)\tessbin\ /y /f /v /e /h /j
		  xcopy $(SolutionDir)ImageParserForAPI\dist\impa.exe $(SolutionDir)PyExec\bin\$(Configuration)\$(TargetFramework)\ /y /f /v
	  </PreBuildEvent>
    <PreBuildEvent Condition="Exists('/usr/bin') and Exists('$(SolutionDir)')"><!--Linux Build, solution build-->
      $(SolutionDir)/bin/python -m PyInstaller --workpath $(SolutionDir)ImageParserForAPI/build --distpath $(SolutionDir)ImageParserForAPI/dist -F -n impa -c $(SolutionDir)ImageParserForAPI/impa.py
      cp -vaf $(SolutionDir)ImageParserForAPI/dist/impa $(SolutionDir)ImageParserForAPI
      mkdir $(SolutionDir)PyExec/bin/$(Configuration)/$(TargetFramework)/tessbin
      cp -vaf $(SolutionDir)ImageParserForAPI/tessbin $(SolutionDir)PyExec/bin/$(Configuration)/$(TargetFramework)
      cp -vaf $(SolutionDir)ImageParserForAPI/dist/impa $(SolutionDir)PyExec/bin/$(Configuration)/$(TargetFramework)
    </PreBuildEvent>
    <PreBuildEvent Condition="Exists('/usr/bin') and !Exists('$(SolutionDir)')"><!--Linux Build, project build-->
      ../../../../bin/python -m PyInstaller --workpath ../../../../ImageParserForAPI/build --distpath ../../../../ImageParserForAPI/dist -F -n impa -c ../../../../ImageParserForAPI/impa.py
      cp -vaf ../../../../ImageParserForAPI/dist/impa ../../../../ImageParserForAPI
      mkdir ../../../../PyExec/bin/$(Configuration)/$(TargetFramework)/tessbin
      cp -vaf ../../../../ImageParserForAPI/tessbin ../../../../PyExec/bin/$(Configuration)/$(TargetFramework)
      cp -vaf ../../../../ImageParserForAPI/dist/impa ../../../../PyExec/bin/$(Configuration)/$(TargetFramework)
    </PreBuildEvent>
    */