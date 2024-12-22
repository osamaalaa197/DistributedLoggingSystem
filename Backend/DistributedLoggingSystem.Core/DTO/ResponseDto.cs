namespace DistributedLoggingSystem.Core.DTO
{
    public class ResponseDto
    {
        public object? Result {  get; set; }
        public bool? IsSuccess { get; set; }=true;
        public string? Message { get; set; } 
        public int? TotalRecords { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
