using FolderizerLib.Results;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FolderizerLib.Organizers.Audio
{
    public partial class AudioOrganizer
    {
        private string _pathToDestinationDirectory;
        private string _pathToUnorganizedDirectory;
        private DirectoryHierarchy<AudioTag> _newDirectoryHierarchy;

        public AudioOrganizer(string pathToUnorganizedDirectory, DirectoryHierarchy<AudioTag> newDirectoryHierarchy)
        {
            this.PathToRootUnorganizedDirectory = pathToUnorganizedDirectory;
            this.NewDirectoryHierarchy = newDirectoryHierarchy;
        }

        public string PathToRootDestinationDirectory
        {
            get => _pathToDestinationDirectory ?? PathToRootUnorganizedDirectory;
            set => _pathToDestinationDirectory = value ?? throw new ArgumentNullException();            
        }

        public string PathToRootUnorganizedDirectory 
        {
            get => _pathToUnorganizedDirectory;
            private set => _pathToUnorganizedDirectory = value ?? throw new ArgumentNullException();
        }

        public DirectoryHierarchy<AudioTag> NewDirectoryHierarchy
        {
            get => _newDirectoryHierarchy;
            private set => _newDirectoryHierarchy = value ?? throw new ArgumentNullException();
        }

        public FileHandlingMethod FileHandlingMethod
        {   
            get;
            set;
        } = FileHandlingMethod.Copy;
    }

    public partial class AudioOrganizer : IOrganizer
    {
        public OrganizationResult Organize()
        {
            var result = new OrganizationResult();
            var files = Directory.EnumerateFiles(PathToRootUnorganizedDirectory);

            foreach (string file in files)
            {
                try
                {
                    this.OrganizeFileIntoNewLocation(file);
                }
                catch (Exception ex)
                {
                    result.AppendFailure(file, ex);
                }
            }
            return result;
        }

        public Task<OrganizationResult> OrganizeAsync()
        {
            return Task.Run(Organize);
        }


        private void OrganizeFileIntoNewLocation(string file)
        {
            if (NotAudioFile(file))
                return;

            string finalDirectoryPath = GenerateNewDirectoryPath(filePath: file, PathToRootDestinationDirectory);
            string finalFilePath = Path.Combine(finalDirectoryPath, Path.GetFileName(file));

            Directory.CreateDirectory(finalDirectoryPath);

            if (this.FileHandlingMethod == FileHandlingMethod.Move)
            {
                File.Move(file, finalFilePath);
            }
            else
            {
                File.Copy(file, finalFilePath);
            }
        }

        private string GenerateNewDirectoryPath(string filePath, string mountingPath)
        {
            foreach (AudioTag tag in NewDirectoryHierarchy.Get())
            {
                mountingPath = Path.Combine(mountingPath, GetTagValueFromFile(tag, filePath));
            }
            return mountingPath;
        }

        private string GetTagValueFromFile(AudioTag tag, string filePath)
        {
            TagLib.File file = TagLib.File.Create(filePath);
            string value;

            switch (tag)
            {
                case AudioTag.Album: 
                    value = file.Tag.Album;
                    break;

                case AudioTag.Artist: 
                    value = !String.IsNullOrWhiteSpace(file.Tag.JoinedAlbumArtists) ? file.Tag.JoinedAlbumArtists : file.Tag.JoinedPerformers;
                    break;
                
                case AudioTag.Year: 
                    value = file.Tag.Year.ToString();
                    break;

                case AudioTag.Genre: 
                    value = file.Tag.FirstGenre;
                    break;

                default:
                    value = String.Empty;
                    break;
            }

            return String.IsNullOrWhiteSpace(value) ? $"Unknown {tag}" : value;
        }

        private bool NotAudioFile(string filePath) => !AudioFormats.Extensions.Contains(Path.GetExtension(filePath));
    }

}
