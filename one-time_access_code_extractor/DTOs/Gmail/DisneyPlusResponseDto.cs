namespace one_time_access_code_extractor.DTOs.Gmail;

public class DisneyPlusResponseDto
{
    public DateTime? ReceivedAt { get; set; }
    public string? Code { get; set; }
    public string Message { get; set; }
    public bool Found { get; set; }

    public DisneyPlusResponseDto(DateTime? receivedAt, string? code, string? message = null)
    {
        ReceivedAt = receivedAt;
        Code = code;
        Found = Code != null;
        Message = message ?? "No code found";
    }

    public DisneyPlusResponseDto(int searchHoursLimit)
    {
        ReceivedAt = null;
        Code = null;
        Message = $"No access code was found in any email from the last {searchHoursLimit*-1} hours.";
    }
}