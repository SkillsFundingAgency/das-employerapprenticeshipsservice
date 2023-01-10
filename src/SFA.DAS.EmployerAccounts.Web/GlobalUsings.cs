﻿global using System;
global using System.Net;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Mvc;

global using AutoMapper;
global using MediatR;
global using SFA.DAS.Authentication;
global using SFA.DAS.EmployerAccounts.Interfaces;
global using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
global using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;
global using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
global using SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;
global using SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation;
global using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
global using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
global using SFA.DAS.EmployerAccounts.Web.Helpers;
global using SFA.DAS.EmployerAccounts.Web.Orchestrators;
global using SFA.DAS.EmployerAccounts.Web.ViewModels;