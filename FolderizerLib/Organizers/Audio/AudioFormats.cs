using System.Collections.Generic;

namespace FolderizerLib.Organizers.Audio
{
    struct AudioFormats
    {
        public static IReadOnlyList<string> Extensions = new List<string>
        {
            ".aa", ".aax", ".aac", ".aiff", ".ape", ".dsf", ".flac", ".m4a", ".m4b", ".m4p", 
            ".mp3", ".mpc", ".mpp", ".ogg", ".oga", ".wav", ".wma", ".wv", ".webm"
        };
    }
}
