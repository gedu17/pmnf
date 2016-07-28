
using System.Collections.Generic;

namespace VidsNet
{
    public class SubtitleType {
        private List<string> _types {get;set;}

        public SubtitleType() {
            _types = new List<string>();
            _types.Add(".srt");
            _types.Add(".ssa");
            _types.Add(".ass");
            _types.Add(".smi");
            _types.Add(".sub");
            _types.Add(".idx");
            _types.Add(".mpl");
            _types.Add(".vtt");
            _types.Add(".psb");
            _types.Add(".sami");
            _types.Add(".pjs");
        }

        public bool IsSubtitle(string extension) {
            return _types.Contains(extension);
        }
    }
}