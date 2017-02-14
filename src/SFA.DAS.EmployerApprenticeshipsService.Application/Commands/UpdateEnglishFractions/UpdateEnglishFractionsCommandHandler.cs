using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Commands.UpdateEnglishFractions
{
    public class UpdateEnglishFractionsCommandHandler : AsyncRequestHandler<UpdateEnglishFractionsCommand>
    {
        private readonly IHmrcService _hmrcService;
        private readonly IEnglishFractionRepository _englishFractionRepository;
        private readonly ILogger _logger;

        public UpdateEnglishFractionsCommandHandler(IHmrcService hmrcService,
            IEnglishFractionRepository englishFractionRepository, 
            ILogger logger)
        {
            _hmrcService = hmrcService;
            _englishFractionRepository = englishFractionRepository;
            _logger = logger;
        }

        protected override async Task HandleCore(UpdateEnglishFractionsCommand message)
        {
            var existingFractions = (await _englishFractionRepository.GetAllEmployerFractions(message.EmployerReference)).ToList();

            if (existingFractions.Any() && !message.EnglishFractionUpdateResponse.UpdateRequired)
            {
                return;
            }

            DateTime? dateFrom = null;
            if (existingFractions?.OrderByDescending(x=>x.DateCalculated).FirstOrDefault()?.DateCalculated != null 
                && existingFractions?.OrderByDescending(x => x.DateCalculated).FirstOrDefault()?.DateCalculated != DateTime.MinValue)
            {
                dateFrom = existingFractions?.OrderByDescending(x => x.DateCalculated).FirstOrDefault()?.DateCalculated.AddDays(-1);
            }


            var fractionCalculations = await _hmrcService.GetEnglishFractions(message.EmployerReference, dateFrom);

            var hmrcFractions = fractionCalculations.FractionCalculations.SelectMany(calculations =>
            {
                var fractions = new List<DasEnglishFraction>();
                DateTime dateCalculated;

                if (!DateTime.TryParse(calculations.CalculatedAt, out dateCalculated))
                {
                    _logger.Error($"Could not convert HMRC API calculatedAt value {calculations.CalculatedAt} to a datetime for english fraction update for EmpRef {message.EmployerReference}");
                    return fractions;
                }

                foreach (var fraction in calculations.Fractions)
                {
                    decimal amount;

                    if (decimal.TryParse(fraction.Value, out amount))
                    {
                        fractions.Add(
                            new DasEnglishFraction
                            {
                                EmpRef = fractionCalculations.Empref,
                                DateCalculated = DateTime.Parse(calculations.CalculatedAt),
                                Amount = decimal.Parse(fraction.Value)
                            });
                    }
                    else
                    {
                        _logger.Error($"Could not convert HMRC API fraction value {fraction.Value} to a decimal for english fraction update for EmpRef {message.EmployerReference}");
                    }
                }
             
                return fractions;
            }).ToList();

            var newFraction = hmrcFractions.Except(existingFractions, new DasEmployerComparer()).ToList();

            foreach (var englishFraction in newFraction)
            {
                await _englishFractionRepository.CreateEmployerFraction(englishFraction, englishFraction.EmpRef);
            }
            
        }
    }

    public class DasEmployerComparer : IEqualityComparer<DasEnglishFraction>
    {
        public bool Equals(DasEnglishFraction source, DasEnglishFraction target)
        {
            return source.EmpRef.Equals(target.EmpRef) &&
                   source.DateCalculated.Equals(target.DateCalculated);
        }

        public int GetHashCode(DasEnglishFraction obj)
        {
            return obj.DateCalculated.GetHashCode() ^ obj.EmpRef.GetHashCode();
        }
    }
}
