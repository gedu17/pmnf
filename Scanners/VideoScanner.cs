using System.IO;
using Microsoft.Extensions.Logging;
using VidsNet.Enums;
using VidsNet.DataModels;
using Microsoft.AspNetCore.Http;
using VidsNet.Models;

namespace VidsNet.Scanners
{
    public class VideoScanner : BaseScanner
    {
    public VideoScanner(ILoggerFactory logger, UserData userData, DatabaseContext db)
     : base(logger.CreateLogger("VideoScanner"), db, userData) {
    }
        protected override CheckTypeResult CheckType(string filePath)
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