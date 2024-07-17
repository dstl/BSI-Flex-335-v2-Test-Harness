// Crown-owned copyright, 2021-2024
using Sapient.Data;

namespace SapientServices.Communication
{
    public interface IConnection
    {
        event SapientMessageEventHandler MessageSent;

        event SapientMessageEventHandler MessageReceived;

        event SapientMessageEventHandler MessageError;

        event SapientMessageEventHandler SendErrorMessage;

        bool ValidationEnabled { get; set; }

        uint ConnectionID { get; }

        bool SendMessage(SapientMessage msg);

        void SetNoDelay(bool no_delay);
    }
}
