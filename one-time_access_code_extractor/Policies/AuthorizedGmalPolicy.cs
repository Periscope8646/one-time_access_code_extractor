using Polly;

namespace one_time_access_code_extractor.Policies;

public class AuthorizedGmailPolicy : IAuthorizedGmailAsyncPolicy
{
    private readonly IAsyncPolicy _innerPolicy;

    public AuthorizedGmailPolicy(IAsyncPolicy innerPolicy)
    {
        _innerPolicy = innerPolicy;
    }

    public string PolicyKey => _innerPolicy.PolicyKey;
    public IAsyncPolicy WithPolicyKey(string policyKey) => _innerPolicy.WithPolicyKey(policyKey);

    public Task ExecuteAsync(Func<Task> action) => _innerPolicy.ExecuteAsync(action);

    public Task ExecuteAsync(Func<Context, Task> action, IDictionary<string, object> contextData) =>
        _innerPolicy.ExecuteAsync(action, contextData);

    public Task ExecuteAsync(Func<Context, Task> action, Context context) => _innerPolicy.ExecuteAsync(action, context);

    public Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken) =>
        _innerPolicy.ExecuteAsync(action, cancellationToken);

    public Task ExecuteAsync(Func<Context, CancellationToken, Task> action, IDictionary<string, object> contextData,
        CancellationToken cancellationToken) => _innerPolicy.ExecuteAsync(action, contextData, cancellationToken);

    public Task ExecuteAsync(Func<Context, CancellationToken, Task> action, Context context,
        CancellationToken cancellationToken) => _innerPolicy.ExecuteAsync(action, context, cancellationToken);

    public Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken,
        bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAsync(action, cancellationToken, continueOnCapturedContext);

    public Task ExecuteAsync(Func<Context, CancellationToken, Task> action, IDictionary<string, object> contextData,
        CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAsync(action, contextData, cancellationToken, continueOnCapturedContext);

    public Task ExecuteAsync(Func<Context, CancellationToken, Task> action, Context context,
        CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAsync(action, context, cancellationToken, continueOnCapturedContext);

    public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action) => _innerPolicy.ExecuteAsync(action);

    public Task<TResult> ExecuteAsync<TResult>(Func<Context, Task<TResult>> action, Context context) =>
        _innerPolicy.ExecuteAsync(action, context);

    public Task<TResult> ExecuteAsync<TResult>(Func<Context, Task<TResult>> action,
        IDictionary<string, object> contextData) => _innerPolicy.ExecuteAsync(action, contextData);

    public Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken) => _innerPolicy.ExecuteAsync(action, cancellationToken);

    public Task<TResult> ExecuteAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action,
        IDictionary<string, object> contextData, CancellationToken cancellationToken) =>
        _innerPolicy.ExecuteAsync(action, contextData, cancellationToken);

    public Task<TResult> ExecuteAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context,
        CancellationToken cancellationToken) => _innerPolicy.ExecuteAsync(action, context, cancellationToken);

    public Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAsync(action, cancellationToken, continueOnCapturedContext);

    public Task<TResult> ExecuteAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action,
        IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAsync(action, contextData, cancellationToken, continueOnCapturedContext);

    public Task<TResult> ExecuteAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context,
        CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAsync(action, context, cancellationToken, continueOnCapturedContext);

    public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Task> action) => _innerPolicy.ExecuteAndCaptureAsync(action);

    public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, Task> action,
        IDictionary<string, object> contextData) => _innerPolicy.ExecuteAndCaptureAsync(action, contextData);

    public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, Task> action, Context context) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, context);

    public Task<PolicyResult> ExecuteAndCaptureAsync(Func<CancellationToken, Task> action,
        CancellationToken cancellationToken) => _innerPolicy.ExecuteAndCaptureAsync(action, cancellationToken);

    public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task> action,
        IDictionary<string, object> contextData, CancellationToken cancellationToken) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, contextData, cancellationToken);

    public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task> action, Context context,
        CancellationToken cancellationToken) => _innerPolicy.ExecuteAndCaptureAsync(action, context, cancellationToken);

    public Task<PolicyResult> ExecuteAndCaptureAsync(Func<CancellationToken, Task> action,
        CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, cancellationToken, continueOnCapturedContext);

    public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task> action,
        IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, contextData, cancellationToken, continueOnCapturedContext);

    public Task<PolicyResult> ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task> action, Context context,
        CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, context, cancellationToken, continueOnCapturedContext);

    public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Task<TResult>> action) =>
        _innerPolicy.ExecuteAndCaptureAsync(action);

    public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Context, Task<TResult>> action,
        IDictionary<string, object> contextData) => _innerPolicy.ExecuteAndCaptureAsync(action, contextData);

    public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<Context, Task<TResult>> action,
        Context context) => _innerPolicy.ExecuteAndCaptureAsync(action, context);

    public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken) => _innerPolicy.ExecuteAndCaptureAsync(action, cancellationToken);

    public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(
        Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData,
        CancellationToken cancellationToken) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, contextData, cancellationToken);

    public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(
        Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, context, cancellationToken);

    public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, cancellationToken, continueOnCapturedContext);

    public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(
        Func<Context, CancellationToken, Task<TResult>> action, IDictionary<string, object> contextData,
        CancellationToken cancellationToken, bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, contextData, cancellationToken, continueOnCapturedContext);

    public Task<PolicyResult<TResult>> ExecuteAndCaptureAsync<TResult>(
        Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken,
        bool continueOnCapturedContext) =>
        _innerPolicy.ExecuteAndCaptureAsync(action, context, cancellationToken, continueOnCapturedContext);
}