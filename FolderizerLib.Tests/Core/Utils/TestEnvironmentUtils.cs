using FolderizerLib.Tests.Core.TestData;
using System.Collections;
using System.IO;
using System.Linq;

namespace FolderizerLib.Tests.Core.Utils
{
    class TestEnvironmentUtils
    {
        public static void PopulateBasePath()
        {
            IEnumerable rootTestFolderFiles = Directory.EnumerateFiles(TestPaths.UntouchedAudioFiles);

            foreach (string file in rootTestFolderFiles)
            {
                try
                {
                    string fileDestinationName = file.Split("\\").Last();
                    File.Copy(file, Path.Combine(TestPaths.UnorganizedFolder, fileDestinationName));
                }
                catch { return; }
            }
        }

        public static void DeleteBasePath()
        {
            if (Directory.Exists(TestPaths.UnorganizedFolder))
                Directory.Delete(TestPaths.UnorganizedFolder, true);
        }

        public static void DeleteMountingPath()
        {
            if (Directory.Exists(TestPaths.DestinationFolder))
                Directory.Delete(TestPaths.DestinationFolder, true);
        }
    }
}
