//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PbsApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class sys_version
    {
        public int sid { get; set; }
        public Nullable<int> version_code { get; set; }
        public string version_name { get; set; }
        public string apk_name { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public string version_hint { get; set; }
        public Nullable<int> download_count { get; set; }
    }
}
