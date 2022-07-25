using FolderizerLib.Organizers;
using FolderizerLib.Organizers.Audio;
using FolderizerLib.Tests.Core.Utils;
using FolderizerLib.Tests.Core.TestData;
using NUnit.Framework;
using System.IO;
using System.Linq;
using FolderizerLib.Results;

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
        public void Organize_WhenDestinationFolderIsNotSpecified_NewDirectoryStructureHasOneCriteria_ShouldOrganizeTheNewDirectoryStructureInBasePath()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _oneLevelDirectoryHierarchy);
            organizer.Organize();

            AssertDirectoryStructureArtist(TestPaths.UnorganizedFolder);
        }

        [Test]
        public void Organize_WhenDestinationFolderIsNotSpecified_NewDirectoryStructureHasMoreThanOneCriteria_ShouldOrganizeTheNewDirectoryStructureInBasePath()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
            organizer.Organize();

            AssertDirectoryStructureArtistAlbum(TestPaths.UnorganizedFolder);
        }

        [Test]
        public void Organize_WhenDestinationFolderIsValid_NewDirectoryStructureHasOneCriteria_ShouldOrganizeTheNewDirectoryStructureInTheSpecifiedDestinationFolder()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _oneLevelDirectoryHierarchy);
            organizer.PathToRootDestinationDirectory = TestPaths.DestinationFolder;
            organizer.Organize();

            AssertDirectoryStructureArtist(TestPaths.DestinationFolder);
        }


        [Test]
        public void Organize_WhenDestinationFolderIsValid_NewDirectoryStructureHasMoreThanOneCriteria_ShouldOrganizeTheNewDirectoryStructureInTheSpecifiedDirectory()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
            organizer.PathToRootDestinationDirectory = TestPaths.DestinationFolder;
            organizer.Organize();

            AssertDirectoryStructureArtistAlbum(TestPaths.DestinationFolder);
        }

        [Test]
        public void Organize_WhenDestinationFolderIsInexistent_ShouldCreateTheFolderAndOrganize()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
            organizer.PathToRootDestinationDirectory = TestPaths.DestinationFolder;
            TestEnvironmentUtils.DeleteMountingPath();

            organizer.Organize();

            AssertDirectoryStructureArtistAlbum(TestPaths.DestinationFolder);
        }

        [Test]
        public void Organize_WhenBasePathIsInvalid_ShouldThrowDirectoryNotFoundException()
        {
            var organizer = new AudioOrganizer(TestPaths.NotCreatedDirectory, _twoLevelDirectoryHierarchy);

            Assert.Throws<DirectoryNotFoundException>(() => { organizer.Organize(); });
        }


        [Test]
        public void Organize_WhenFileHandlingMethodIsMove_ShouldMoveAndOrganizeTheFilesInTheNewLocation()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
            organizer.PathToRootDestinationDirectory = TestPaths.DestinationFolder;
            organizer.FileHandlingMethod = FileHandlingMethod.Move;

            organizer.Organize();

            AssertDirectoryStructureArtistAlbum(TestPaths.DestinationFolder);
            AssertFilesHaveBeenMovedFromBasePath();
        }

        [Test]
        public void Organize_WhenOrganizationIsSuccessful_ShouldReturnOrganizationResultWithNoFailure()
        {
            var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
            OrganizationResult organizationResult = organizer.Organize();

            Assert.False(organizationResult.HasFailure);
        }

        [Test]
        public void Organize_WhenAnExceptionIsThrownWhenHandlingAnAudioFile_ThenOrganizationResultShouldContainTheFailure()
        {
            var lockedFile = Directory.EnumerateFiles(TestPaths.UnorganizedFolder).First();
            var fileStream = new FileStream(lockedFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            
            try
            {
                var organizer = new AudioOrganizer(TestPaths.UnorganizedFolder, _twoLevelDirectoryHierarchy);
                OrganizationResult organizationResult = organizer.Organize();

                Assert.AreEqual(1, organizationResult.Failures.Count);
                Assert.AreEqual(lockedFile, organizationResult.Failures[0].File);
                Assert.NotNull(organizationResult.Failures[0].Exception);
            }
            finally
            {
                fileStream.Close();
            }
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