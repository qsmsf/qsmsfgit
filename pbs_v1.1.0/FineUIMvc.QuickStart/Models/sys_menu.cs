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
    
    public partial class sys_menu
    {
        public int menu_id { get; set; }
        public string menu_name { get; set; }
        public string menu_disp { get; set; }
        public string menu_url { get; set; }
        public string menu_Icon { get; set; }
        public Nullable<bool> isVisible { get; set; }
        public Nullable<int> parent_id { get; set; }
        public Nullable<bool> isLeaf { get; set; }
    }
}