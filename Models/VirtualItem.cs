
using System;
using System.ComponentModel.DataAnnotations.Schema;
using VidsNet.DataModels;

namespace VidsNet.Models
{
    public class VirtualItem {
        public int Id {get;set;}
        public int UserId {get;set;}

        [ForeignKey("RealItem")]
        public int? RealItemId {get;set;}
        public int ParentId {get;set;}
        public string Name {get;set;}
        public bool IsViewed {get;set;}
        public bool IsDeleted {get;set;}
        public DateTime DeletedTime {get;set;}
        public DateTime ViewedTime {get;set;}
        public Enums.Item Type {get;set;}
        public virtual RealItem RealItem {get;set;}
    }
}