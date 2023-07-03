using System.Linq;
using AutoFixture;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SFA.DAS.EmployerAccounts.TestCommon.AutoFixture
{
    public class DomainAutoDataAttribute : AutoDataAttribute
    {
        public DomainAutoDataAttribute()
           : base(() =>
           {
               var fixture = new Fixture();

               fixture.Behaviors.Add(new OmitOnRecursionBehavior());

               fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));

               fixture
                   .Customize(new DomainCustomizations())
                   .Customize<BindingInfo>(c => c.OmitAutoProperties());

               return fixture;
           })
        { }
    }
}
