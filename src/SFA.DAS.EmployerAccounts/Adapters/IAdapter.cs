namespace SFA.DAS.EmployerAccounts.Adapters
{
    public interface IAdapter<T1, T2>
    {
        T2 Convert(T1 input);
    }
}
