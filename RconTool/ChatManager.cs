using System;

namespace RconTool
{
    class ChatManager
    {
        public static void ProccessChat(Connection connection, ChatMessage cm)
        {
            string newline = "[" + cm.date + " " + cm.time + "] " + cm.name + ": " + cm.message + "";
            String cmd = cm.message.ToLower();
            if (cmd.StartsWith("!version") && cm.uid.Equals("7eb3eb858940196a") && cm.name.Equals("Jaron780"))
            {
                connection.SendPM(cm.name, "Version: " + Form1.toolversion);
            }
            else if (cmd.StartsWith("!who is awesome?") && cm.uid.Equals("7eb3eb858940196a") && cm.name.Equals("Jaron780"))
            {
                connection.SendToRcon("Server.Say \"Jaron is!\" ");
            }
            else if (cm.message.StartsWith("!<3") && cm.uid.Equals("7eb3eb858940196a") && cm.name.Equals("Jaron780"))
            {
                connection.SendToRcon("Server.Say \"<3\"");
            }

            connection.PrintToChat(newline);
        }

    }
}
