namespace SFA.DAS.EAS.Support.Web.Services;

public static class ChallengeHelper
{
    public static string GetChallengeMessage(List<int> challengeCharacterPositions)
    {
        return
            $"{DisplayCharacter(challengeCharacterPositions[0])} & {DisplayCharacter(challengeCharacterPositions[1])} character of a PAYE scheme (excluding the /):";
    }

    private static string DisplayCharacter(int challengeCharacter)
    {
        challengeCharacter++;

        int check;

        if (challengeCharacter < 20)
        {
            check = challengeCharacter;
        }
        else
        {
            var multiplier = challengeCharacter / 20;
            check = challengeCharacter - multiplier * 20;
        }

        switch (check)
        {
            case 1:
                return $"{challengeCharacter}st";
            case 2:
                return $"{challengeCharacter}nd";
            case 3:
                return $"{challengeCharacter}rd";
            default:
                return $"{challengeCharacter}th";
        }
    }
}