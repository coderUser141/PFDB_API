namespace PFDB
{
    namespace PythonFactory
    {
        /// <summary>
        /// Interface for Python execution factories.
        /// </summary>
        public interface IPythonExecutionFactory
        {
            /// <summary>
            /// Starts the factory.
            /// </summary>
            public IPythonExecutionFactoryOutput Start();
        }

    }
}
