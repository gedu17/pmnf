using System.IO;
using Microsoft.Extensions.Logging;
using VidsNet.Enums;
using VidsNet.DataModels;
using Microsoft.AspNetCore.Http;
using VidsNet.Models;

namespace VidsNet.Classes
{
    public class VideoScanner : BaseScanner
    {
    public VideoScanner(ILoggerFactory logger, IHttpContextAccessor accessor, DatabaseContext db)
     : base(logger.CreateLogger("VideoScanner"), accessor, db) {
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