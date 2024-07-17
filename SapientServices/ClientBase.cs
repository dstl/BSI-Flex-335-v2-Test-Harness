// Crown-owned copyright, 2021-2024
namespace SapientServices
{
    using FluentValidation.Results;
    using Google.Protobuf;
    using log4net;
    using Sapient.Data;
    using SapientServices.Communication;
    using SapientServices.Data.Validation;
    using System;
    using Google.Protobuf.WellKnownTypes;

    public class ClientBase
    {
        public event SapientMessageEventHandler MessageSent;

        public event SapientMessageEventHandler MessageReceived;

        public event SapientMessageEventHandler MessageError;

        public event SapientMessageEventHandler SendErrorMessage;

        public bool ValidationEnabled { get; set; } = true;

        protected void OnDataReceived(SapientMessageEventArgs e)
        {
            SapientMessageEventHandler handler = this.MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnDataSend(SapientMessageEventArgs e)
        {
            SapientMessageEventHandler handler = this.MessageSent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnDataError(SapientMessageEventArgs e)
        {
            SapientMessageEventHandler handler = this.MessageError;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected bool Validate(SapientMessage message, bool isSendValidation, ILog logger)
        {
            bool result = false;
            if (message != null)
            {
                if (!this.ValidationEnabled)
                {
                    logger.InfoFormat("{0} Message will not be validated. Validation is turned off.", message?.ContentCase);
                }
                FluentValidation.Results.ValidationResult validationResult = new SapientMainMessageValidator().Validate(message);
                if (!validationResult.IsValid)
                {
                    if (this.ValidationEnabled)
                    {
                        List<string> errors = new List<string>();   
                        string error = isSendValidation ? "Message was not sent." : "Received message was not processed.";
                        error = error + Environment.NewLine + message.ToString();
                        foreach (ValidationFailure validationError in validationResult.Errors)
                        {
                            error = error + Environment.NewLine + validationError.ErrorMessage;
                            errors.Add(validationError.ErrorMessage);  
                        }
                        this.OnDataError(new SapientMessageEventArgs() { Error = error, Message = message });
                        logger.ErrorFormat("Error: {0}", error);
                        if (!isSendValidation) 
                        {
                            this.PublishErrorMessage(message, errors);
                        }
                    }
                    else
                    {
                        logger.Error("Message is invalid but validation ignored.");
                        logger.ErrorFormat("Error: {0}", validationResult.Errors.ToString());
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        public void PublishErrorMessage(SapientMessage badMessage, List<string> errorMessages)
        {
            if (badMessage != null)
            {
                SapientMessageEventHandler handler = this.SendErrorMessage;
                if (handler != null)
                {
                    SapientMessageEventArgs e = new SapientMessageEventArgs()
                    {
                        Message = new SapientMessage()
                        {
                            DestinationId = badMessage.NodeId,
                            Timestamp = Timestamp.FromDateTime(DateTime.Now.ToUniversalTime()),
                            Error = new Error()
                            {
                                Packet = Google.Protobuf.ByteString.CopyFrom(badMessage.ToByteArray())
                            }
                        }
                    };
                    foreach (string error in errorMessages)
                    {
                        e.Message.Error.ErrorMessage.Add(error);
                    }
                    handler(this, e);
                }
            }
        }
    }
}