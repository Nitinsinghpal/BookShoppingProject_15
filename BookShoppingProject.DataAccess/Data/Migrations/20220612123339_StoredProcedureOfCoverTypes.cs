using Microsoft.EntityFrameworkCore.Migrations;

namespace BookShoppingProject.DataAccess.Migrations
{
    public partial class StoredProcedureOfCoverTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_GetCoverTypes
	
                                    AS
	                                SELECT * from CoverTypes

                                    ");
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_GetCoverType
@Id int
AS
	SELECT * from CoverTypes where Id=@Id
RETURN 0
");
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_CreateCoverType
	@Name varchar(50)
AS
	insert CoverTypes values(@Name)
RETURN 0
");
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_UpdateCoverType
	@Id int,
	@Name varchar(50)
AS
	update CoverTypes set Name=@Name where Id = @Id

");
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_DeleteCoverType
	@Id int
AS
	delete from CoverTypes where Id=@Id

");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
