using System;
using System.Windows.Forms;

namespace CSharpChatClient
{
    internal static class Program
    {
        /// <summary>
        /// The main entrance to the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChatForm());
        }
    }
}