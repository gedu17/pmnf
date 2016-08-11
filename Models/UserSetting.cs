
using System;

namespace VidsNet.Models
{
    public class UserSetting : IEquatable<UserSetting> {
        public int Id {get;set;}

        public int UserId {get;set;}
        public string Name {get;set;}
        public string Value {get;set;}
        public string Description {get;set;}

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}.{2}", UserId, Name, Value).GetHashCode();
        }

        bool IEquatable<UserSetting>.Equals(UserSetting other)
        {
            if(other.Value.Equals(Value) && other.UserId == UserId && other.Name.Equals(Name)) {
                return true;
            }
            return false;
        }
    }
}