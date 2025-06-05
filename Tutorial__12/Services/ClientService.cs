using Microsoft.EntityFrameworkCore;
using Tutorial__12.Data;

namespace Tutorial__12.Services
{
    public class ClientService : IClientService
    {
        private readonly MasterContext _context;

        public ClientService(MasterContext context)
        {
            _context = context;
        }

        public async Task<bool> ClientExistsAsync(int clientId)
        {
            return await _context.Clients.AnyAsync(c => c.IdClient == clientId);
        }

        public async Task<bool> ClientHasTripsAsync(int clientId)
        {
            return await _context.ClientTrips.AnyAsync(ct => ct.IdClient == clientId);
        }

        public async Task DeleteClientAsync(int clientId)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null)
                throw new InvalidOperationException("Client not found");

            if (await ClientHasTripsAsync(clientId))
                throw new InvalidOperationException("Cannot delete client with assigned trips");

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }
}