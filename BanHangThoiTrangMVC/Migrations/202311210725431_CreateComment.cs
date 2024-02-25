namespace BanHangThoiTrangMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_ChildComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PCommentId = c.Int(nullable: false),
                        Text = c.String(maxLength: 250),
                        user_id = c.String(),
                        user_name = c.String(),
                        Comment_Date = c.DateTime(nullable: false),
                        CreateBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                        ParentComment_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_ParentComment", t => t.ParentComment_Id)
                .Index(t => t.ParentComment_Id);
            
            CreateTable(
                "dbo.tb_ParentComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Text = c.String(maxLength: 250),
                        user_id = c.String(),
                        user_name = c.String(),
                        Comment_Date = c.DateTime(nullable: false),
                        CreateBy = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tb_Product", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tb_ParentComment", "ProductId", "dbo.tb_Product");
            DropForeignKey("dbo.tb_ChildComment", "ParentComment_Id", "dbo.tb_ParentComment");
            DropIndex("dbo.tb_ParentComment", new[] { "ProductId" });
            DropIndex("dbo.tb_ChildComment", new[] { "ParentComment_Id" });
            DropTable("dbo.tb_ParentComment");
            DropTable("dbo.tb_ChildComment");
        }
    }
}
