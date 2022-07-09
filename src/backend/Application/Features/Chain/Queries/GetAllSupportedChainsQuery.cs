using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Chain.Queries
{
    public class GetAllSupportedChainsQuery : IRequest<Result<List<ChainInfoDto>>>
    {
        public class GetAllSupportedChainsQueryHandler : IRequestHandler<GetAllSupportedChainsQuery, Result<List<ChainInfoDto>>>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public GetAllSupportedChainsQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public Task<Result<List<ChainInfoDto>>> Handle(GetAllSupportedChainsQuery request, CancellationToken cancellationToken)
            {
                var chains = _dbContext.ChainInfos.Where(x => x.IsEnabled).OrderBy(x => x.OrderIdx).ToList();
                var mappedChains = _mapper.Map<List<ChainInfoDto>>(chains);
                return Result<List<ChainInfoDto>>.SuccessAsync(mappedChains);
            }
        }
    }
}
