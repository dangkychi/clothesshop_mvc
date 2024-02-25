using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanHangThoiTrangMVC.Models.EF
{
    [Table("tb_ParentComment")]
    public class ParentComment : CommonAbstract
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }

        [StringLength(250)]
        public string Text { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public DateTime Comment_Date { get; set; }

        public virtual Product Product { get; set; }
        public ICollection<ChildComment> ChildComments { get; set; }
    }
}