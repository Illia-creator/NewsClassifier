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
            { "/info", "Привіт, я бот для класифікації новин" },
            { "/commands", "Доступні команди:\n/info - інформація про бот\n/commands - список доступних команд\n/login - вхід" },
            { "/login", "Вхід в систему, введіть свій пароль:" },
            { "/signup", "Реєстрація в системі, введіть новий пароль:" },
            { "/newslist", "Надішліть кілька повідомлень, після натисніть /newslistclssify" },
            { "/newslistclssify", "Почніть класифікувати повідомлення" },
            { "/statistics", "Загальна кількість новин: 12\n \n Війна (5) - 41.7%\nТактична обстановка (3) - 25.0%\nСпорт (1) - 8.3%\nЕкономіка (3) - 25.0%" }

    };
    }
}
