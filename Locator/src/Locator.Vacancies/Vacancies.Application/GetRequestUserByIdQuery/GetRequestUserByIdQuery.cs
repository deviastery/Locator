using Shared.Abstractions;

namespace Vacancies.Application.GetRequestUserByIdQuery;

public record GetRequestUserByIdQuery(Guid UserId) : IQuery;