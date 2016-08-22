using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLevyDeclaration
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
                Data = new LevyDeclarationSourceData
                {
                    AccountId = message.AccountId,
                    Data = declarations.Select(item => new LevyDeclarationSourceDataItem
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

        private List<LevyDeclarationSourceDataItem> MapFrom(IEnumerable<LevyDeclarationView> source)
        {
            return source.Select(item => new LevyDeclarationSourceDataItem
            {
                Id = item.Id,
                EmpRef = item.EmpRef,
                ActivityDate = item.SubmissionDate,
                Amount = item.Amount,
                LevyItemType = GetLevyItemType(item.SubmissionType)
            }).ToList();
        }

        private static LevyItemType GetLevyItemType(string input)
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