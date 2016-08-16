using System;
using VidsNet.Enums;

namespace VidsNet.DataModels
{
    public abstract class BaseVirtualItem {
        public abstract int Id {get;set;}
        public abstract int UserId {get;set;}
        public abstract int RealItemId {get;set;}
        public abstract int ParentId {get;set;}
        public abstract string Name {get;set;}
        public abstract bool IsViewed {get;set;}
        public abstract bool IsDeleted {get;set;}
        public abstract DateTime DeletedTime {get;set;}
        public abstract DateTime ViewedTime {get;set;}
        public abstract Item Type {get;set;}
    }
}