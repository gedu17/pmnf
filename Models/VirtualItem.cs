
using System;
using System.ComponentModel.DataAnnotations.Schema;
using VidsNet.DataModels;

namespace VidsNet.Models
{
    public class VirtualItem : BaseVirtualItem {
        public override int Id {get;set;}
        public override int UserId {get;set;}

        [ForeignKey("RealItem")]
        public override int? RealItemId {get;set;}
        public override int ParentId {get;set;}
        public override string Name {get;set;}
        public override bool IsViewed {get;set;}
        public override bool IsDeleted {get;set;}
        public override DateTime DeletedTime {get;set;}
        public override DateTime ViewedTime {get;set;}
        public override Enums.Item Type {get;set;}
        public virtual RealItem RealItem {get;set;}
    }
}