﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    public class WhenISubmitTheStartTransferConnectionPage
    {
        private const string SenderHashedAccountId = "ABC123";
        private const string ReceiverHashedAccountId = "XYZ987";

        private TransferConnectionInvitationsController _controller;
        private StartTransferConnectionInvitationViewModel _viewModel;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();
        private readonly Mock<IMapper> _mapper = new Mock<IMapper>();

        [SetUp]
        public void Arrange()
        {
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetTransferConnectionInvitationAccountQuery>()))
                .ReturnsAsync(new GetTransferConnectionInvitationAccountResponse
                {
                    ValidationResult = new ValidationResult
                    {
                        ValidationDictionary = new Dictionary<string, string>()
                    }
                });

            _controller = new TransferConnectionInvitationsController(_mapper.Object, _mediator.Object);

            _viewModel = new StartTransferConnectionInvitationViewModel
            {
                Message = new GetTransferConnectionInvitationAccountQuery
                {
                    SenderAccountHashedId = SenderHashedAccountId,
                    ReceiverAccountHashedId = ReceiverHashedAccountId
                }
            };
        }

        [Test]
        public async Task ThenAGetTransferConnectionInvitationAccountQueryShouldBeSent()
        {
            await _controller.Start(_viewModel);

            _mediator.Verify(m => m.SendAsync(_viewModel.Message), Times.Once);
        }

        [Test]
        public async Task ThenTheModelStateShouldBeValid()
        {
            await _controller.Start(_viewModel);

            Assert.That(_controller.ModelState.IsValid, Is.True);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheSendTransferConnectionPage()
        {
            var result = await _controller.Start(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Send"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("SenderAccountHashedId", out var senderHashedAccountId), Is.True);
            Assert.That(senderHashedAccountId, Is.EqualTo(SenderHashedAccountId));
            Assert.That(result.RouteValues.TryGetValue("ReceiverAccountHashedId", out var receiverHashedAccountId), Is.True);
            Assert.That(receiverHashedAccountId, Is.EqualTo(ReceiverHashedAccountId));
        }

        [Test]
        public async Task ThenTheModelStateShouldNotBeValidIfErrorsAreReturned()
        {
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetTransferConnectionInvitationAccountQuery>()))
                .ReturnsAsync(new GetTransferConnectionInvitationAccountResponse
                {
                    ValidationResult = new ValidationResult
                    {
                        ValidationDictionary = new Dictionary<string, string>
                        {
                            ["Foo"] = "Bar"
                        }
                    }
                });

            await _controller.Start(_viewModel);

            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheStartTransferConnectionPage()
        {
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetTransferConnectionInvitationAccountQuery>()))
                .ReturnsAsync(new GetTransferConnectionInvitationAccountResponse
                {
                    ValidationResult = new ValidationResult
                    {
                        ValidationDictionary = new Dictionary<string, string>
                        {
                            ["Foo"] = "Bar"
                        }
                    }
                });

            var result = await _controller.Start(_viewModel) as RedirectToRouteResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues.TryGetValue("action", out var actionName), Is.True);
            Assert.That(actionName, Is.EqualTo("Start"));
            Assert.That(result.RouteValues.ContainsKey("controller"), Is.False);
            Assert.That(result.RouteValues.TryGetValue("hashedAccountId", out var hashedAccountId), Is.True);
            Assert.That(hashedAccountId, Is.EqualTo(SenderHashedAccountId));
        }
    }
}