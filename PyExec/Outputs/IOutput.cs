namespace PFDB
{
    namespace PythonExecution
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
    }
}
