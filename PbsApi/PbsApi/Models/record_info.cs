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
    
    public partial class record_info
    {
        public int sid { get; set; }
        public string uuid { get; set; }
        public string record_no { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public Nullable<int> creater_id { get; set; }
        public Nullable<System.DateTime> create_time { get; set; }
        public Nullable<int> state { get; set; }
        public Nullable<int> type { get; set; }
        public string avatar_url { get; set; }
        public string record_loc { get; set; }
        public string record_type { get; set; }
        public string loc_name { get; set; }
        public string loc_disp { get; set; }
    }
}
