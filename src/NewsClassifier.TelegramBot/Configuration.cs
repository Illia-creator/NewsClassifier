using System.Collections.Concurrent;
using System.Threading.Tasks;
using NewsClassifier.TelegramBot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NewsClassifier.TelegramBot
{
    public class Configuration
    {
        private readonly TelegramBotClient _client;
        private readonly MessageHandler _messageHandler;

        public Configuration()
        {
            // Инициализация клиента Telegram бота
            _client = new TelegramBotClient("7020177405:AAFoHbJGeH9-yx4tJJLsZcX1hwaQ_Ejhucc");
            _messageHandler = new MessageHandler(_client);
        }

        public async Task StartConnection()
        {
            _client.StartReceiving(HandleUpdate, HandleError);
        }

        public Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            try
            {
                if (message != null && ( message.Text != null || message.Caption != null))
                {
                    var result = _messageHandler.SelectCommand(message);
                }
                else
                {
                    if (message.Type.ToString() == "Video" || message.Type.ToString() == "Photo")
                        return;
                    else
                        throw new Exception("Введіть команду, або новину!");
                }
            }
            catch (Exception ex) 
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, ex.Message);
            }
         }
    }

    public static class StatusService
    {
        private static readonly ConcurrentDictionary<long, string> keyValuePairs = new ConcurrentDictionary<long, string>();

        public static void addKey(long chatId, string status)
        { 
            keyValuePairs.TryAdd(chatId, status);
        }

        public static void removeKey(long chatId)
        {
            keyValuePairs.TryRemove(chatId,out _);
        }

        public static bool isInOperation(long chatId)
        { 
            return keyValuePairs.ContainsKey(chatId);
        }

        public static string getStatusOfUser(long chatId)
        {
            return keyValuePairs[chatId];
        }

        public static void cahngeStatus(long chatId, string status)
        {
            keyValuePairs[chatId] = status;
        }

    }
}
