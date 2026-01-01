namespace one_time_access_code_extractor.DTOs.Gmail;

public class EmailMetaData
{
    public string Id { get; set; }
    public string Subject { get; set; }
    public DateTime ReceivedAt { get; set; }

    public EmailMetaData(string id, string subject, DateTime receivedAt)
    {
        Id = id;
        Subject = subject;
        ReceivedAt = receivedAt;
    }
}