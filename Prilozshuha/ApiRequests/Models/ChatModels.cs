namespace Prilozshuha.ApiRequests.Models
{
    public class ChatMessagesResponse
    {
        public bool status { get; set; }
        public List<MessageModel>? messages { get; set; }
    }

    public class MessageModel
    {
        public int messageId { get; set; }
        public int? movieId { get; set; }
        public int fromUserId { get; set; }
        public string fromUserName { get; set; } = string.Empty;
        public int? toUserId { get; set; }
        public string content { get; set; } = string.Empty;
        public string? photoUrl { get; set; }
        public DateTime createdAt { get; set; }
        public bool isEdited { get; set; }
        public bool isOwn { get; set; }
    }
}