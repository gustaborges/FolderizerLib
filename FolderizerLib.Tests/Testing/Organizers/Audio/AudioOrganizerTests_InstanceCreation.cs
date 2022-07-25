using FolderizerLib.DirectoryStructureValidators;
using FolderizerLib.Organizers;
using FolderizerLib.Organizers.Audio;
using FolderizerLib.Tests.Core.TestData;
using NUnit.Framework;
using System;

namespace FolderizerLib.Tests.Testing.Organizers.Audio
{
    public class AudioOrganizerTests_InstanceCreation
    {
        DirectoryHierarchy<AudioTag> validHierarchy;

        [OneTimeSetUp]
        public void SetUp()
        {
            validHierarchy = new DirectoryHierarchy<AudioTag>();
            validHierarchy.Append(AudioTag.Album);
        }

        [Test]
        public void Constructor_WhenPathToUnorganizedDirectoryIsNull_ThenShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AudioOrganizer(null, validHierarchy));
        }

        [Test]
        public void Constructor_WhenNewDirectoryHierarchyIsNull_ThenShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AudioOrganizer("", null));
        }


        [Test]
        public void WhenDestinationDirectoryIsNotSpecified_ThenPathToDestinationDirectoryShouldEqualPathToUnorganizedDirectory()
        {
            AudioOrganizer organizer = new(TestPaths.UnorganizedFolder, validHierarchy);

            Assert.AreEqual(organizer.PathToRootUnorganizedDirectory, organizer.PathToRootDestinationDirectory);
        }

        [Test]
        public void WhenDestinationDirectoryIsSpecified_ThenPathToDestinationDirectoryShouldEqualTheSpecifiedValue()
        {
            AudioOrganizer organizer = new(TestPaths.UnorganizedFolder, validHierarchy)
            {
                PathToRootDestinationDirectory = TestPaths.OrganizationFolder
            };

            Assert.AreEqual(TestPaths.OrganizationFolder, organizer.PathToRootDestinationDirectory);
        }

        [Test]
        public void WhenHandlingMethodIsNotSpecified_ThenHandlingMethodShouldDefaultToCopy()
        {
            AudioOrganizer organizer = new(TestPaths.UnorganizedFolder, validHierarchy);

            Assert.AreEqual(FileHandlingMethod.Copy, organizer.FileHandlingMethod);
        }
    }
}