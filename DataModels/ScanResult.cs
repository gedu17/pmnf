using System.Collections.Generic;

namespace VidsNet.DataModels
{
    public class ScanResult
    {
        public List<Item> NewItems {get;set;}
        public List<Item> DeletedItems {get;set;}

        public ScanResult() {
            NewItems = new List<Item>();
            DeletedItems = new List<Item>();
        }
    }
}