using Base.Censorship.Entities;
using Base.Settings;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Base.Censorship.Service
{
    public class CensorshipService: ICensorshipService
    {
        private readonly ISettingItemService _settingItemService;

        public CensorshipService(ISettingItemService settingItemService)
        {
            _settingItemService = settingItemService;
        }

        public void CheckObsceneLexis(string message)
        {
            var config = _settingItemService.GetValue(Consts.Settings.KEY_CONFIG, null) as CensorshipConfig;
            if (config == null)
                throw new NullReferenceException("CensorshipConfig is null");

            if (!config.TurnOn) return;

            Regex wordFilter = new Regex(config.Regex, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

            var match = wordFilter.Match(message.Replace('ё', 'е'));

            while (match.Success)
            {
                if (!config.WhiteListArray.Any(x => match.Value.Contains(x)))
                    throw new CensorshipException("Текст содержит недопустимое слово: " + match.Value);

                match = match.NextMatch();
            }  
        }
    }
}
