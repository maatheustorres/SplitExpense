using MediatR;
using Microsoft.EntityFrameworkCore;
using SplitExpense.Application.Core.Abstractions.Data;
using SplitExpense.Contracts.Users;
using SplitExpense.Domain.Core.Errors;
using SplitExpense.Domain.Core.Primitives;
using SplitExpense.Domain.Core.Primitives.Result;
using SplitExpense.Domain.Entities;

namespace SplitExpense.Application.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ResultT<UserResponse>>
{
    private readonly IDbContext _dbContext;

    public GetUserByIdQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResultT<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if(request.UserId == Guid.Empty)
        {
            return Result.Failure<UserResponse>(Error.NullValue);
        }

        UserResponse user = await _dbContext.Set<User>().AsNoTracking()
            .Where(x => x.Id == request.UserId)
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                FistName = user.FirstName,
                LastName = user.LastName
            }).SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(DomainErrors.User.NotFound);
        }

        return user;
    }
}
