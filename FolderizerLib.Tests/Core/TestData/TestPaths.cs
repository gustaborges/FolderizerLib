using NUnit.Framework;
using System.IO;

namespace FolderizerLib.Tests.Core.TestData
{
    /// <summary>
    /// This struct provides valid and invalid directory paths for the execution of unit tests.
    /// </summary>
    readonly struct TestPaths
    {
        private static readonly string _rootFileManipulationDirPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "FilesManipulationFolder");
        private static readonly string _untouchedAudioFilesDirPath = Path.Combine(_rootFileManipulationDirPath, "Untouched Audio Files");
        private static readonly string _unorganizedDirPath = Path.Combine(_rootFileManipulationDirPath, "Unorganized Folder");
        private static readonly string _destinationDirPath = Path.Combine(_rootFileManipulationDirPath, "Destination Folder");
        private static readonly string _inexistentDirPath = Path.Combine(_rootFileManipulationDirPath, "Inexistent Folder");

        public static string UntouchedAudioFiles
        {
            get
            {
                Directory.CreateDirectory(_untouchedAudioFilesDirPath);
                return _untouchedAudioFilesDirPath;
            }
        }

        public static string UnorganizedFolder
        {
            get
            {
                Directory.CreateDirectory(_unorganizedDirPath);
                return _unorganizedDirPath;
            }
        }

        public static string DestinationFolder
        {
            get
            {
                Directory.CreateDirectory(_destinationDirPath);
                return _destinationDirPath;
            }
        }

        public static string NotCreatedDirectory
        {
            get
            {
                if (Directory.Exists(_inexistentDirPath))
                {
                    Directory.Delete(_inexistentDirPath, true);
                }
                return _inexistentDirPath;
            }
        }



    }
}