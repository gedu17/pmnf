

using VidsNet.Enums;

namespace VidsNet.DataModels
{
    public class Item {

        public string Path { get; set; } 
        public int Id { get; set; }
        public ItemType Type {get;set;}
    }
}