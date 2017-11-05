using System;
using System.Text.RegularExpressions;

namespace RconTool
{
    class RegexParser
    {
        public static bool IsChat(String line)
        {
            ChatMessage cm = ParseChat(line);
            if(cm != null)
            {
                if(cm.name != null && cm.ip != null && cm.message != null && cm.uid != null)
                {
                    if(!cm.name.Equals("") && !cm.ip.Equals("") && !cm.message.Equals("") && !cm.uid.Equals(""))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static string cregex = "(\\[)((?:[0]?[1-9]|[1][012])[-:\\/.](?:(?:[0-2]?\\d{1})|(?:[3][01]{1}))[-:\\/.](?:(?:\\d{1}\\d{1})))(?![\\d])( )((?:(?:[0-1][0-9])|(?:[2][0-3])|(?:[0-9])):(?:[0-5][0-9])(?::[0-5][0-9])?(?:\\s?(?:am|AM|pm|PM))?)(\\])( )(<)((?:.*))(\\/)((?:.*))(\\/)((?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))(?![\\d])(>)( )((?:.*))";
        public static ChatMessage ParseChat(string chat)
        {
            Regex r = new Regex(cregex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(chat);
            if (m.Success)
            {
                ChatMessage msg = new ChatMessage();
                msg.date = m.Groups[2].ToString();
                msg.time = m.Groups[4].ToString();
                msg.name = m.Groups[8].ToString();
                msg.uid = m.Groups[10].ToString();
                msg.ip = m.Groups[12].ToString();
                msg.message = m.Groups[15].ToString();
                
                return msg;
            }
            return null;
        }
    }
}
