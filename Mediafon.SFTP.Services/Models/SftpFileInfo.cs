

using System.ComponentModel.DataAnnotations;

namespace Mediafon.SFTP.Services.Models
{
    public class SftpFileInfo
    {
        [Key]
        public string? Id { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public DateTime? MovingTime { get; set; }


    }
}
