using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Account.Query
{
    public class GetUserBalanceByIdQuery : IRequest<ApiResponse<GetUserBalanceResponse>>
    {
        public int? UserId { get; set; }
    }
    public class GetUserBalanceResponse
    {
        public double? Balance { get; set; }
    }
    public class GetUserBalanceByIdQueryHandler : IRequestHandler<GetUserBalanceByIdQuery, ApiResponse<GetUserBalanceResponse>>
    {
        private readonly IApplicationDbContext _context;
        public GetUserBalanceByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<ApiResponse<GetUserBalanceResponse>> Handle(GetUserBalanceByIdQuery request, CancellationToken cancellationToken)
        {
            var respsone = await _context.UserAccounts.Where(x => x.UserId == request.UserId).FirstOrDefaultAsync();
            var userBalance = new GetUserBalanceResponse()
            {
                Balance = respsone.Balance,
            };
            return new ApiResponse<GetUserBalanceResponse>(userBalance);
        }
    }
}
