namespace JitsiAppointmentApi.Application.DTOs
{
    public class AppointmentResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class AppointmentDetailResponse
    {
        public string Id { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string> AppoinmentsDetailsi { get; set; } = new();
        public List<string> PatientHistory { get; set; } = new();
        public List<string> PatientNotes { get; set; } = new();
    }

    /// <summary>
    /// Pagination metadata for API responses
    /// </summary>
    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }

    /// <summary>
    /// Paginated response wrapper for appointments
    /// </summary>
    public class PaginatedAppointmentsResponse
    {
        public string Status { get; set; } = "success";
        public IEnumerable<AppointmentResponse> Data { get; set; } = new List<AppointmentResponse>();
        public PaginationMetadata Pagination { get; set; } = new();
    }
}
