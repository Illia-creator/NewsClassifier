using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsClassifier.TelegramBot.Constants
{
    public static class CommandResponses
    {
        public static readonly Dictionary<string, string> commandResponses = new Dictionary<string, string>
        {
            { "/info", "Привет, я бот!" },
            { "/commands", "Доступные команды:\n/info - информация о боте\n/commands - список доступных команд\n/login - войти\n/my_role - моя роль" },
            { "/login", "Вхід в систему, введіть свій пароль:" },
            { "/signup", "Реєстрація в системі, введіть новий пароль:" },
            { "/my_role", "Ваша роль: пользователь" },
            { "/newslist", "Надішліть кілька новин, після натисніть /newslistclssify" },
            { "/newslistclssify", "Почніть класифікувати новини" },

        };
    }
}
