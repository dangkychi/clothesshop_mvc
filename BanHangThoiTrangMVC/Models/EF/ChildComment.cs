using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHangThoiTrangMVC.Models.EF
{
    [Table("tb_ChildComment")]
    public class ChildComment : CommonAbstract
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PCommentId { get; set; }

        [StringLength(250)]
        public string Text { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public DateTime Comment_Date { get; set; }

        public virtual ParentComment ParentComment { get; set; }
    }
}