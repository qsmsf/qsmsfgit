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
    
    public partial class privilege
    {
        public int privilege_id { get; set; }
        public string privilege_master { get; set; }
        public Nullable<int> privilege_value { get; set; }
        public string privilege_access { get; set; }
        public Nullable<int> privilege_access_value { get; set; }
        public Nullable<bool> privilege_operation { get; set; }
    }
}
