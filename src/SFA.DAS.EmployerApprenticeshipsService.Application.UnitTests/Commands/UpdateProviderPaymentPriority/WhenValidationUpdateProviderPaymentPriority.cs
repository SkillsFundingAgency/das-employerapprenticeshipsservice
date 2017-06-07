using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpdateProviderPaymentPriority;
using System.Collections.Generic;

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
        public void ShouldNotValidateProviderIdIsNotUnique()
        {
            var command = new UpdateProviderPaymentPriorityCommand
            {
                ProviderPriorityOrder = new List<long>
                                             {
                                                 111,
                                                 222,
                                                 111
                                             }
            };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsValue("Provider payment priority cannot contains duplication of provider id").Should().BeTrue();
        }

        [Test]
        public void ShouldNotValidateWhenInputListIsLessThan2Items()
        {
            var command = new UpdateProviderPaymentPriorityCommand
            {
                ProviderPriorityOrder = new List<long> { 111 }
            };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsValue("Need more than 1 item to set provider payment priority").Should().BeTrue();
        }

        [Test]
        public void ShouldNotValidateWhenInputListIsEmpty()
        {
            var command = new UpdateProviderPaymentPriorityCommand
            {
                ProviderPriorityOrder = new List<long>()
            };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsValue("Need more than 1 item to set provider payment priority").Should().BeTrue();
        }

        [Test]
        public void ShouldNotValidateWhenInputListIsNull()
        {
            var command = new UpdateProviderPaymentPriorityCommand
            {
                ProviderPriorityOrder = null
            };
            var validationResult = _sut.Validate(command);

            validationResult.IsValid().Should().BeFalse();
            validationResult.ValidationDictionary.ContainsValue("Need more than 1 item to set provider payment priority").Should().BeTrue();
        }
    }
}