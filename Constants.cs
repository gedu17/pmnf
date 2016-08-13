using VidsNet.Enums;

namespace VidsNet
{   
    public class Constants
    {
        public static string Issuer = "VidsNet";
        public static DatabaseType DatabaseType = DatabaseType.Sqlite;
        public static bool IsSqlite = DatabaseType == DatabaseType.Sqlite;
        public static int KeyDerivationIterationCount = 10000;
        public static int SaltSize = 128 / 8;
        public static int PasswordSize = 256 / 8;
        
    }
    
}