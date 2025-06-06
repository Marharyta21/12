using Microsoft.EntityFrameworkCore;
using Tutorial__12.Data;
using Tutorial__12.Models;
using Tutorial__12.DTOs;

namespace Tutorial__12.Services
{
    public class TripService : ITripService
    {
        private readonly MasterContext _context;

        public TripService(MasterContext context)
        {
            _context = context;
        }

        public async Task<TripResponseDto> GetTripsAsync(int page = 1, int pageSize = 10)
        {
            var totalTrips = await _context.Trips.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalTrips / pageSize);

            var trips = await _context.Trips
                .Include(t => t.IdCountries)
                .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
                .OrderByDescending(t => t.DateFrom)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TripDto
                {
                    Name = t.Name,
                    Description = t.Description,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo,
                    MaxPeople = t.MaxPeople,
                    Countries = t.IdCountries.Select(c => new CountryDto
                    {
                        Name = c.Name
                    }).ToList(),
                    Clients = t.ClientTrips.Select(ct => new ClientDto
                    {
                        FirstName = ct.IdClientNavigation.FirstName,
                        LastName = ct.IdClientNavigation.LastName
                    }).ToList()
                })
                .ToListAsync();

            return new TripResponseDto
            {
                PageNum = page,
                PageSize = pageSize,
                AllPages = totalPages,
                Trips = trips
            };
        }

        public async Task<bool> TripExistsAsync(int tripId)
        {
            return await _context.Trips.AnyAsync(t => t.IdTrip == tripId);
        }

        public async Task<bool> IsTripInFutureAsync(int tripId)
        {
            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == tripId);
            return trip != null && trip.DateFrom > DateTime.Now;
        }

        public async Task<bool> IsClientRegisteredForTripAsync(string pesel, int tripId)
        {
            return await _context.ClientTrips
                .AnyAsync(ct => ct.IdClientNavigation.Pesel == pesel && ct.IdTrip == tripId);
        }

        public async Task AddClientToTripAsync(AddClientToTripDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingClient = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);

                if (existingClient != null)
                    throw new InvalidOperationException("Client with this PESEL already exists");
        
                if (await IsClientRegisteredForTripAsync(dto.Pesel, dto.IdTrip))
                    throw new InvalidOperationException("Client is already registered for this trip");
        
                if (!await TripExistsAsync(dto.IdTrip))
                    throw new InvalidOperationException("Trip does not exist");
        
                if (!await IsTripInFutureAsync(dto.IdTrip))
                    throw new InvalidOperationException("Cannot register for a trip that has already occurred");
        
                var newClient = new Client
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Telephone = dto.Telephone,
                    Pesel = dto.Pesel
                };

                _context.Clients.Add(newClient);
                await _context.SaveChangesAsync();
        
                var clientTrip = new ClientTrip
                {
                    IdClient = newClient.IdClient,
                    IdTrip = dto.IdTrip,
                    RegisteredAt = DateTime.Now,
                    PaymentDate = dto.PaymentDate
                };

                _context.ClientTrips.Add(clientTrip);
                await _context.SaveChangesAsync();
        
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task AddCountryToTripAsync(int tripId, int countryId)
        {
            var trip = await _context.Trips
                .Include(t => t.IdCountries)
                .FirstOrDefaultAsync(t => t.IdTrip == tripId);

            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.IdCountry == countryId);

            if (trip == null || country == null)
                throw new InvalidOperationException("Trip or Country not found");

            if (!trip.IdCountries.Contains(country))
            {
                trip.IdCountries.Add(country);
                await _context.SaveChangesAsync();
            }
        }
    }
}
