using System;
using VidsNet.Enums;
using VidsNet.DataModels;

namespace VidsNet.Models
{
    public class VirtualItemSqlite : BaseVirtualItem {
        public override int Id {get;set;}
        public override int UserId {get;set;}
        public override int? RealItemId {get;set;}
        public override int ParentId {get;set;}
        public override string Name {get;set;}
        public override bool IsViewed {get;set;}
        public override bool IsDeleted {get;set;}
        public override DateTime DeletedTime {get;set;}
        public override DateTime ViewedTime {get;set;}
        public override Item Type {get;set;}
    }
}