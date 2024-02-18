// See https://aka.ms/new-console-template for more information
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PFDB.PythonExecutor;

Console.WriteLine();

namespace PFDB
{
    namespace PythonExecutor
    {
        /// <summary>
        /// Python Execution Class
        /// <para>
        /// Note: To compile the python file into a working executable, follow these general steps:
        /// <list type="number">
        /// <item>Ensure Python 3.12 is downloaded</item>
        /// <item>Install 'numpy', 'pytesseract', and 'opencv-python' using pip install</item>
        /// <item>Install 'PyInstaller'</item>
        /// <item>Navigate to the folder containing the python file (impa.py) and run "py -m PyInstaller -path=[path to scripts file, i.e. C:\Users\(youruser)\AppData\Local\Programs\Python\Python312\Scripts] --onefile impa.py"</item>
        /// </list>
        /// </para>
        /// </summary>
        public class PythonExecutor : IPythonExecutor<IOutput>
		{
			/// <summary>
			/// Executes the Python Executable. List of weaponTypes:
			/// <list type="number">
			/// <item> = primary</item>
			/// <item> = secondary</item>
			/// <item> = grenade</item>
			/// <item> = melee</item>
			/// </list>
			/// </summary>
			/// <returns>A Tuple, with the first item containing the result; the second containing the first stopwatch time (in seconds) and the third containing the second stopwatch time (in milliseconds). Note that the second and third items return -1.0 when the function fails.</returns>
			/// <exception cref="ArgumentNullException"></exception>
			/// <exception cref="ArgumentException"></exception>
			public IOutput Execute(IPythonExecutable<IOutput> input)
			{
				try
				{
					input.CheckInput();
				}catch(Exception ex)
				{
					return new FailedPythonOutput(ex.Message);
				}
				return input.returnOutput();
				
			}
			

		}
		
	}
}