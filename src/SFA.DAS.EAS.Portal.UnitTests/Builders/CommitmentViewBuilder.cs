using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.EAS.Portal.UnitTests.Builders
{
    public class CommitmentViewBuilder
    {
        public CommitmentViewBuilder()
        {           
        }

        public CommitmentView Build()
        {
            return new CommitmentView();
        }      

        public static implicit operator CommitmentView(CommitmentViewBuilder instance)
        {
            return instance.Build();
        }
    }
}
