using Microsoft.AspNetCore.SignalR.Client;
using Prilozshuha.ApiRequests.Models;
using System.Text.Json;

namespace Prilozshuha.ApiRequests
{
    public class ChatService
    {
        private HubConnection? _hubConnection;
        private readonly HttpClient _httpClient;
        private int? _currentUserId;

        public List<MessageModel> Messages { get; private set; } = new List<MessageModel>();
        public HashSet<int> OnlineUsers { get; private set; } = new HashSet<int>();
        public Action? OnMessagesUpdated { get; set; }
        public Action? OnOnlineUsersUpdated { get; set; }
        public Action<int>? OnUserOnline { get; set; }
        public Action<int>? OnUserOffline { get; set; }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        private HashSet<int> _receivedMessageIds = new HashSet<int>();

        public ChatService(HttpClient httpClient) => _httpClient = httpClient;

        public void SetCurrentUserId(int userId) => _currentUserId = userId;

        public async Task Connect()
        {
            if (_currentUserId == null) return;

            var baseAddress = _httpClient.BaseAddress?.AbsoluteUri.TrimEnd('/');
            var hubUrl = $"{baseAddress}/chat?userId={_currentUserId}";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<MessageModel>("ReceiveMessage", msg =>
            {
                if (_receivedMessageIds.Contains(msg.messageId))
                {
                    Console.WriteLine($"Duplicate message {msg.messageId} ignored");
                    return;
                }

                _receivedMessageIds.Add(msg.messageId);
                msg.isOwn = msg.fromUserId == _currentUserId;

                if (!Messages.Any(m => m.messageId == msg.messageId))
                {
                    Messages.Add(msg);
                    OnMessagesUpdated?.Invoke();
                    Console.WriteLine($"Message {msg.messageId} added");
                }
            });

            _hubConnection.On<MessageModel>("MessageEdited", msg =>
            {
                var index = Messages.FindIndex(m => m.messageId == msg.messageId);
                if (index != -1)
                {
                    Messages[index].content = msg.content;
                    Messages[index].isEdited = true;
                    OnMessagesUpdated?.Invoke();
                }
            });

            _hubConnection.On<int>("MessageDeleted", messageId =>
            {
                Messages.RemoveAll(m => m.messageId == messageId);
                OnMessagesUpdated?.Invoke();
            });

            _hubConnection.On<int>("UserOnline", userId =>
            {
                if (!OnlineUsers.Contains(userId))
                {
                    OnlineUsers.Add(userId);
                    OnOnlineUsersUpdated?.Invoke();
                    OnUserOnline?.Invoke(userId);
                    Console.WriteLine($"User {userId} came online");
                }
            });

            _hubConnection.On<int>("UserOffline", userId =>
            {
                if (OnlineUsers.Contains(userId))
                {
                    OnlineUsers.Remove(userId);
                    OnOnlineUsersUpdated?.Invoke();
                    OnUserOffline?.Invoke(userId);
                    Console.WriteLine($"User {userId} went offline");
                }
            });

            _hubConnection.On<List<int>>("ReceiveOnlineUsers", (users) =>
            {
                OnlineUsers.Clear();
                foreach (var u in users)
                {
                    OnlineUsers.Add(u);
                }
                OnOnlineUsersUpdated?.Invoke();
                Console.WriteLine($"Received online users: {string.Join(", ", users)}");
            });

            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine($"SignalR connected! UserId={_currentUserId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR error: {ex.Message}");
            }
        }

        public async Task Disconnect()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }

        public async Task JoinMovieChat(int movieId)
        {
            if (IsConnected)
                await _hubConnection!.InvokeAsync("JoinMovieChat", movieId);
        }

        public async Task JoinPrivateChat(int withUserId)
        {
            if (IsConnected)
                await _hubConnection!.InvokeAsync("JoinPrivateChat", withUserId);
        }

        public async Task SendMessage(int? movieId, int? toUserId, string content, string? photoUrl = null)
        {
            if (IsConnected && !string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    Console.WriteLine($"Sending message: movieId={movieId}, toUserId={toUserId}, content={content}");
                    await _hubConnection!.InvokeAsync("SendMessage", movieId, toUserId, content, photoUrl);
                    Console.WriteLine("Message sent successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Cannot send: IsConnected={IsConnected}, content={content}");
            }
        }

        public async Task EditMessage(int messageId, string newContent)
        {
            if (IsConnected && !string.IsNullOrWhiteSpace(newContent))
                await _hubConnection!.InvokeAsync("EditMessage", messageId, newContent);
        }

        public async Task DeleteMessage(int messageId)
        {
            if (IsConnected)
                await _hubConnection!.InvokeAsync("DeleteMessage", messageId);
        }

        public async Task LoadMovieMessages(int movieId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Chat/movie/{movieId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ChatMessagesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result?.status == true && result.messages != null)
                    {
                        Messages.Clear();
                        _receivedMessageIds.Clear();

                        foreach (var m in result.messages)
                        {
                            m.isOwn = m.fromUserId == _currentUserId;
                            Messages.Add(m);
                            _receivedMessageIds.Add(m.messageId);
                        }
                        OnMessagesUpdated?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки чата фильма: {ex.Message}");
            }
        }

        public async Task LoadPrivateMessages(int withUserId)
        {
            try
            {
                if (_currentUserId == null) return;

                _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
                _httpClient.DefaultRequestHeaders.Add("X-User-Id", _currentUserId.Value.ToString());

                var response = await _httpClient.GetAsync($"api/Chat/private/{withUserId}");
                _httpClient.DefaultRequestHeaders.Remove("X-User-Id");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ChatMessagesResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result?.status == true && result.messages != null)
                    {
                        Messages.Clear();
                        _receivedMessageIds.Clear();

                        foreach (var m in result.messages)
                        {
                            m.isOwn = m.fromUserId == _currentUserId;
                            Messages.Add(m);
                            _receivedMessageIds.Add(m.messageId);
                        }
                        OnMessagesUpdated?.Invoke();
                        Console.WriteLine($"Loaded {Messages.Count} messages");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки личных сообщений: {ex.Message}");
            }
        }

        public void ClearMessages()
        {
            Messages.Clear();
            _receivedMessageIds.Clear();
            OnMessagesUpdated?.Invoke();
        }

        public bool IsUserOnline(int userId) => OnlineUsers.Contains(userId);
    }
}