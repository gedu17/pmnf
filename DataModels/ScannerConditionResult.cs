using VidsNet.Enums;

namespace VidsNet.DataModels
{
    public class ScannerConditionResult {
        public bool CorrectType {get;set;}
        public bool WriteVirtualItem {get;set;}
        public Item Type {get;set;}
    }
}