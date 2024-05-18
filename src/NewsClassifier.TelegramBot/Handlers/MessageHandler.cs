using NewsClassifier.Classifier.Servicess;
using NewsClassifier.TelegramBot.Constants;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace NewsClassifier.TelegramBot.Handlers
{
    internal class MessageHandler
    {
        private readonly string[] tags = new string[] { "/info", "/commands", "/login", "/my_role", "/signup" };
        private readonly LoginService _loginService = new LoginService();
        private readonly TelegramBotClient _client;
        private readonly Configuration _configuration;
        private readonly ClassifyService _classifyService = new ClassifyService();
        private static readonly ConcurrentDictionary<long, string> newsTexts = new ConcurrentDictionary<long, string>();
        private List<string> messages = new List<string>();

        public MessageHandler(TelegramBotClient client)
        {
            _client = client;
        }

        public async Task SelectCommand(Message message)
        {
            string resultMessage;

                switch (message.Text)
                {
                    case "/info":
                        await SendMessageToUser(message.Chat.Id, CommandResponses.commandResponses["/info"]);
                        break;

                    case "/commands":
                        await SendMessageToUser(message.Chat.Id, CommandResponses.commandResponses["/commands"]);
                    break;

                    case "/login":
                        await SendMessageToUser(message.Chat.Id, CommandResponses.commandResponses["/login"]);
                    if (StatusService.isInOperation(message.Chat.Id))
                            StatusService.cahngeStatus(message.Chat.Id, "login");
                    else
                            StatusService.addKey(message.Chat.Id, "login");
                    break;

                    case "/signup":
                        await SendMessageToUser(message.Chat.Id, CommandResponses.commandResponses["/signup"]);
                    if (StatusService.isInOperation(message.Chat.Id))
                        StatusService.cahngeStatus(message.Chat.Id, "signup");
                    else
                        StatusService.addKey(message.Chat.Id, "signup"); ;
                    break;

                    case "/newslist":
                        await SendMessageToUser(message.Chat.Id, CommandResponses.commandResponses["/newslist"]);
                        StatusService.cahngeStatus(message.Chat.Id, "fillinglist");
                    break;

                    case "/newslistclssify":
                        await SendMessageToUser(message.Chat.Id, CommandResponses.commandResponses["/newslistclssify"]);
                        StatusService.cahngeStatus(message.Chat.Id, "classifyinglist");
                        await SelectClassForList(message);
                    break;

                default:
                        if (StatusService.isInOperation(message.Chat.Id))
                        {
                            switch (StatusService.getStatusOfUser(message.Chat.Id))
                            {
                                case "login":
                                    await LoginUser(message);
                                break;

                                case "signup":
                                    await SignupUser(message);
                                break;

                                case "awaiting_class_selection":
                                    await HandleClassSelection(message);
                                break;

                                case "fillinglist":
                                    await FillListOfNews(message);
                                break;

                                case ("classifyinglist"):
                                    await HandleClassSelectionList(message);
                                break;


                            default:
                                await AddDataToDataset(message);
                                break;
                            }

                        }
                        else
                            await _client.SendTextMessageAsync(message.Chat.Id, "заглушка на класификацию");

                    break;
                }
        }

        private async Task LoginUser(Message message)
        {
            if (_loginService.UserExists(message.Chat.Id))
            {
                if (_loginService.AuthenticateUser(message.Chat.Id, message.Text))
                {
                    await _client.SendTextMessageAsync(message.Chat.Id, "Ви авторизовані, як редактор");
                    StatusService.cahngeStatus(message.Chat.Id, "redactor");
                }

                else
                {
                    StatusService.removeKey(message.Chat.Id);
                    await _client.SendTextMessageAsync(message.Chat.Id, "Ви не авторизовані, як редактор");
                }

            }
            else
            {
                StatusService.removeKey(message.Chat.Id);
                await _client.SendTextMessageAsync(message.Chat.Id, "Вас не авторизовано в системі");
            }
        }

        private async Task SignupUser(Message message)
        {
            if (!_loginService.UserExists(message.Chat.Id))
            {
                _loginService.AddUser(message.Chat.Id, message.Text);
                StatusService.cahngeStatus(message.Chat.Id, "redactor");
                await _client.SendTextMessageAsync(message.Chat.Id, "Вас зареэстровано в системі, як редактора!");
            }
            else
            {
                StatusService.removeKey(message.Chat.Id);
                await _client.SendTextMessageAsync(message.Chat.Id, "Ви вже зареєстровані як редактор! \n Використайте команду /login щоб авторизуватися");
            }
        }

        public async Task SendMessageToUser(long chatId, string message)
        {
            await _client.SendTextMessageAsync(chatId, message);
        }

        public async Task FillListOfNews(Message message)
        {
            StatusService.cahngeStatus(message.Chat.Id, "fillinglist");

            if (message.Caption != null)
                messages.Add(message.Caption);
            else
                messages.Add(message.Text);
        }

       

        public async Task CheckUser(long chatId)
        {
            StatusService.removeKey(chatId);
            if (_loginService.UserExists(chatId))
            {
                await _client.SendTextMessageAsync(chatId, "Ви авторизовані як адмінестратор!");
            }
            else
            {
                await _client.SendTextMessageAsync(chatId, "Пароль не вірний!");
            }
        }

        public async Task AddDataToDataset(Message message)
        {
            StatusService.cahngeStatus(message.Chat.Id, "awaiting_class_selection");
            if (message.Caption != null)
                newsTexts.TryAdd(message.Chat.Id, message.Caption);
            else
                newsTexts.TryAdd(message.Chat.Id, message.Text);

            ReplyKeyboardMarkup keyboard = new(new[]
            {
                new KeyboardButton[] {"Війна", "Політика" },
                new KeyboardButton[] {"Економіка", "Тактична інформація" },
                new KeyboardButton[] {"Наука", "Здоров'я" },
                new KeyboardButton[] {"Економіка", "Спорт" },
                new KeyboardButton[] {"Культура", "Інцеденти" },
            })
            {
                ResizeKeyboard = true
            };
        }

        public async Task RedirectToSelection(Message message)
        {
            await SelectClassForList(message);
        }

        public async Task SelectClassForList(Message message)
        {
            string currentNews = messages.FirstOrDefault();

            if (string.IsNullOrEmpty(currentNews))
            {
                var removeKeyboard = new ReplyKeyboardRemove();
                await _client.SendTextMessageAsync(message.Chat.Id, "новини для класифыкації закінчились. Для додавання нових натисніть /newslist", replyMarkup: removeKeyboard);
                return;
            }

            StatusService.cahngeStatus(message.Chat.Id, "classifyinglist");
            ReplyKeyboardMarkup keyboard = new(new[]
            {
                new KeyboardButton[] {"Війна", "Політика" },
                new KeyboardButton[] {"Економіка", "Тактична інформація" },
                new KeyboardButton[] {"Наука", "Здоров'я" },
                new KeyboardButton[] {"Економіка", "Спорт" },
                new KeyboardButton[] {"Культура", "Інцеденти" },
            })
            {
                ResizeKeyboard = true
            };

            await _client.SendTextMessageAsync(message.Chat.Id, $"Оберіть клас, до якого відноситься новина \n \n {currentNews}", replyMarkup: keyboard);
        }

        public async Task HandleClassSelection(Message message)
        {
            string originalMessageText = newsTexts[message.Chat.Id];
            if (string.IsNullOrEmpty(originalMessageText))
            {
                StatusService.cahngeStatus(message.Chat.Id, "redactor");
                throw new Exception("Новини для класифікації закінчились, додайте нові");
            }
            string selectedClass = message.Text;

            // Here you can call the method that processes the dataset addition
            await _classifyService.AddDataToDataset(selectedClass, originalMessageText);

            var removeKeyboard = new ReplyKeyboardRemove();

            await _client.SendTextMessageAsync(message.Chat.Id, $"Новина додана до класу: {selectedClass}\nТекст новини: {originalMessageText}", replyMarkup: removeKeyboard);

            newsTexts.TryRemove(message.Chat.Id, out _);
        }

        public async Task HandleClassSelectionList(Message message)
        {
            string originalMessageText = messages.FirstOrDefault();
            string selectedClass = message.Text;

            // Here you can call the method that processes the dataset addition
            await _classifyService.AddDataToDataset(selectedClass, originalMessageText);

            var removeKeyboard = new ReplyKeyboardRemove();

            await _client.SendTextMessageAsync(message.Chat.Id, $"Новина додана до класу: {selectedClass}\nТекст новини: {originalMessageText}", replyMarkup: removeKeyboard);

            messages.Remove(originalMessageText);
            StatusService.cahngeStatus(message.Chat.Id, "classifyinglist");
            await RedirectToSelection(message);
        }
    }
}
