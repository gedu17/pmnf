using System;
using System.Collections.Generic;
using VidsNet.Enums;

namespace VidsNet.DataModels
{
    public class ScanItem : IEquatable<ScanItem> {

        public string Path { get; set; } 
        public int Id { get; set; }
        public int UserPathId {get;set;}
        public Item Type {get;set;}
        public bool WriteVirtualItem {get;set;}
        public List<ScanItem> Children {get;set;}

        public ScanItem () {
            Children = new List<ScanItem>();
            WriteVirtualItem = true;
        }


        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", Path, Type).GetHashCode();
        }

        bool IEquatable<ScanItem>.Equals(ScanItem other)
        {
            if(other.Path.Equals(Path)) {
                return true;
            }
            return false;
        }
    }
}