using System.IO;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Interfaces;

namespace VidsNet.Scanners
{
    public class VideoCondition : IScannerCondition {
        public CheckTypeResult CheckType(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var videoType = new VideoType();
            if(videoType.IsVideo(extension))
            {
                return new CheckTypeResult() { CorrectType = true, Type = ItemType.Video, WriteVirtualItem = true };
            }
            return new CheckTypeResult() { CorrectType = false, Type = ItemType.None, WriteVirtualItem = false };
        }
    }
}