using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Framework
{
    public static class PasswordValidator
    {
        private static Dictionary<char, int> codeTable = new Dictionary<char, int>();

        static PasswordValidator()
        {
            string[] rows = { "`1234567890-=", "qwertyuiop[]", "asdfghjkl;'", "zxcvbnm,.", " ", "\\" };

            for (int i = 0; i < rows.Length; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    codeTable[rows[i][j]] = i * 100 + j;
                }
            }
        }

        private static int GetKeyboardCode(char ch)
        {
            if (!codeTable.ContainsKey(ch))
            {
                throw new ArgumentException();
            }
            return codeTable[ch];
        }

        public static bool Check(string password, out string message, int minLen = 6, int maxRepeats = 2, bool checkKeyboard = true)
        {
            password = password ?? "";
            message = "";

            if (password.Length < minLen)
            {
                message = String.Format("Пароль должен быть не менее {0} символов", minLen);
                //Console.WriteLine("> length");
                return false;
            }

            if (password.ToCharArray().All(x => char.IsDigit(x)))
            {
                message = "Пароль не должен содержать только цифры";
                //Console.WriteLine("> digit");
                return false;
            }

            int repeats = (from ch in password.ToCharArray()
                           group ch by ch into gr
                           select new { count = gr.Count() }).Max(x => x.count);

            if (repeats > maxRepeats)
            {
                message = String.Format("Пароль не должен содержать {0} одинаковых символа подряд", maxRepeats);
                //Console.WriteLine("> repeats");
                return false;
            }

            if (checkKeyboard)
            {
                password = password.ToLower();

                string[] repl = { "~!@#$%^&*()_+{}:\"<>?|", "`1234567890-=[];',./\\" };
                for (int i = 0; i < repl[0].Length; i++)
                {
                    password = password.Replace(repl[0][i], repl[1][i]);
                }

                try
                {
                    for (int i = 2; i < password.Length; i++)
                    {
                        if (Math.Abs(GetKeyboardCode(password[i]) - GetKeyboardCode(password[i - 1])) <= 1 &&
                            Math.Abs(GetKeyboardCode(password[i - 1]) - GetKeyboardCode(password[i - 2])) <= 1)
                        {
                            message = "Пароль не должен содержать более трех последовательных на клавиатуре символов";

                            //Console.WriteLine("> in row");
                            return false;
                        }
                    }
                }
                catch
                {
                    message = "Пароль не должен содержать более трех последовательных на клавиатуре символов";
                    return false;
                }
            }

            Regex allowed = new Regex(@"^(\d|[a-zA-Z]|(!|@|#|\$|%|\^|&|\*|\(|\)|-|=|_|\+|\[|\]|;|:|'|\""|\\|\||,|<|\.|>|/|\?))+$");
            if(!allowed.IsMatch(password))
            {
                message = "Пароль содержит недопустимые символы";
                return false;
            }

            return true;
        }
    }
}
