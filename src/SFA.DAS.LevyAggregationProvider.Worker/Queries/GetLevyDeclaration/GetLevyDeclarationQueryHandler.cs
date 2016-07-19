using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.LevyAggregationProvider.Worker.Model;

namespace SFA.DAS.LevyAggregationProvider.Worker.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationQueryHandler : IAsyncRequestHandler<GetLevyDeclarationRequest, GetLevyDeclarationResponse>
    {
        private readonly IDasLevyRepository _repository;

        public GetLevyDeclarationQueryHandler(IDasLevyRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            _repository = repository;
        }

        public async Task<GetLevyDeclarationResponse> Handle(GetLevyDeclarationRequest message)
        {
            var declarations = await _repository.GetAccountLevyDeclarations(message.AccountId);

            return new GetLevyDeclarationResponse
            { 
                Data = new SourceData
                {
                    AccountId = message.AccountId,
                    Data = declarations.Select(item => new SourceDataItem
                    {
                        Id = item.Id,
                        EmpRef = item.EmpRef,
                        ActivityDate = item.SubmissionDate,
                        Amount = item.Amount,
                        LevyItemType = GetLevyItemType(item.SubmissionType)
                    }).ToList()
                }
            };
        }

        private List<SourceDataItem> MapFrom(List<LevyDeclarationView> source)
        {
            return source.Select(item => new SourceDataItem
            {
                Id = item.Id,
                EmpRef = item.EmpRef,
                ActivityDate = item.SubmissionDate,
                Amount = item.Amount,
                LevyItemType = GetLevyItemType(item.SubmissionType)
            }).ToList();
        }

        private LevyItemType GetLevyItemType(string input)
        {
            switch (input)
            {
                case "Levy":
                    return LevyItemType.Declaration;
                case "TopUp":
                    return LevyItemType.TopUp;
                default:
                    return LevyItemType.Unknown;
            }
        }
    }
}