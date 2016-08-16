using System.IO;
using VidsNet.DataModels;
using VidsNet.Enums;
using VidsNet.Interfaces;

namespace VidsNet.Scanners
{
    public class DirectoryCondition : IScannerCondition {
        public ScannerConditionResult CheckType(string filePath)
        {
            if(Directory.Exists(filePath))
            {
                return new ScannerConditionResult() { CorrectType = true, Type = Item.Folder, WriteVirtualItem = true };
            }
            return new ScannerConditionResult() { CorrectType = false, Type = Item.None, WriteVirtualItem = false };
        }
    }
}