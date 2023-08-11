using System.Linq;
using System.Text.RegularExpressions;

namespace Content.Server.Chat.Systems;

public sealed partial class ChatSystem
{
    private static readonly Dictionary<string, string> SlangReplace = new()
    {
        // Game
        { "хос", "гсб" },
        { "хоса", "гсб" },
        { "смо", "гв" },
        { "се", "си" },
        { "хоп", "гп" },
        { "хопа", "гп" },
        { "рд", "нр" },
        { "вард", "смотритель" },
        { "варден", "смотритель" },
        { "вардена", "смотрителя" },
        { "геник", "генератор" },
        { "кк", "красный код" },
        { "ск", "синий код" },
        { "зк", "зелёный код" },
        { "пда", "кпк" },
        { "дэк", "детектив" },
        { "дэку", "детективу" },
        { "дэка", "детектива" },
        { "дек", "детектив" },
        { "деку", "детективу" },
        { "дека", "детектива" },
        { "мш", "имплант защиты разума" },
        { "трейтор", "предатель" },
        { "инж", "инженер" },
        { "инжи", "инженеры" },
        { "инжы", "инженеры" },
        { "инжу", "инженеру" },
        { "инжам", "инженерам" },
        { "инжинер", "инженер" },
        { "яо", "яой" }, // braindead
        { "нюк", "ядерный оперативник" },
        { "нюкеры", "ядерные оперативники" },
        { "нюкер", "ядерный оперативник" },
        { "нюкеровец", "ядерный оперативник" },
        { "нюкеров", "ядерных оперативников" },
        { "аирлок", "шлюз" },
        { "аирлоки", "шлюзы" },
        { "айрлок", "шлюз" },
        { "айрлоки", "шлюзы" },
        { "визард", "волшебник" },
        { "дизарм", "толчок" },
        { "синга", "сингулярность" },
        { "сингу", "сингулярность" },
        { "синги", "сингулярности" },
        { "сингой", "сингулярностью" },
        { "разгерм", "разгерметизация" },
        { "разгерма", "разгерметизация" },
        { "разгерму", "разгерметизацию" },
        { "разгермы", "разгерметизации" },
        { "разгермой", "разгерметизацией" },
        { "бикардин", "бикаридин" },
        { "бика", "бикаридин" },
        { "бику", "бикаридин" },
        { "декса", "дексалин" },
        { "дексу", "дексалин" },
        // IC
        { "хз", "не знаю" },
        { "синд", "синдикат" },
        { "пон", "понятно" },
        { "непон", "не понятно" },
        { "нипон", "не понятно" },
        { "кста", "кстати" },
        { "кст", "кстати" },
        { "плз", "пожалуйста" },
        { "пж", "пожалуйста" },
        { "спс", "спасибо" },
        { "сяб", "спасибо" },
        { "прив", "привет" },
        { "ок", "окей" },
        { "чел", "мужик" },
        { "лан", "ладно" },
        { "збс", "заебись" },
        { "мб", "может быть" },
        { "оч", "очень" },
        { "омг", "боже мой" },
        { "нзч", "не за что" },
        { "пок", "пока" },
        { "бб", "пока" },
        { "пох", "плевать" },
        { "ясн", "ясно" },
        { "всм", "всмысле" },
        { "чзх", "что за херня?" },
        { "изи", "легко" },
        { "гг", "хорошо сработано" },
        { "пруф", "доказательство" },
        { "пруфани", "докажи" },
        { "пруфанул", "доказал" },
        { "брух", "мда..." },
        { "имба", "нечестно" },
        { "разлокать", "разблокировать" },
        { "юзать", "использовать" },
        { "юзай", "используй" },
        { "юзнул", "использовал" },
        { "хилл", "лечение" },
        { "подхиль", "полечи" },
        { "хильни", "полечи" },
        { "хелп", "помоги" },
        { "хелпани", "помоги" },
        { "хелпанул", "помог" },
        { "рофл", "прикол" },
        { "рофлишь", "шутишь" },
        { "крч", "короче говоря" },
        { "шатл", "шаттл" },
        // OOC
        { "афк", "ссд" },
        { "админ", "бог" },
        { "админы", "боги" },
        { "админов", "богов" },
        { "забанят", "покарают" },
        { "бан", "наказание" },
        { "пермач", "наказание" },
        { "перм", "наказание" },
        { "запермили", "наказание" },
        { "запермят", "накажут" },
        { "нонрп", "плохо" },
        { "нрп", "плохо" },
        { "ерп", "ужас" },
        { "рдм", "плохо" },
        { "дм", "плохо" },
        { "гриф", "плохо" },
        { "фрикил", "плохо" },
        { "фрикилл", "плохо" },
        { "лкм", "левая рука" },
        { "пкм", "правая рука" },
        { "Пидор", "кхе-кхе" },
        { "Пидорас", "кхе-кхе" },
        { "Пидорасина", "кхе-кхе" },
        { "Пидар", "кхе-кхе" },
        { "Педар", "кхе-кхе" },
        { "Пииддаарр", "кхе-кхе" },
        { "пидор", "кхе-кхе" },
        { "пидорас", "кхе-кхе" },
        { "пидарас", "кхе-кхе" },
        { "педик", "кхе-кхе" },
        { "даун", "кхе-кхе" },
        { "слава Украине", "кхе-кхе" },
        { "Слава Украине", "кхе-кхе" },
        { "слава России", "кхе-кхе" },
        { "Слава России", "кхе-кхе" },
        { "ватник", "кхе-кхе" },
        { "хохол", "кхе-кхе" },
        { "порно", "кхе-кхе" },
        { "яой", "мультики" },
        { "нигер", "кхе-кхе" },
        { "негр", "кхе-кхе" },
        { "нееггрр", "кхе-кхе" },
        { "ниггер", "кхе-кхе" },
        { "негер", "кхе-кхе" },
        { "нигир", "кхе-кхе" },
        { "нига", "кхе-кхе" },
        { "Негры", "кхе-кхе" },
        { "негра", "кхе-кхе" },
        { "негру", "кхе-кхе" },
        { "негры", "кхе-кхе" },
    };

    private string ReplaceWords(string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;

        return Regex.Replace(message, "\\b(\\w+)\\b", match =>
        {
            bool isUpperCase = match.Value.All(Char.IsUpper);

            if (SlangReplace.TryGetValue(match.Value.ToLower(), out var replacement))
                return isUpperCase ? replacement.ToUpper() : replacement;
            return match.Value;
        });
    }
}
