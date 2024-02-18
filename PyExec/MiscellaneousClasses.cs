using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB
{
    namespace PythonExecutor
    {

        /// <summary>
        /// Class for weapon type.
        /// </summary>
        public static class WeaponType
        {
            /// <summary>
            /// Weapon type enumerator for Python script.
            /// </summary>
            public enum Weapon
            {
                /// <summary>
                /// Primary Gun.
                /// </summary>
                Primary = 1,
                /// <summary>
                /// Secondary Gun.
                /// </summary>
                Secondary,
                /// <summary>
                /// Grenade.
                /// </summary>
                Grenade,
                /// <summary>
                /// Melee.
                /// </summary>
                Melee
            }
        }

        /// <summary>
        /// General utility class for Python interop.
        /// </summary>
        public static class PyUtilityClass
        {

            /// <summary>
            /// Compares two file paths and determines a common directory (excludes absolute if a common one is found). Can be used to obfuscate parent root directories when they are not needed.
            /// </summary>
            /// <param name="currentProcessPath">Current directory. Note that this parameter need not be the actual current process directory.</param>
            /// <param name="foreignPath">Foreign directory. Note that this parameter need not be the actual foreign directory.</param>
            /// <returns>A <c>Tuple</c>, with the first item containing the path from the common path to the current directory path, and the second item containing the path from the common path to the foreign directory.</returns>
            /// <exception cref="Exception">Throws exceptions if two paths are unequal when they should be. (illegal case)</exception>
            public static Tuple<string, string> commonExecPath(string currentProcessPath, string foreignPath)
            {
                string tempCurrent = currentProcessPath;
                string tempForeign = foreignPath;
                if (!currentProcessPath.StartsWith(foreignPath) && !foreignPath.StartsWith(currentProcessPath))
                { //distinct, but has common directory
                    for (int i = 0; i < Math.Min(currentProcessPath.Length, foreignPath.Length); ++i)
                    {
                        if (currentProcessPath[i] != foreignPath[i])
                        {
                            tempCurrent = tempCurrent.Substring(0, i);
                            tempForeign = tempForeign.Substring(0, i);

                            if (tempForeign != tempCurrent) throw new Exception($"Something went really wrong: {tempCurrent} should equal {tempForeign}");

                            int lastSlashBeforeSubstringC = tempCurrent.LastIndexOf('\\'); //finds last slash (aka last common directory)
                            int lastSlashBeforeSubstringF = tempForeign.LastIndexOf('\\');

                            if (lastSlashBeforeSubstringC != lastSlashBeforeSubstringF) throw new Exception($"Something went really wrong: {lastSlashBeforeSubstringC} should equal {lastSlashBeforeSubstringF}");

                            tempCurrent = tempCurrent.Substring(0, lastSlashBeforeSubstringC); //truncates off last slash
                            tempForeign = tempForeign.Substring(0, lastSlashBeforeSubstringF);

                            if (tempForeign != tempCurrent) throw new Exception($"Something went really wrong: {tempCurrent} should equal {tempForeign}");

                            int lastSlashBeforeSubstringC2 = tempCurrent.LastIndexOf('\\');
                            int lastSlashBeforeSubstringF2 = tempForeign.LastIndexOf('\\');

                            tempCurrent = currentProcessPath.Substring(lastSlashBeforeSubstringC2);
                            tempForeign = foreignPath.Substring(lastSlashBeforeSubstringF2);
                            break;
                        }
                    }
                }
                else //subset, or the same
                {
                    tempCurrent = tempCurrent.Substring(0, Math.Min(currentProcessPath.Length, foreignPath.Length));
                    tempForeign = tempForeign.Substring(0, Math.Min(currentProcessPath.Length, foreignPath.Length));

                    int lastSlashBeforeSubstringC = tempCurrent.LastIndexOf('\\'); //finds last slash (aka last common directory)
                    int lastSlashBeforeSubstringF = tempForeign.LastIndexOf('\\');
                    if (lastSlashBeforeSubstringC == -1) lastSlashBeforeSubstringC = 0; //couldn't find slash in currentDir, make it beginning of string
                    if (lastSlashBeforeSubstringF == -1) lastSlashBeforeSubstringF = 0;

                    tempCurrent = tempCurrent.Substring(0, lastSlashBeforeSubstringC); //truncates off last slash
                    tempForeign = tempForeign.Substring(0, lastSlashBeforeSubstringF);
                    //alternative: tempForeign = tempForeign[..lastSlashBeforeSubstringF];

                    int lastSlashBeforeSubstringC2 = tempCurrent.LastIndexOf('\\');
                    int lastSlashBeforeSubstringF2 = tempForeign.LastIndexOf('\\');
                    if (lastSlashBeforeSubstringC2 == -1) lastSlashBeforeSubstringC2 = 0; //couldn't find slash in currentDir, make it beginning of string
                    if (lastSlashBeforeSubstringF2 == -1) lastSlashBeforeSubstringF2 = 0;

                    tempCurrent = currentProcessPath.Substring(lastSlashBeforeSubstringC2);
                    tempForeign = foreignPath.Substring(lastSlashBeforeSubstringF2);

                }
                return Tuple.Create(tempCurrent, tempForeign);
            }
        }

        /// <summary>
        /// Default implementation of <see cref="IOutput"/>.
        /// </summary>
        public class PythonOutput : IOutput
        {
            /// <summary>
            /// Output string.
            /// </summary>
            public string OutputString { get; init; }

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="outputString">Output string from result.</param>
            public PythonOutput (string outputString)
            {
                OutputString = outputString;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IOutput"/> meant to represent a failed execution.
        /// </summary>
        public class FailedPythonOutput : IOutput
        {

            /// <summary>
            /// Output string.
            /// </summary>
            public string OutputString { get; init; }


            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="outputString">Output string from result.</param>
            public FailedPythonOutput(string outputString)
            {
                OutputString = outputString;
            }
        }

    }
}
