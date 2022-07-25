using NUnit.Framework;
using System.IO;

namespace FolderizerLib.Tests.Core.TestData
{
    /// <summary>
    /// This struct provides valid and invalid directory paths for the execution of unit tests.
    /// </summary>
    readonly struct TestPaths
    {
        private static readonly string _rootAudioTestFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "FilesManipulationFolder");
        private static readonly string _untouchedAudioFiles = Path.Combine(_rootAudioTestFolder, "Untouched Audio Files");
        private static readonly string _unorganizedFolder = Path.Combine(_rootAudioTestFolder, "Unorganized Folder");
        private static readonly string _organizationFolder = Path.Combine(_rootAudioTestFolder, "Destination Folder");
        private static readonly string _inexistentDirectoryPath = Path.Combine(_rootAudioTestFolder, "Inexistent folder");
        /// <summary>
        /// <para>Provides the path of the root testing environment. This folder contains audio file samples which can be copied to the BasePath folder, when <see cref="PopulateBasePath"/> is called, so that the tests can be run.</para>
        /// </summary>
        public static string UntouchedAudioFiles
        {
            get
            {
                Directory.CreateDirectory(_untouchedAudioFiles);
                return _untouchedAudioFiles;
            }
        }
        /// <summary>
        /// <para>Provides the path of an existing directory. In runtime, the getter creates the directory if needed, so that it's always valid.</para>
        /// </summary>
        public static string UnorganizedFolder
        {
            get
            {
                Directory.CreateDirectory(_unorganizedFolder);
                return _unorganizedFolder;
            }
        }

        /// <summary>
        /// <para>Provides the path of an existing directory. In runtime, the getter creates the directory if needed, so that it's always valid.</para>
        /// <para>By default: </para>
        /// </summary>
        public static string OrganizationFolder
        {
            get
            {
                Directory.CreateDirectory(_organizationFolder);
                return _organizationFolder;
            }
        }

        /// <summary>
        /// <para>Provides the path of an inexistent directory.</para>
        /// </summary>
        public static string NotCreatedDirectory
        {
            get
            {
                if (Directory.Exists(_inexistentDirectoryPath))
                {
                    Directory.Delete(_inexistentDirectoryPath, true);
                }
                return _inexistentDirectoryPath;
            }
        }



    }
}