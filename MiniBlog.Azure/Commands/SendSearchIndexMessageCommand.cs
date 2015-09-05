using Microsoft.WindowsAzure.Storage.Queue;
using MiniBlog.Contracts.Framework;

namespace MiniBlog.Azure.Commands
{
    internal class SendSearchIndexMessageCommand : ICommand<CloudQueue>
    {
        private readonly string message;

        public SendSearchIndexMessageCommand(string message)
        {
            this.message = message;
        }

        public void Apply(CloudQueue model)
        {
            model.AddMessage(new CloudQueueMessage(message));
        }
    }
}