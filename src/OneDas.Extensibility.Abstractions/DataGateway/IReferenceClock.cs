using System;

namespace OneDas.Extensibility
{
    public interface IReferenceClock
    {
        DateTime GetUtcDateTime();
        long GetTimerDrift();
    }
}
