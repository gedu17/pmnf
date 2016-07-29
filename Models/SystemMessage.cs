
namespace VidsNet.Models
{
    public class SystemMessage {
        public int Id {get;set;}
        public int UserId {get;set;}

        public string Message {get;set;}
        public int Read {get;set;}

        public int Severity {get;set;}
    }
}