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
            { "userInstruction", "Ви авторизовані як користувач. \nВи можете отримати клас однієї новини. Для цього: \n1) Введіть тег /classifyOneNews \n2) Надішліть одну новину \n\nВи можете отримати статистику відносно декількох новин. Для цього: \n1) Введіть тег /classifySeveralNews \n2) Надішліть декілька новин, щоб отримати до них статистику \n3) Введіть тег /getStatistics" }
    };
    }
}
