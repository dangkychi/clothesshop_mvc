namespace BanHangThoiTrangMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateLikeProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tb_LikeProduct",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ProductId = c.Int(nullable: false),
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
            DropForeignKey("dbo.tb_LikeProduct", "ProductId", "dbo.tb_Product");
            DropIndex("dbo.tb_LikeProduct", new[] { "ProductId" });
            DropTable("dbo.tb_LikeProduct");
        }
    }
}
