using System;

public static class CurrentTime
{
    public static long MilliSeconds => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
}
