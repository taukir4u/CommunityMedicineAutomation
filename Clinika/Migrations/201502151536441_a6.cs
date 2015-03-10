namespace Clinika.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorEntry", "ServiceCenterCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DoctorEntry", "ServiceCenterCode");
        }
    }
}
