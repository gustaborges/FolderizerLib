using FolderizerLib.DirectoryStructureValidators;
using FolderizerLib.Organizers;
using FolderizerLib.Organizers.Audio;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FolderizerLib.Tests.Testing.Organizers
{
    internal class DirectoryHierarchyTests
    {
        private DirectoryHierarchy<AudioTag> hierarchy;

        [SetUp]
        public void SetUp()
        {
            hierarchy = new DirectoryHierarchy<AudioTag>();
        }

        [Test]
        public void Append_WhenNewDirectoryStructureContainsDuplicateTag_ShouldThrowInvalidDirectoryStructureException()
        {
            hierarchy.Append(AudioTag.Artist);

            Assert.Throws<InvalidDirectoryStructureException>(() => hierarchy.Append(AudioTag.Artist));

        }

        [Test]
        public void Append_WhenTagIsNotDuplicate_ShouldBeAddedToHierarchy()
        {
            hierarchy.Append(AudioTag.Artist);
            hierarchy.Append(AudioTag.Album);

            Assert.AreEqual(AudioTag.Album, hierarchy.GetStructure().Last());
        }

        [Test]
        public void GetStructure_WhenNoItemHasBeenAdded_ShouldReturnEmptyList()
        {
            Assert.Zero(hierarchy.GetStructure().Count);
        }

        [Test]
        public void GetStructure_WhenMoreThanOneTagHasBeenAdded_ThenAdditionOrderShouldBePreserved()
        {
            IList<AudioTag> expectedHierarchy = new List<AudioTag>()
            { 
                AudioTag.Album, AudioTag.Artist, AudioTag.Year
            };

            hierarchy.Append(expectedHierarchy[0]);
            hierarchy.Append(expectedHierarchy[1]);
            hierarchy.Append(expectedHierarchy[2]);

            Assert.AreEqual(expectedHierarchy, hierarchy.GetStructure());
        }


        [Test]
        public void Indexer_ShouldReturnEquivalentItemReturnedFromGetStructureMethod()
        {
            hierarchy.Append(AudioTag.Album);
            hierarchy.Append(AudioTag.Genre);

            Assert.AreEqual(hierarchy[0], hierarchy.GetStructure()[0]);
            Assert.AreEqual(hierarchy[1], hierarchy.GetStructure()[1]);
        }

        [Test]
        public void Indexer_WhenIndexIsOutOfRange_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => 
            {
                AudioTag _ = hierarchy[0];
            });
        }
    }
}
