

using System.ComponentModel.DataAnnotations;

namespace Mediafon.SFTP.Services.Models
{
    public class SftpFileInfo
    {
        [Key]
        public string? Id { get; set; }
        public string? RemoteFilePath { get; set; }
        public string? LocalFilePath { get; set; }
        public string? FileName { get; set; }
        public DateTime? FileDowloadTime { get; set; }
        public DateTime? LastWriteTime { get; set; }
        public DateTime? LastAccessTime { get; set; }


    }
}
