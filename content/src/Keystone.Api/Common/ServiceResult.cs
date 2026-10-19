using System.Diagnostics.CodeAnalysis;
using Keystone.Contracts;

namespace Keystone.Api.Common;

/// <summary>The outcome of a service call: success, or an <see cref="ApiError"/>.</summary>
public class ServiceResult
{
    public ServiceResult()
    {
    }

    public ServiceResult(ApiError error) => Error = error;

    public ApiError? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccessful => Error is null;
}

/// <summary>The outcome of a service call: <see cref="Data"/>, or an <see cref="ApiError"/>.</summary>
public sealed class ServiceResult<T> : ServiceResult
{
    public ServiceResult(T data) => Data = data;

    public ServiceResult(ApiError error)
        : base(error)
    {
    }

    public T? Data { get; }

    // Redeclared so the nullability attributes below can name it.
    public new ApiError? Error => base.Error;

    [MemberNotNullWhen(true, nameof(Data))]
    [MemberNotNullWhen(false, nameof(Error))]
    public new bool IsSuccessful => base.IsSuccessful;
}
