using System.Collections.Generic;

namespace VidsNet.DataModels
{
    public class ScanResult
    {
        public List<Item> NewItems {get;set;}
        public List<Item> DeletedItems {get;set;}
        public int NewItemsCount {get;set;}
        public int DeletedItemsCount {get;set;}

        public ScanResult() {
            NewItems = new List<Item>();
            DeletedItems = new List<Item>();
            NewItemsCount = 0;
            DeletedItemsCount = 0;
        }
    }
}