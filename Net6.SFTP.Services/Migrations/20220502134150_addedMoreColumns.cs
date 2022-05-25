using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Net6.SFTP.Services.Migrations
{
    public partial class addedMoreColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MovingTime",
                table: "SftpFileInfos",
                newName: "LastWriteTime");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "SftpFileInfos",
                newName: "RemoteFilePath");

            migrationBuilder.AddColumn<DateTime>(
                name: "FileDowloadTime",
                table: "SftpFileInfos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessTime",
                table: "SftpFileInfos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalFilePath",
                table: "SftpFileInfos",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileDowloadTime",
                table: "SftpFileInfos");

            migrationBuilder.DropColumn(
                name: "LastAccessTime",
                table: "SftpFileInfos");

            migrationBuilder.DropColumn(
                name: "LocalFilePath",
                table: "SftpFileInfos");

            migrationBuilder.RenameColumn(
                name: "RemoteFilePath",
                table: "SftpFileInfos",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "LastWriteTime",
                table: "SftpFileInfos",
                newName: "MovingTime");
        }
    }
}
