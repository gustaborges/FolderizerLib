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
                    this.OrganizeFileIntoNewLocation(file, FileHandlingMethod);
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


        private void OrganizeFileIntoNewLocation(string filePath, FileHandlingMethod handlingMethod)
        {
            if (NotAnAudioFile(filePath))
            {
                return;
            }

            string newDirectoryPath = GenerateNewDirectoryPathAccordingToDesiredHierarchy(filePath);
            string newFilePath = GenerateNewFilePath(filePath, newDirectoryPath);

            Directory.CreateDirectory(newDirectoryPath);

            PerformOrganizationAccordingToHandlingMethod(filePath, newFilePath, handlingMethod);
        }

        private static void PerformOrganizationAccordingToHandlingMethod(string currentFilePath, string newFilePath, FileHandlingMethod handlingMethod)
        {
            switch (handlingMethod)
            {
                case FileHandlingMethod.Move: File.Move(currentFilePath, newFilePath); break;
                case FileHandlingMethod.Copy: File.Copy(currentFilePath, newFilePath); break;
                default: throw new InvalidOperationException(handlingMethod + " handling method is not supported");
            }
        }

        private static string GenerateNewFilePath(string currentFilePath, string newDirectoryPath)
        {
            string fileName = Path.GetFileName(currentFilePath);
            return Path.Combine(newDirectoryPath, fileName);
        }

        private string GenerateNewDirectoryPathAccordingToDesiredHierarchy(string filePath)
        {
            string newDirectoryPath = PathToRootDestinationDirectory;

            foreach (AudioTag tag in NewDirectoryHierarchy.GetStructure())
            {
                string extractedTagValue = ExtractTagFromFile(tag, filePath);
                newDirectoryPath = Path.Combine(newDirectoryPath, extractedTagValue);
            }

            return newDirectoryPath;
        }

        private string ExtractTagFromFile(AudioTag tag, string filePath)
        {
            TagLib.File file = TagLib.File.Create(filePath);
            string value;

            switch (tag)
            {
                case AudioTag.Album:
                    value = file.Tag.Album;
                    break;

                case AudioTag.Artist: 
                    value = !String.IsNullOrWhiteSpace(file.Tag.JoinedAlbumArtists)
                        ? file.Tag.JoinedAlbumArtists
                        : file.Tag.JoinedPerformers;
                    break;
                
                case AudioTag.Year: 
                    value = file.Tag.Year.ToString();
                    break;

                case AudioTag.Genre: 
                    value = file.Tag.FirstGenre;
                    break;

                default:
                    throw new InvalidOperationException("Extraction of  '" + tag + "' tag is not supported");
            }

            return String.IsNullOrWhiteSpace(value) ? $"Unknown {tag}" : value;
        }

        private bool NotAnAudioFile(string filePath) => !AudioFormats.Extensions.Contains(Path.GetExtension(filePath));
    }

}
