namespace Tutorial__12.DTOs
{
    public class TripResponseDto
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public int AllPages { get; set; }
        public List<TripDto> Trips { get; set; } = new List<TripDto>();
    }

    public class TripDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }
        public List<CountryDto> Countries { get; set; } = new List<CountryDto>();
        public List<ClientDto> Clients { get; set; } = new List<ClientDto>();
    }

    public class CountryDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class ClientDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}