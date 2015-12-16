using CSharpChatClient.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.controller
{
    public class ProgramController
    {
        public ChatForm chatForm { get; set; }
        public GraphicalInterfaceController graphicControl { get; set; }
        public NetworkService networkService { get; set; }
        public FileService fileService {get; set;}

        public ProgramController(ChatForm chatForm)
        {
            this.chatForm = chatForm;
            InitiateController();
        }

        private void InitiateController()
        {
            Debug.WriteLine("\n\nNew Program start");
            networkService = new NetworkService(this);
            graphicControl = new GraphicalInterfaceController(this, chatForm, networkService);
            fileService = new FileService(this);
        }

    }
}
