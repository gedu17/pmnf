
using System;
using System.ComponentModel.DataAnnotations.Schema;
using VidsNet.Enums;

namespace VidsNet.Models
{
    //Foreign key not supported by sqlite driver
    public class VirtualItem {
        public int Id {get;set;}
        public int UserId {get;set;}
        //[ForeignKey("RealItem")]
        public int RealItemId {get;set;}
        public int ParentId {get;set;}
        public string Name {get;set;}
        public bool IsSeen {get;set;}
        public bool IsDeleted {get;set;}
        public DateTime DeletedTime {get;set;}
        public DateTime SeenTime {get;set;}
        public ItemType Type {get;set;}
        //public virtual RealItem RealItem {get;set;}
    }
}