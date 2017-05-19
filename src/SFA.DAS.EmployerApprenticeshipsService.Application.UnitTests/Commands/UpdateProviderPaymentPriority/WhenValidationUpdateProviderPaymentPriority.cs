using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using NUnit.Framework;

using SFA.DAS.EAS.Application.Commands.UpdateProviderPaymentPriority;
using SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UpdateProviderPaymentPriority
{
    [TestFixture]
    public class WhenValidationUpdateProviderPaymentPriority
    {
        private UpdateProviderPaymentPriorityValidator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new UpdateProviderPaymentPriorityValidator();
        }

        [Test]
        public void ShouldNotValidateWithNegativeNumbers()
        {
            var command = new UpdateProviderPaymentPriorityCommand
                              {
                                  Data = new List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI>
                                             {
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 222, PaymentPriority = 2, ProviderName = "p2"},
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 111, PaymentPriority = -1, ProviderName = "p1"},
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 333, PaymentPriority = 3, ProviderName = "p3"}
                                             }
                              };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.LessThan1").Should().BeTrue();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.GreaterThan").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.Duplication").Should().BeFalse();
            validationResult.ValidationDictionary.FirstOrDefault(
                m => m.Value.Contains("cannot be less than 1"))
                .Value.Should().NotBeNull();
        }

        [Test]
        public void ShouldNotValidateWithAPriorityGreaterThanAmountOfItems()
        {
            var command = new UpdateProviderPaymentPriorityCommand
            {
                Data = new List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI>
                                             {
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 111, PaymentPriority = 1, ProviderName = "p1"},
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 222, PaymentPriority = 2, ProviderName = "p2"},
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 333, PaymentPriority = 4, ProviderName = "p3"}
                                             }
            };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.LessThan1").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.GreaterThan").Should().BeTrue();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.Duplication").Should().BeFalse();
            validationResult.ValidationDictionary.FirstOrDefault(
                m => m.Value.Contains("cannot have a priority order hight than the number of providers"))
                .Value.Should().NotBeNull();
        }

        [Test]
        public void ShouldNotValidateWhiteNotAllPaymentOrdersUnique()
        {
            var command = new UpdateProviderPaymentPriorityCommand
            {
                Data = new List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI>
                                             {
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 111, PaymentPriority = 1, ProviderName = "p1"},
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 222, PaymentPriority = 1, ProviderName = "p2"},
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 333, PaymentPriority = 3, ProviderName = "p3"}
                                             }
            };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.LessThan1").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.GreaterThan").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.Duplication").Should().BeTrue();
            validationResult.ValidationDictionary.FirstOrDefault(
                m => m.Value.Contains("cannot contains duplication of payment priority. 1,1,3"))
                .Value.Should().NotBeNull();
        }

        [Test]
        public void ShouldNotValidateWhenInputListIsLessThan2Items()
        {
            var command = new UpdateProviderPaymentPriorityCommand
            {
                Data = new List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI>
                                             {
                                                 new GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI
                                                    { ProviderId = 111, PaymentPriority = 1, ProviderName = "p1"}
                                             }
            };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.LessThan1").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.GreaterThan").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.Duplication").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.NeedMoreThanOneItemsInList").Should().BeTrue();
            validationResult.ValidationDictionary.FirstOrDefault(
                m => m.Value.Contains("Need more than 1 item"))
                .Value.Should().NotBeNull();
        }

        [Test]
        public void ShouldNotValidateWhenInputListIsEmpty()
        {
            var command = new UpdateProviderPaymentPriorityCommand
            {
                Data = new List<GetProviderPaymentPriorityHandler.ProviderPaymentPriorityItemAPI>()
            };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.LessThan1").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.GreaterThan").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.Duplication").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.NeedMoreThanOneItemsInList").Should().BeTrue();
            validationResult.ValidationDictionary.FirstOrDefault(
                m => m.Value.Contains("Need more than 1 item"))
                .Value.Should().NotBeNull();
        }

        [Test]
        public void ShouldNotValidateWhenInputListIsNull()
        {
            var command = new UpdateProviderPaymentPriorityCommand
            {
                Data = null
            };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.LessThan1").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.GreaterThan").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.Duplication").Should().BeFalse();
            validationResult.ValidationDictionary.ContainsKey("PaymentPriority.NeedMoreThanOneItemsInList").Should().BeTrue();
            validationResult.ValidationDictionary.FirstOrDefault(
                m => m.Value.Contains("Need more than 1 item"))
                .Value.Should().NotBeNull();
        }
    }
}