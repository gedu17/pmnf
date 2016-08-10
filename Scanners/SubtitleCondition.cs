using System.IO;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Interfaces;

namespace VidsNet.Scanners
{
    public class SubtitleCondition : IScannerCondition {
        public CheckTypeResult CheckType(string filePath)
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