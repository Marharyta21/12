namespace Tutorial__12.Services
{
    public interface IClientService
    {
        Task<bool> ClientExistsAsync(int clientId);
        Task DeleteClientAsync(int clientId);
    }
}