namespace SmartTaskManagement.Application.DTOs.Shared
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    // public class PagedResponse<T>
    // {
    //     public IEnumerable<T> Items { get; set; } = new List<T>();
    //     public int PageNumber { get; set; }
    //     public int PageSize { get; set; }
    //     public int TotalCount { get; set; }
    //     public int TotalPages { get; set; }
    //     public bool HasPreviousPage => PageNumber > 1;
    //     public bool HasNextPage => PageNumber < TotalPages;
    // }
}