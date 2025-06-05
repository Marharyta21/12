using Tutorial__12.DTOs;

namespace Tutorial__12.Services
{
    public interface ITripService
    {
        Task<TripResponseDto> GetTripsAsync(int page = 1, int pageSize = 10);
        Task AddClientToTripAsync(AddClientToTripDto dto);
    }
}