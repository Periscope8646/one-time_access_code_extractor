using Polly;

namespace one_time_access_code_extractor.Policies;

public interface IAuthorizedGmailAsyncPolicy : IAsyncPolicy { }