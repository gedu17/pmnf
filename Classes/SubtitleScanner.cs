using System.IO;
using Microsoft.Extensions.Logging;
using VidsNet.Enums;
using VidsNet.DataModels;

namespace VidsNet.Classes
{
    public class SubtitleScanner : BaseScanner
    {
        public SubtitleScanner(ILogger logger, int userId) : base(logger, userId) {
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