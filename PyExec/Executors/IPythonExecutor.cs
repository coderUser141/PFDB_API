using System.Threading;

namespace PFDB
{
    namespace PythonExecution
    {
        /// <summary>
        /// Wrapper interface that takes
        /// </summary>
        public interface IPythonExecutor
        {
            /// <summary>
            /// Output of the enclosed <see cref="IOutput"/> object.
            /// </summary>
            public IOutput Output { get; }

            /// <summary>
            /// Input for this enclosing class (<see cref="PythonExecutor"/>). Will execute and return in <see cref="Output"/> as a class implementing <see cref="IOutput"/>.
            /// </summary>
            public IPythonExecutable<IOutput> Input { get; }

            /// <summary>
            /// Signal state for <see cref="PFDB.PythonFactory.PythonExecutionFactory.Start()"/>.
            /// </summary>
            ManualResetEvent manualEvent { get; set; }

            /// <summary>
            /// Executes the Python application.
            /// </summary>
            public abstract void Execute(object? bs);

            /// <summary>
            /// Loads the <see cref="IPythonExecutable{IOutput}"/> object to be executed.
            /// </summary>
            /// <param name="input">The <see cref="IPythonExecutable{IOutput}"/> object.</param>
            public abstract void Load(IPythonExecutable<IOutput> input);


        }
    }
}
