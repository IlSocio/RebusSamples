using System;

namespace PubSub.Publisher.Contracts
{
    public class TimeSpanNotification
    {
        public TimeSpan TimeSpan { get; }

        public TimeSpanNotification(TimeSpan timeSpan)
        {
            TimeSpan = timeSpan;
        }
    }
}