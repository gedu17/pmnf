using System;
using System.Collections.Generic;
using VidsNet.Enums;

namespace VidsNet.DataModels
{
    public class Item : IEquatable<Item> {

        public string Path { get; set; } 
        public int Id { get; set; }
        public int UserPathId {get;set;}
        public ItemType Type {get;set;}
        public bool WriteVirtualItem {get;set;}
        public List<Item> Children {get;set;}

        public Item () {
            Children = new List<Item>();
            WriteVirtualItem = true;
        }


        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", Path, Type).GetHashCode();
        }

        bool IEquatable<Item>.Equals(Item other)
        {
            if(other.Path.Equals(Path)) {
                return true;
            }
            return false;
        }
    }
}