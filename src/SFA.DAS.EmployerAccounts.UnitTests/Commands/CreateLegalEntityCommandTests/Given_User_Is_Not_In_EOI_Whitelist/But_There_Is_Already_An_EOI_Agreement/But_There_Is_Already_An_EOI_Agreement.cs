using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests.Given_User_Is_Not_In_EOI_Whitelist.But_There_Is_Already_An_EOI_Agreement
{
    [ExcludeFromCodeCoverage]
    public class But_There_Is_Already_An_EOI_Agreement
    :Given_User_Is_Not_In_EIO_Whitelist
    {
        public But_There_Is_Already_An_EOI_Agreement()
        {
            EmployerAccountsRepository
                .Setup(
                    m => m.GetAccountById(
                        Owner.AccountId))
                .ReturnsAsync(
                    BuildAccountWithOneEOIAgreement
                );
        }

        private Account BuildAccountWithOneEOIAgreement()
        {
            return
                new Account
                {
                    AccountLegalEntities = new List<AccountLegalEntity>
                    {
                        new AccountLegalEntity
                        {
                            Agreements = new List<EmployerAgreement>
                            {
                                new EmployerAgreement
                                {
                                    Template = new AgreementTemplate
                                    {
                                        AgreementType = AgreementType.NoneLevyExpressionOfInterest
                                    }
                                }
                            }
                        }
                    }
                };
        }
    }
}