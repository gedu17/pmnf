
using VidsNet.Enums;

namespace VidsNet.DataModels
{
    public class CheckTypeResult {
        public bool CorrectType {get;set;}
        public bool WriteVirtualItem {get;set;}
        public ItemType Type {get;set;}
    }
}