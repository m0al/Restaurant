using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOOP_FoodPoint_Restaurant
{
    public static class UserSession
    {
// Stores user information during session.
        public static int UserID { get; set; }
        public static string UserName { get; set; }
        public static string UserRole { get; set; }

// Method to clear session when logging out.
        public static void Logout()
        {
            UserID = 0;
            UserName = string.Empty;
            UserRole = string.Empty;
        }
    }

}
