using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Events.ProcessDeclaration;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommandHandler : AsyncRequestHandler<RefreshEmployerLevyDataCommand>
    {
        
        private readonly IValidator<RefreshEmployerLevyDataCommand> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IMediator _mediator;
        private readonly IHmrcDateService _hmrcDateService;

        public RefreshEmployerLevyDataCommandHandler(IValidator<RefreshEmployerLevyDataCommand> validator, IDasLevyRepository dasLevyRepository, IMediator mediator, IHmrcDateService hmrcDateService)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _mediator = mediator;
            _hmrcDateService = hmrcDateService;
        }

        protected override async Task HandleCore(RefreshEmployerLevyDataCommand message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            bool sendLevyDataChanged = false;
            foreach (var employerLevyData in message.EmployerLevyData)
            {
                foreach (var dasDeclaration in employerLevyData.Declarations.Declarations.OrderBy(c=>c.SubmissionDate))
                {
                    var declaration = await _dasLevyRepository.GetEmployerDeclaration(dasDeclaration.Id, employerLevyData.EmpRef);

                    if (declaration == null)
                    {
                        if (dasDeclaration.NoPaymentForPeriod)
                        {
                            var previousSubmission = await _dasLevyRepository.GetLastSubmissionForScheme(employerLevyData.EmpRef);
                            dasDeclaration.LevyDueYtd =  previousSubmission?.LevyDueYtd ?? 0;
                            dasDeclaration.LevyAllowanceForFullYear = previousSubmission?.LevyAllowanceForFullYear ?? 0;
                        }

                        if (dasDeclaration.PayrollMonth.HasValue && _hmrcDateService.IsSubmissionEndOfYearAdjustment(dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value, dasDeclaration.SubmissionDate))
                        {
                            var adjustmentDeclaration = await _dasLevyRepository.GetSubmissionByEmprefPayrollYearAndMonth(employerLevyData.EmpRef, dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value);
                            dasDeclaration.EndOfYearAdjustment = true;
                            dasDeclaration.EndOfYearAdjustmentAmount = adjustmentDeclaration?.LevyDueYtd - dasDeclaration.LevyDueYtd ?? 0;
                        }

                        if (dasDeclaration.PayrollMonth.HasValue && _hmrcDateService.IsSubmissionForFuturePeriod(dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value, DateTime.UtcNow))
                        {
                            continue;
                        }

                        await _dasLevyRepository.CreateEmployerDeclaration(dasDeclaration, employerLevyData.EmpRef, message.AccountId);
                        sendLevyDataChanged = true;
                    }
                }
            }

            if (sendLevyDataChanged)
            {
                await _mediator.PublishAsync(new ProcessDeclarationsEvent());
            }
            
        }
    }
}
