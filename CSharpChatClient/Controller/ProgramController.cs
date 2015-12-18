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
        private ChatForm chatForm;
        private GraphicalInterfaceController graphicControl;
        private NetworkService networkService;
        private FileService fileService;

        public ProgramController(ChatForm chatForm)
        {
            this.chatForm = chatForm;
            InitiateController();
        }

        private void InitiateController()
        {
            Debug.WriteLine("\n\nNew Program start");
            fileService = new FileService(this);
            networkService = new NetworkService(this);
            graphicControl = new GraphicalInterfaceController(this, chatForm, networkService);

            networkService.Start();
        }

        internal void Stop()
        {
            networkService.Stop();
            Logger.Log("Moin");
        }

        public ChatForm ChatForm
        {
            get { return chatForm; }
            //set { chatForm = value; }
        }
        public GraphicalInterfaceController GraphicControl
        {
            get { return graphicControl; }
            //set { graphicControl = value; }
        }
        public NetworkService NetworkService
        {
            get { return networkService; }
            //set { networkService = value; }
        }
        public FileService FileService
        {
            get { return fileService; }
            //set { fileService = value; }
        }

    }
}
