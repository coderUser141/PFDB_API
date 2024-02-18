using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFDB
{
    namespace PythonExecutor
    {

        // INTERFACES //

        /// <summary>
        /// Interface for output types for this class.
        /// </summary>
        public interface IOutput
        {
            /// <summary>
            /// Output string of <see cref="IOutput"/> producers.
            /// </summary>
            public string OutputString { get; }

        }

        /// <summary>
        /// General input (consumable) for types derived from <see cref="IPythonExecutor{IOutput}"/>
        /// </summary>
        /// <typeparam name="IOutput">Producable type of <see cref="IPythonExecutable{IOutput}"/>.
        /// </typeparam>
        public interface IPythonExecutable<out IOutput>
        {
            /// <summary>
            /// The filename of the image to pass to the executor.
            /// </summary>
            public string Filename { get; }

            /// <summary>
            /// Directory where the Python executable resides
            /// </summary>
            public string ProgramDirectory { get; }

            /// <summary>
            /// Builds and returns a <see cref="ProcessStartInfo"/> object for program execution.
            /// </summary>
            /// <returns>A <see cref="ProcessStartInfo"/> object.</returns>
            public abstract ProcessStartInfo GetProcessStartInfo();

            /// <summary>
            /// Checks the input, if necessary.
            /// </summary>
            public abstract void CheckInput();

            /// <summary>
            /// Returns the output 
            /// </summary>
            /// <returns></returns>
            public abstract IOutput returnOutput();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="IOutput"></typeparam>
        public interface IPythonExecutor<IOutput>
        {
            /// <summary>
            /// Executes the Python application.
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public abstract IOutput Execute(IPythonExecutable<IOutput> input);
        }
    }
}
