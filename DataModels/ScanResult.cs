using System.Collections.Generic;

namespace VidsNet.DataModels
{
    public class ScanResult
    {
        public List<ScanItem> NewItems {get;set;}
        public List<ScanItem> DeletedItems {get;set;}
        public int NewItemsCount {get;set;}
        public int DeletedItemsCount {get;set;}

        public ScanResult() {
            NewItems = new List<ScanItem>();
            DeletedItems = new List<ScanItem>();
            NewItemsCount = 0;
            DeletedItemsCount = 0;
        }
    }
}