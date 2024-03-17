using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFDB.PythonExecution;
using PFDB.WeaponUtility;

namespace PFDB
{
    namespace PythonExecution
    {
        /// <summary>
        /// Dummy initialization implementation of <see cref="IOutput"/>.
        /// </summary>
        public class TestOutput : IOutput
        {
            /// <summary>
            /// Dummy output string.
            /// </summary>
            public string OutputString { get; private set; }

            /// <summary>
            /// Dummy constructor.
            /// </summary>
            public TestOutput()
            {
                this.OutputString = "init object";
            }
        }

        /// <summary>
        /// Dummy initialization implementation of <see cref="IPythonExecutable{IOutput}"/>.
        /// </summary>
        public class InitExecutable : IPythonExecutable<IOutput>
        {
            /// <summary>
            /// Dummy default constructor.
            /// </summary>
            public InitExecutable(){
            }

            /// <summary>
            /// Dummy filename. Set to <c>string.Empty</c> by default;
            /// </summary>
            public string Filename { get; private set; } = string.Empty;

            /// <summary>
            /// Dummy program directory.
            /// </summary>
            public string ProgramDirectory { get; private set; } = string.Empty;

            /// <summary>
            /// Dummy version.
            /// </summary>
            public PhantomForcesVersion Version { get; private set; } = new PhantomForcesVersion("8.0.1");

            /// <summary>
            /// Dummy input-checker.
            /// </summary>
            public void CheckInput()
            {
                return;
            }

            /// <summary>
            /// Dummy producer.
            /// </summary>
            /// <returns>Blank <see cref="ProcessStartInfo"/>.</returns>
            public ProcessStartInfo GetProcessStartInfo()
            {
                return new ProcessStartInfo();
            }

            /// <summary>
            /// Dummy return.
            /// </summary>
            /// <returns>Blank <see cref="TestOutput"/>.</returns>
            public IOutput ReturnOutput()
            {
                return new TestOutput();
            }

            /// <summary>
            /// Dummy constructor.
            /// </summary>
            /// <param name="filename">Dummy parameter.</param>
            /// <param name="fileDirectory">Dummy parameter.</param>
            /// <param name="version">Dummy parameter.</param>
            /// <param name="weaponType">Dummy parameter.</param>
            /// <param name="programDirectory">Dummy parameter.</param>
            /// <returns>The current object for chaining.</returns>
            public IPythonExecutable<IOutput> Construct(string filename, string fileDirectory, PhantomForcesVersion version, WeaponType weaponType, string programDirectory)
            {
                return this;
            }
        }

    }
}
