using VidsNet.Enums;

namespace VidsNet.DataModels
{
    public class MenuItem {
        public string Name {get;set;}
        public string Url {get;set;}
        public string Html {get;set;}
        public string OnClick {get;set;}
        public LinkType ItemType {get;set;}

        public MenuItem() {
            Html = string.Empty;
            OnClick = string.Empty;
            ItemType = LinkType.Both;
        }
    }
}