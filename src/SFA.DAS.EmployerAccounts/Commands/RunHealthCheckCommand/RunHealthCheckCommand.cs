using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand;

public class RunHealthCheckCommand : IRequest
{
    [IgnoreMap]
    [Required]
    public Guid? UserRef { get; set; }
}