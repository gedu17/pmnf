
namespace VidsNet
{
    public class RealItem {
        public int Id {get;set;}
        public int ParentId {get;set;}

        public ItemType Type {get;set;}
        public int UserPathId {get;set;}

        public string Name {get;set;}
        public string Path {get;set;}
        public string Extension {get;set;}
    }
}