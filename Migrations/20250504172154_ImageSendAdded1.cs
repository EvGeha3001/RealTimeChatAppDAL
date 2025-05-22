using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeChatAppDAL.Migrations
{
    /// <inheritdoc />
    public partial class ImageSendAdded1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Messages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageMimeType",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ImageMimeType",
                table: "Messages");
        }
    }
}
