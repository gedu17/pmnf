using System.IO;
using Microsoft.Extensions.Logging;
using VidsNet.Enums;
using VidsNet.DataModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VidsNet.Models;

namespace VidsNet.Scanners
{
    public class SubtitleScanner : BaseScanner
    {
        public SubtitleScanner(ILoggerFactory logger, UserData userData, DatabaseContext db)
         : base(logger.CreateLogger("VideoScanner"), db, userData) {
        }
        protected override CheckTypeResult CheckType(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var subtitleType = new SubtitleType();
            if(subtitleType.IsSubtitle(extension))
            {
                return new CheckTypeResult() { CorrectType = true, Type = ItemType.Subtitle, WriteVirtualItem = false };
            }
            return new CheckTypeResult() { CorrectType = false, Type = ItemType.None, WriteVirtualItem = false };
        }
    }
}