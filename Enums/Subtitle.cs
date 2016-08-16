
using System.Collections.Generic;

namespace VidsNet.Enums
{
    public class Subtitle {
        private List<string> _types {get;set;}
        private Dictionary<string, string> _mimes {get;set;}

        public Subtitle() {
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
            _mimes = new Dictionary<string, string>();
            _mimes.Add(".srt", "application/x-subrip");
            _mimes.Add(".ssa", "application/octet-stream");
            _mimes.Add(".ass", "application/octet-stream");
            _mimes.Add(".smi", "application/smil+xml");
            _mimes.Add(".sub", "text/vnd.dvb.subtitle");
            _mimes.Add(".idx", "application/octet-stream");
            _mimes.Add(".mpl", "application/octet-stream");
            _mimes.Add(".vtt", "text/vtt");
            _mimes.Add(".psb", "application/vnd.3gpp.pic-bw-small");
            _mimes.Add(".sami", "application/octet-stream");
            _mimes.Add(".pjs", "application/octet-stream");
        }

        public bool IsSubtitle(string extension) {
            return _types.Contains(extension);
        }

        public string GetMime(string extension) {
            if(_mimes.ContainsKey(extension)) {
                return _mimes[extension];
            }
            else {
                return "application/octet-stream";
            }
            
        }
    }
}