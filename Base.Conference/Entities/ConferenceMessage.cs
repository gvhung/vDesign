using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Security;
using Newtonsoft.Json;

namespace Base.Conference.Entities
{
    public enum MessageContentType
    {
        Text = 0,
        File = 1
    }

    public class ConferenceMessage : BaseObject
    {
        public string TextMessage { get; set; }

        [ForeignKey("File")]
        public int? FileID { get; set; }
        public virtual FileData File { get; set; }

        [ForeignKey("From")]
        public int FromId { get; set; }

        [JsonIgnore]
        public virtual User From { get; set; }

        public DateTime Date { get; set; }

        public bool IsNew { get; set; }

        public MessageContentType MessageType { get; set; }
    }

    public class PrivateMessage : ConferenceMessage
    {
        public PrivateMessage() { }

        public PrivateMessage(ConferenceMessage message)
        {
            TextMessage = message.TextMessage;
            FromId = message.FromId;
            Date = message.Date;
            IsNew = message.IsNew;
            MessageType = message.MessageType;
            File = message.File;
            FileID = message.FileID;
        }

        public int? ToUserId { get; set; }

        [JsonIgnore]
        [ForeignKey("ToUserId")]
        public virtual User ToUser { get; set; }
        
    }

    public class PublicMessage : ConferenceMessage
    {
        public PublicMessage() { }

        public PublicMessage(ConferenceMessage message)
        {
            TextMessage = message.TextMessage;
            FromId = message.FromId;
            Date = message.Date;
            IsNew = message.IsNew;
            MessageType = message.MessageType;
            File = message.File;
            FileID = message.FileID;
        }

        public int? ToConferenceId { get; set; }

        [JsonIgnore]
        [InverseProperty("Messages")]
        [ForeignKey("ToConferenceId")]
        public virtual Conference ToConference { get; set; }
    }

    public class MessageResult
    {
        public ConferenceMessage Message { get; set; }
        public List<int> Targets { get; set; }
    }
}
