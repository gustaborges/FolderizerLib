using FolderizerLib.Organizers;
using FolderizerLib.Organizers.Audio;
using FolderizerLib.Tests.Core.Utils;
using FolderizerLib.Tests.Core.TestData;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace FolderizerLib.Tests.Testing.Organizers.Audio
{
    [TestFixture]
    public class AudioOrganizerTests_FilesOrganization
    {
        private AudioFilesRepository _filesRepository = new AudioFilesRepository();
        private DirectoryHierarchy<AudioTag> _oneLevelDirectoryHierarchy = new DirectoryHierarchy<AudioTag>();
        private DirectoryHierarchy<AudioTag> _twoLevelDirectoryHierarchy = new DirectoryHierarchy<AudioTag>();

        [SetUp]
        public void SetUpFilesManipulationFolder()
        {
            TestEnvironmentUtils.DeleteBasePath();
            TestEnvironmentUtils.PopulateBasePath();
            TestEnvironmentUtils.DeleteMountingPath();
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            _oneLevelDirectoryHierarchy.Append(AudioTag.Artist);

            _twoLevelDirectoryHierarchy.Append(AudioTag.Artist);
            _twoLevelDirectoryHierarchy.Append(AudioTag.Album);
        }


        [Test]
        public void Run_WhenDestinationFolderIsNotSpecified_NewDirectoryStructureHasOneCriteria_ShouldOrganizeTheNewDirectoryStructureInBasePath()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _oneLevelDirectoryHierarchy);
            organizer.Organize();

            AssertDirectoryStructureArtist(TestPaths.UnorganizedFolder);
        }

        [Test]
        public void Run_WhenDestinationFolderIsNotSpecified_NewDirectoryStructureHasMoreThanOneCriteria_ShouldOrganizeTheNewDirectoryStructureInBasePath()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
            organizer.Organize();

            AssertDirectoryStructureArtistAlbum(TestPaths.UnorganizedFolder);
        }

        [Test]
        public void Run_WhenDestinationFolderIsValid_NewDirectoryStructureHasOneCriteria_ShouldOrganizeTheNewDirectoryStructureInTheSpecifiedDestinationFolder()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _oneLevelDirectoryHierarchy);
            organizer.PathToRootDestinationDirectory = TestPaths.OrganizationFolder;
            organizer.Organize();

            AssertDirectoryStructureArtist(TestPaths.OrganizationFolder);
        }


        [Test]
        public void Run_WhenDestinationFolderIsValid_NewDirectoryStructureHasMoreThanOneCriteria_ShouldOrganizeTheNewDirectoryStructureInTheSpecifiedDirectory()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
            organizer.PathToRootDestinationDirectory = TestPaths.OrganizationFolder;
            organizer.Organize();

            AssertDirectoryStructureArtistAlbum(TestPaths.OrganizationFolder);
        }

        [Test]
        public void Run_WhenDestinationFolderIsInexistent_ShouldCreateTheFolderAndOrganize()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
            organizer.PathToRootDestinationDirectory = TestPaths.OrganizationFolder;
            TestEnvironmentUtils.DeleteMountingPath();

            organizer.Organize();

            AssertDirectoryStructureArtistAlbum(TestPaths.OrganizationFolder);
        }

        [Test]
        public void Run_WhenBasePathIsInvalid_ShouldThrowDirectoryNotFoundException()
        {
            var organizer = new AudioOrganizer(TestPaths.NotCreatedDirectory, _twoLevelDirectoryHierarchy);

            Assert.Throws<DirectoryNotFoundException>(() => { organizer.Organize(); });
        }


        [Test]
        public void Run_WhenFileHandlingMethodIsMove_ShouldMoveAndOrganizeTheFilesInTheNewLocation()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
            organizer.PathToRootDestinationDirectory = TestPaths.OrganizationFolder;
            organizer.FileHandlingMethod = FileHandlingMethod.Move;

            organizer.Organize();

            AssertDirectoryStructureYearAlbum(TestPaths.OrganizationFolder);
            AssertFilesHaveBeenMovedFromBasePath();
        }

        [Test]
        public void Run_WhenExceptionOccursWhileOrganizingTheFiles_ShouldReturnExecutionResultWithFailureLogged()
        {
            //var fileWithForcedError = TestAudioFiles.Files[0];
            //var fileWithForcedErrorPath = $"{folderizerAudio.BasePath}\\{fileWithForcedError.Name}{fileWithForcedError.Format}";

            //using (var stream = File.OpenRead(fileWithForcedErrorPath))
            //{
            //    folderizerAudio.MountingPath = TestPaths.ValidMountingPath;
            //    folderizerAudio.OperationMethod = OperationMethod.Move;
            //    folderizerAudio.SetDesiredDirectoryStructure(AudioTag.Year, AudioTag.Album);

            //    ExecutionResult executionResult = folderizerAudio.Organize();

            //    Assert.True(executionResult.Errors.Length > 0);
            //    Assert.True(executionResult.Errors[0].FilePath.Equals(fileWithForcedErrorPath));
            //}
        }

        private void AssertFilesHaveBeenMovedFromBasePath()
        {
            foreach (var file in _filesRepository.Files)
            {
                Assert.False(File.Exists(Path.Combine(TestPaths.UnorganizedFolder, $"{file.Name}.{file.Format}")));
            }
        }

        private void AssertDirectoryStructureArtistAlbum(string path)
        {
            var distinctArtists = _filesRepository.Files
                .Select((f) => f.AlbumArtist)
                .Distinct();

            foreach (var artist in distinctArtists)
            {
                var albumsOfTheArtist = _filesRepository.Files
                   .Where((f) => f.AlbumArtist == artist)
                   .Select((f) => f.Album)
                   .Distinct();

                foreach (var album in albumsOfTheArtist)
                    Assert.True(Directory.Exists(Path.Combine(path, artist, album)));
            }
        }

        private void AssertDirectoryStructureYearAlbum(string path)
        {
            var distinctYears = _filesRepository.Files
                .Select((f) => f.Year)
                .Distinct();

            foreach (var year in distinctYears)
            {
                var albumsOfTheYear = _filesRepository.Files
                   .Where((f) => f.Year == year)
                   .Select((f) => f.Album)
                   .Distinct();

                foreach (var album in albumsOfTheYear)
                {
                    Assert.True(Directory.Exists(Path.Combine(path, year, album)));
                }
            }
        }

        private void AssertDirectoryStructureArtist(string path)
        {
            var distinctArtists = _filesRepository.Files
                .Select(f => f.AlbumArtist)
                .Distinct();

            foreach (var artist in distinctArtists)
            {
                Assert.True(Directory.Exists(Path.Combine(path, artist)));
            }
        }


    }
}