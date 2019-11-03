namespace PubSub.Publisher.Contracts
{
    public class StringNotification
    {
        public string Text { get; }

        public StringNotification(string text)
        {
            Text = text;
        }
    }
}
