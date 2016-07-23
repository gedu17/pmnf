using System.Linq;
namespace VidsNet
{
    public static class Setup {

        public static bool UsersExist() {
            using(var db = new DatabseContext()) {
                if(db.Users.Any()) {
                    return true;
                }
                return false;
            }
        }

        public static bool SettingsExist() {
            using(var db = new DatabseContext()) {
                if(db.Settings.Any()) {
                    return true;
                }
                return false;
            }
        }

        public static bool IsSetup() {
            return (Setup.UsersExist() && Setup.SettingsExist());
        }

    }
}