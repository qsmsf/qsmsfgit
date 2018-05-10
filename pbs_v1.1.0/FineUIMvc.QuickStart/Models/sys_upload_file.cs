//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace FineUIMvc.QuickStart.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class sys_upload_file
    {
        public int file_id { get; set; }

        [Display(Name = "文件名")]
        public string file_name { get; set; }

        [Display(Name = "文件备注")]
        public string file_hint { get; set; }
        [Display(Name = "文件地址")]
        public string file_url { get; set; }
        [Display(Name = "提交人")]
        public Nullable<int> file_uploader { get; set; }

        [Display(Name = "提交时间")]
        public Nullable<System.DateTime> file_upload_time { get; set; }
        [Display(Name = "所属记录")]
        public Nullable<int> record_id { get; set; }
        [Display(Name = "类型")]
        [Required]
        public Nullable<int> file_type { get; set; }
        [Display(Name = "关联id")]
        public string rec_uuid { get; set; }
    }
}
