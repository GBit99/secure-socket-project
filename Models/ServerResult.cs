using System;

namespace Models
{
    public class ServerResult
    {
        public ServerResult()
        { }

        public ServerResult(string message)
        {
            Status = StatusCode.SendingMessage;
            Message = message;
        }

        public ServerResult(Guid elementId, StatusCode status)
        {
            ElementId = elementId;
            Status = status;
        }

        public Guid? ElementId { get; set; }

        public StatusCode Status { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            if (Status == StatusCode.SendingMessage)
            {
                return Message;
            }
            else
            {
                return $"ElementId: {ElementId.Value};\nStatus: {Status}";
            }
        }
    }
}
