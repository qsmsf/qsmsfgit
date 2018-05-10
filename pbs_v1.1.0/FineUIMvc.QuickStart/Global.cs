using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FineUIMvc.QuickStart
{
    public class Global
    {
        public const int SELF_DATA = 1001;
        public const int UNIT_DATA = 1002;
        public const int FAMILY_DATA = 1003;
        public const int ALL_DATA = 1004;
        public const int FATHER_DATA = 1005;
        public class LoginInfo
        {
            public int user_id { get; set; }
            public string user_pwd { get; set; }
            public string user_fullname { get; set; }
            public int unit_id { get; set; }
            public string unit_name { get; set; }
            public string user_zw { get; set; }
            public string privList { get; set; }
        }

        public class UserPriv
        {
            public string priv_access { get; set; }
            public int priv_access_value { get; set; }
            public bool priv_oper { get; set; }
        }

        public static List<UserPriv> userPrivList = new List<UserPriv>();
        public static LoginInfo loginInfo = new LoginInfo();
    }
}