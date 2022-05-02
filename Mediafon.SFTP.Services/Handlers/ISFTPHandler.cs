
namespace Mediafon.SFTP.Services.Handlers
{
    public interface ISFTPHandler
    {
        Task<bool> Connect();
        void Disconnect();

    }
}
