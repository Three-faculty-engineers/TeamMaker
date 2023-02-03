using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teammaker.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sifra = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Indeks = table.Column<int>(type: "int", nullable: false),
                    Fakultet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Bloks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikSndID = table.Column<int>(type: "int", nullable: true),
                    KorisnikRcvID = table.Column<int>(type: "int", nullable: true),
                    Verzija = table.Column<int>(type: "int", nullable: false),
                    Poruka = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bloks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bloks_Korisnici_KorisnikRcvID",
                        column: x => x.KorisnikRcvID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Bloks_Korisnici_KorisnikSndID",
                        column: x => x.KorisnikSndID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Ocene",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikSndID = table.Column<int>(type: "int", nullable: true),
                    KorisnikRcvID = table.Column<int>(type: "int", nullable: true),
                    Vrednost = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocene", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Ocene_Korisnici_KorisnikRcvID",
                        column: x => x.KorisnikRcvID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Ocene_Korisnici_KorisnikSndID",
                        column: x => x.KorisnikSndID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Poruke",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikSndID = table.Column<int>(type: "int", nullable: true),
                    KorisnikRcvID = table.Column<int>(type: "int", nullable: true),
                    Vreme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tekst = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poruke", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Poruke_Korisnici_KorisnikRcvID",
                        column: x => x.KorisnikRcvID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Poruke_Korisnici_KorisnikSndID",
                        column: x => x.KorisnikSndID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Timovi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaderID = table.Column<int>(type: "int", nullable: true),
                    NeedsMembers = table.Column<bool>(type: "bit", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timovi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Timovi_Korisnici_LeaderID",
                        column: x => x.LeaderID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Invites",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamID = table.Column<int>(type: "int", nullable: true),
                    KorisnikID = table.Column<int>(type: "int", nullable: true),
                    Verzija = table.Column<int>(type: "int", nullable: false),
                    Poruka = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Invites_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Invites_Timovi_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Timovi",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "KorisnikTeam",
                columns: table => new
                {
                    KorisniciID = table.Column<int>(type: "int", nullable: false),
                    TeamsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikTeam", x => new { x.KorisniciID, x.TeamsID });
                    table.ForeignKey(
                        name: "FK_KorisnikTeam_Korisnici_KorisniciID",
                        column: x => x.KorisniciID,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KorisnikTeam_Timovi_TeamsID",
                        column: x => x.TeamsID,
                        principalTable: "Timovi",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Objave",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamID = table.Column<int>(type: "int", nullable: true),
                    KorisnikID = table.Column<int>(type: "int", nullable: true),
                    Vreme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Poruka = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objave", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Objave_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Objave_Timovi_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Timovi",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Sprints",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartSprint = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndSprint = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Sprints_Timovi_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Timovi",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Tagovi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tagovi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tagovi_Timovi_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Timovi",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Taskovi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Poeni = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Vreme = table.Column<int>(type: "int", nullable: false),
                    TeamID = table.Column<int>(type: "int", nullable: true),
                    SprintID = table.Column<int>(type: "int", nullable: true),
                    KorisnikID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taskovi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Taskovi_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Taskovi_Sprints_SprintID",
                        column: x => x.SprintID,
                        principalTable: "Sprints",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Taskovi_Timovi_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Timovi",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bloks_KorisnikRcvID",
                table: "Bloks",
                column: "KorisnikRcvID");

            migrationBuilder.CreateIndex(
                name: "IX_Bloks_KorisnikSndID",
                table: "Bloks",
                column: "KorisnikSndID");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_KorisnikID",
                table: "Invites",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_TeamID",
                table: "Invites",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikTeam_TeamsID",
                table: "KorisnikTeam",
                column: "TeamsID");

            migrationBuilder.CreateIndex(
                name: "IX_Objave_KorisnikID",
                table: "Objave",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Objave_TeamID",
                table: "Objave",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Ocene_KorisnikRcvID",
                table: "Ocene",
                column: "KorisnikRcvID");

            migrationBuilder.CreateIndex(
                name: "IX_Ocene_KorisnikSndID",
                table: "Ocene",
                column: "KorisnikSndID");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_KorisnikRcvID",
                table: "Poruke",
                column: "KorisnikRcvID");

            migrationBuilder.CreateIndex(
                name: "IX_Poruke_KorisnikSndID",
                table: "Poruke",
                column: "KorisnikSndID");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_TeamID",
                table: "Sprints",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Tagovi_TeamID",
                table: "Tagovi",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Taskovi_KorisnikID",
                table: "Taskovi",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Taskovi_SprintID",
                table: "Taskovi",
                column: "SprintID");

            migrationBuilder.CreateIndex(
                name: "IX_Taskovi_TeamID",
                table: "Taskovi",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Timovi_LeaderID",
                table: "Timovi",
                column: "LeaderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bloks");

            migrationBuilder.DropTable(
                name: "Invites");

            migrationBuilder.DropTable(
                name: "KorisnikTeam");

            migrationBuilder.DropTable(
                name: "Objave");

            migrationBuilder.DropTable(
                name: "Ocene");

            migrationBuilder.DropTable(
                name: "Poruke");

            migrationBuilder.DropTable(
                name: "Tagovi");

            migrationBuilder.DropTable(
                name: "Taskovi");

            migrationBuilder.DropTable(
                name: "Sprints");

            migrationBuilder.DropTable(
                name: "Timovi");

            migrationBuilder.DropTable(
                name: "Korisnici");
        }
    }
}
