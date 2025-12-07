using MediatR;

namespace Ais.Commons.CQRS.Requests;

public interface IRequestMeta : IBaseRequest
{
    string RequestName { get; }
    bool EnableTracing { get; }
    bool EnablePipelineBehavior { get; }
}