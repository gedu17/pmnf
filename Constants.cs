namespace VidsNet
{   
    public class Constants
    {
        public static string Issuer = "VidsNet";
        public static int KeyDerivationIterationCount = 10000;
        public static int SaltSize = 128 / 8;
        public static int PasswordSize = 256 / 8;
        public static double CookieExpiryTime = 20160;        
    }
    
}