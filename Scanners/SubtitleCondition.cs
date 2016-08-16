using System.IO;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Interfaces;

namespace VidsNet.Scanners
{
    public class SubtitleCondition : IScannerCondition {
        public ScannerConditionResult CheckType(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var subtitle = new Subtitle();
            if(subtitle.IsSubtitle(extension))
            {
                return new ScannerConditionResult() { CorrectType = true, Type = Item.Subtitle, WriteVirtualItem = false };
            }
            return new ScannerConditionResult() { CorrectType = false, Type = Item.None, WriteVirtualItem = false };
        }
    }
}