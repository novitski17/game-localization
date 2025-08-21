using System.Collections.ObjectModel;

namespace GameLocalization.Infrastructure.Data.Seed
{
    internal static class SeedData
    {
        public static readonly ReadOnlyCollection<string> Keys = Array.AsReadOnly(new[]
        {
            "app.title",
            "menu.play",
            "menu.settings",
            "menu.exit",
            "label.language",
            "label.username",
            "msg.welcome",
            "msg.goodbye",
            "error.network",
            "tooltip.save"
        });

        private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> TranslationsByLang =
            new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["en"] = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["app.title"] = "Game Title",
                    ["menu.play"] = "Play",
                    ["menu.settings"] = "Settings",
                    ["menu.exit"] = "Exit",
                    ["label.language"] = "Language",
                    ["label.username"] = "Username",
                    ["msg.welcome"] = "Welcome!",
                    ["msg.goodbye"] = "Goodbye!",
                    ["error.network"] = "Network error",
                    ["tooltip.save"] = "Save",
                },
                ["ru"] = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["app.title"] = "Название игры",
                    ["menu.play"] = "Играть",
                    ["menu.settings"] = "Настройки",
                    ["menu.exit"] = "Выход",
                    ["label.language"] = "Язык",
                    ["label.username"] = "Имя пользователя",
                    ["msg.welcome"] = "Добро пожаловать!",
                    ["msg.goodbye"] = "До встречи!",
                    ["error.network"] = "Ошибка сети",
                    ["tooltip.save"] = "Сохранить",
                },
                ["it"] = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["app.title"] = "Titolo del gioco",
                    ["menu.play"] = "Gioca",
                    ["menu.settings"] = "Impostazioni",
                    ["menu.exit"] = "Esci",
                    ["label.language"] = "Lingua",
                    ["label.username"] = "Nome utente",
                    ["msg.welcome"] = "Benvenuto!",
                    ["msg.goodbye"] = "Arrivederci!",
                    ["error.network"] = "Errore di rete",
                    ["tooltip.save"] = "Salva",
                }
            };

        public static string GetValue(string langCode, string key)
        {
            if (TranslationsByLang.TryGetValue(langCode, out var dict) &&
                dict.TryGetValue(key, out var value))
            {
                return value;
            }

            return string.Empty; 
        }
    }
}