using Application.Interfaces;
using MediatR;
using Domain;

namespace Application.Features.User.Command
{
    public class CreateUserAccountCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public double Balance { get; set; }
        public int CreatedBy { get; set; }

        internal class CreateUserAccountCommandHandler : IRequestHandler<CreateUserAccountCommand, int>
        {
            private readonly IApplicationDbContext _context;
            public CreateUserAccountCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(CreateUserAccountCommand request, CancellationToken cancellationToken)
            {
                var account = new Domain.Entities.UserAccount()
                {
                    UserId = request.UserId,
                    Balance = request.Balance,
                    CreatedBy=request.CreatedBy
                };

                await _context.UserAccounts.AddAsync(account);
                await _context.SavechangesAsync();
                return account.Id;
            }
        }
    }
}
