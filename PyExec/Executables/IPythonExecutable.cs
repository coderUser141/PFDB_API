using System;
using System.Diagnostics;
using PFDB.WeaponUtility;

namespace PFDB
{
    namespace PythonExecution
    {
        /// <summary>
        /// This interface is responsible for calling and interacting with the Python script directly.
        /// </summary>
        /// <typeparam name="IOutput">Producable type of <see cref="IPythonExecutable{IOutput}"/>.
        /// </typeparam>
        public interface IPythonExecutable<out IOutput>
        {
            /// <summary>
            /// The filename of the image to pass to the executor.
            /// </summary>
            string Filename { get; }

            /// <summary>
            /// Directory where the Python executable resides
            /// </summary>
            string ProgramDirectory { get; }

            /// <summary>
            /// Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.
            /// </summary>
            PhantomForcesVersion Version { get; }

            /// <summary>
            /// Builds and returns a <see cref="ProcessStartInfo"/> object for program execution.
            /// </summary>
            /// <returns>A <see cref="ProcessStartInfo"/> object.</returns>
            ProcessStartInfo GetProcessStartInfo();

            /// <summary>
            /// Checks the input, if necessary.
            /// </summary>
            /// <exception cref="ArgumentException"/>
            void CheckInput();

            /// <summary>
            /// Returns the output.
            /// </summary>
            /// <returns>The <see cref="IOutput"/> object associated with the current class.</returns>
            IOutput ReturnOutput();

            /// <summary>
            /// Default constructor to be used when instantiating an object.
            /// </summary>
            /// <param name="filename">Name of the file to be read by the Python application.</param>
            /// <param name="fileDirectory">Directory where the images for reading reside.</param>
            /// <param name="weaponType">WeaponType of the weapon, telling the Python application where to read.</param>
            /// <param name="version">Phantom Forces Version. "800" = Version 8.0.0; "1001" = Version 10.0.1, etc.</param>
            /// <param name="programDirectory">Directory where the Python executable resides.</param>
            /// <returns></returns>
            IPythonExecutable<IOutput> Construct(string filename, string fileDirectory, PhantomForcesVersion version, WeaponType weaponType, string programDirectory);
        }
    }
}
