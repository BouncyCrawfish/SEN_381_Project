namespace CivicConnect.SharedKernel;


public interface IOperationalLog
{
    void Error(string message, Exception exception);
}
