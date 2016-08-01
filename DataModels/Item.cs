

using System;
using System.Collections;
using VidsNet.Enums;

namespace VidsNet.DataModels
{
    public class Item : IEquatable<Item> {

        public string Path { get; set; } 
        public int Id { get; set; }
        public ItemType Type {get;set;}


        public override int GetHashCode()
        {
            return string.Format("{0}.{1}.{2}", Id, Path, Type).GetHashCode();
        }

        bool IEquatable<Item>.Equals(Item other)
        {
            if(other.Id == Id) {
                return true;
            }
            return false;
        }
    }
}