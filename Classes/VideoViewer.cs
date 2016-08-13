using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VidsNet.Enums;
using VidsNet.Models;
using VidsNet.DataModels;

namespace VidsNet.Classes
{
    public class VideoViewer : Controller {

        private BaseDatabaseContext _db;
        private string _extension;
        private VideoType _videoType;
        private SubtitleType _subtitleType;
        private RealItem _item;
        private string _name;
        private long _offset;
        private int _count;
        private string _contentRange;
        private string _fileSize;
        private FileInfo _fileInfo;
        private int _packetSize;
        private long _left;
        private long _right;
        private string _range;
        private VideoViewResult _result;
        
        public VideoViewer(BaseDatabaseContext db, VideoType videoType, SubtitleType subtitleType) {
            _db = db;
            _videoType = videoType;
            _subtitleType = subtitleType;
            _offset = 0;
            _count = 0;
            _contentRange = string.Empty;
            _fileSize = string.Empty;
            _extension = string.Empty;
            _packetSize  = 64 * 1024 * 1024; //64MB
            _result = new VideoViewResult();
        }

        public VideoViewResult View(int id, int userId, string name, string range) {
            _range = range;
            _name = name;
            _extension = Path.GetExtension(name);
            if(ViewItemExists(id, userId)) {
                if(_videoType.IsVideo(_extension)) {
                    return ViewVideo();
                }
                else if(_subtitleType.IsSubtitle(_extension)) {
                    return ViewSubtitle();     
                }
            }

            return _result;
        }

        private VideoViewResult ViewSubtitle() {
            //x.Name == name &&
            //TODO: find a way to transform changed name to original
            var subtitle = _db.RealItems.Where(x =>  x.Extension == _extension 
            && x.ParentId == _item.ParentId).FirstOrDefault();
            if(subtitle is RealItem) {
                using(var fs = new FileStream(subtitle.Path, FileMode.Open, FileAccess.Read)) {
                    var byteArray = new byte[fs.Length+1];
                    fs.Read(byteArray, 0, (int)fs.Length);
                    var ret = new FileContentResult(byteArray, _subtitleType.GetMime(_extension));
                    ret.FileDownloadName = _name;
                    _result.ActionResult = ret;
                    _result.ContentLength = fs.Length;
                    _result.StatusCode = 200;
                    return _result;
                }
            } 
                
            return _result;                
        }

        private VideoViewResult BadRange() {
            _result.ContentRange = "bytes: */" + _fileSize;
            _result.ContentLength = 0;
            _result.StatusCode = 416;
            _result.ActionResult = new EmptyResult();
            return _result;
        }

        private VideoViewResult ViewVideoNoStart() {
            /*if(_right > _fileInfo.Length) {
                return BadRange();
            }
            else {*/
            if(_right <= _fileInfo.Length) {
                _offset = (int)_fileInfo.Length - _right;
                if(_right > _packetSize) {
                    _contentRange = string.Format("{0}-{1}", (_fileInfo.Length - _right), _packetSize);
                    _count = _packetSize;
                }
                else {
                    _contentRange = string.Format("{0}-{1}", (_fileInfo.Length - _right), _fileSize);
                    _count = (int)_right;
                }
                
                return ReturnVideo();
            }

            return BadRange();
        }

        private VideoViewResult ViewVideoNoEnd() {            
            /*if(_left > _fileInfo.Length) {
                return BadRange();
            }
            else {*/
            if(_left <= _fileInfo.Length) {
                var count = (int)(_fileInfo.Length - _left);
                _offset = _left;
                if(count > _packetSize) {
                    _contentRange = string.Format("{0}-{1}", _left, _packetSize);
                    _count = _packetSize;
                }
                else {
                    _contentRange = string.Format("{0}-{1}", _left, _fileSize);
                    _count = count;
                }
                
                return ReturnVideo();
            }

            return BadRange();
        }

        private VideoViewResult ViewVideoRange() {
            /*if(_left > _fileInfo.Length || _right > _fileInfo.Length) {
                return BadRange();
            }
            else {*/
            if(_left <= _fileInfo.Length && _right <= _fileInfo.Length) {
                var count = (int)(_right - _left + 1);
                _offset = _left;
                if(count > _packetSize) {
                    _contentRange = string.Format("{0}-{1}", _left, _packetSize);
                    _count = _packetSize;
                }
                else {
                    _contentRange = string.Format("{0}-{1}", _left, _right);
                    _count = count;
                }
                
                return ReturnVideo();
            }

            return BadRange();
        }

        /*private IActionResult ViewVideoNoRange() {
            if(_packetSize > _fileInfo.Length) {
                _contentRange = string.Format("0-{0}", _fileInfo.Length - 1);
                _count = (int)_fileInfo.Length;
            }
            else {
                _contentRange = string.Format("0-{0}", _packetSize - 1);
                _count = _packetSize;
            }

            return ReturnVideo();
        }*/

        private VideoViewResult ReturnVideo() {
            using(var fs = new FileStream(_item.Path, FileMode.Open, FileAccess.Read)) {
                var byteArray = new byte[_count];
                fs.Position = _offset;
                //TODO: use async version
                fs.Read(byteArray, 0, _count);
                var ret = new FileContentResult(byteArray, _videoType.GetMime(_extension)); 
                ret.FileDownloadName = _name;
                _result.ContentRange = "bytes " + _contentRange + "/" + _fileSize;
                _result.ContentLength = _count;
                _result.StatusCode = 206;
                _result.ActionResult = ret;
                return _result;
            }
        }

        private void SetUpRange() {
            var range = _range.Replace("bytes=", string.Empty)
            .Replace("bytes: ", string.Empty).Split('-');

            _left = -1;
            _right = -1;
            if(!string.IsNullOrWhiteSpace(range[0])) {
                _left = long.Parse(range[0]);
            }
            if(!string.IsNullOrWhiteSpace(range[1])) {
                _right = long.Parse(range[1]);
            }
        }

        private VideoViewResult FullVideo() {
            var memStream = new MemoryStream();
            long length = 0;
            using(var fs = new FileStream(_item.Path, FileMode.Open, FileAccess.Read)) {
                fs.CopyTo(memStream);
                length = fs.Length;
            }

            memStream.Position = 0;
            var ret = new FileStreamResult(memStream, _videoType.GetMime(_extension));
            ret.FileDownloadName = _name;
            _result.StatusCode = 200;
            _result.ContentLength = length;
            _result.ActionResult = ret;
            return _result;
        }

        private bool ViewItemExists(int id, int userId) {
            var virtualItem = _db.VirtualItems.Where(x => x.Id == id && x.UserId == userId).FirstOrDefault();
            //TODO: MIGHT BE BUGGED AFTER CHANGING TO BaseVirtualItem!
            if(virtualItem is BaseVirtualItem) {
                var realItem = _db.RealItems.Where(x => x.Id == virtualItem.RealItemId).FirstOrDefault();
                if(realItem is RealItem) {
                    _item = realItem;
                    return true;
                }
            }
            return false;
        }

        private VideoViewResult ViewVideo() {
            _fileInfo = new FileInfo(_item.Path);
            _fileSize = (_fileInfo.Length -1).ToString();

            if(!string.IsNullOrWhiteSpace(_range)) {
                SetUpRange();

                if(_left == -1) {
                    return ViewVideoNoStart(); 
                }
                else if(_right == -1) {
                    return ViewVideoNoEnd();
                }
                else {
                    return ViewVideoRange();
                }
            }
            else {
                return FullVideo();
            }
        }
    }

}