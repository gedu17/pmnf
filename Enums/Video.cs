
using System.Collections.Generic;

namespace VidsNet.Enums
{
    public class Video {

        private List<string> _types {get;set;}
        private Dictionary<string, string> _mimes {get;set;}

        public Video() {
            _types = new List<string>();
            _types.Add(".avi");
            _types.Add(".m4v");
            _types.Add(".asf");
            _types.Add(".wmv");
            _types.Add(".mpeg");
            _types.Add(".mp4");
            _types.Add(".ogv");
            _types.Add(".webm");
            _types.Add(".mkv");
            _mimes = new Dictionary<string, string>();
            _mimes.Add(".avi", "video/x-msvideo");
            _mimes.Add(".m4v", "video/x-m4v");
            _mimes.Add(".asf", "video/x-ms-asf");
            _mimes.Add(".wmv", "video/x-ms-wmv");
            _mimes.Add(".mpeg", "video/mpeg");
            _mimes.Add(".mp4", "video/mp4");
            _mimes.Add(".ogv", "video/ogg");
            _mimes.Add(".webm", "video/webm");
            _mimes.Add(".mkv", "video/x-matroska");
        }

        public bool IsVideo(string extension) {
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