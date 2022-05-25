namespace Net6.SFTP.Services.Config
{
    public class SftpSettings
    {
        public string? SftpServer { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Port { get; set; }
        public string? SftpFolderLocation { get; set; }
        public string? LocalFolderLocation { get; set; }
    }
}
