using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatAppDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddVoiceMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VoiceContentType",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "VoiceData",
                table: "Messages",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceContentType",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "VoiceData",
                table: "Messages");
        }
    }
}
