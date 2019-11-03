using System;

namespace PubSub.Publisher.Contracts
{
    public class DateTimeNotification
    {
        public DateTime DateTime { get; }

        public DateTimeNotification(DateTime dateTime)
        {
            DateTime = dateTime;
        }
    }
}