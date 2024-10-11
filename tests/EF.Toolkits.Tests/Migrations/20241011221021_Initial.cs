using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace EF.Toolkits.Tests.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Идентификатор.")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true, comment: "Кличка."),
                    Species = table.Column<string>(type: "text", nullable: true, comment: "Порода."),
                    AnimalType = table.Column<int>(type: "integer", nullable: false, comment: "Тип.\n\n0 - Собакен.\n1 - Кошька.\n2 - Рыбка.\n3 - Игрушка?")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                },
                comment: "Сущность \"Живтоное\".");

            migrationBuilder.CreateTable(
                name: "Emploees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    SecondName = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    Company = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emploees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Figures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Shape = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Figures", x => x.Id);
                });

            migrationBuilder.Sql("Body_Up");

            migrationBuilder.Sql("CREATE FUNCTION Test_AfterDelete() RETURNS trigger as $Test_AfterDelete$\r\nBEGIN\r\nSELECT 1\r\nRETURN OLD;\r\nEND;\r\n$Test_AfterDelete$ LANGUAGE plpgsql;\r\n\r\nCREATE TRIGGER Test_AfterDelete AFTER DELETE\r\nON Animals\r\nFOR EACH ROW EXECUTE PROCEDURE Test_AfterDelete();\r\n");

            migrationBuilder.Sql("CREATE FUNCTION Test_BeforeInsert() RETURNS trigger as $Test_BeforeInsert$\r\nBEGIN\r\nSELECT 1\r\nRETURN NEW;\r\nEND;\r\n$Test_BeforeInsert$ LANGUAGE plpgsql;\r\n\r\nCREATE TRIGGER Test_BeforeInsert BEFORE INSERT\r\nON Animals\r\nFOR EACH ROW EXECUTE PROCEDURE Test_BeforeInsert();\r\n");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Body_Down");

            migrationBuilder.Sql("DROP FUNCTION Test_AfterDelete() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION Test_BeforeInsert() CASCADE;");

            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "Emploees");

            migrationBuilder.DropTable(
                name: "Figures");
        }
    }
}
