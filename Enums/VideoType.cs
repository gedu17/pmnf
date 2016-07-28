
using System.Collections.Generic;

namespace VidsNet
{
    public class VideoType {

        private List<string> _types {get;set;}

        public VideoType() {
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
        }

        public bool IsVideo(string extension) {
            return _types.Contains(extension);
        }

    }

}