using CSharpChatClient.Controller;

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
            Logger.LogInfo("New Program start");
            fileService = new FileService(this);
            networkService = new NetworkService(this);
            graphicControl = new GraphicalInterfaceController(this, chatForm, networkService);

            Logger.LogInfo("Start network service.");
            networkService.Start();
        }

        internal void Stop()
        {
            Logger.LogInfo("Stop program controller.");
            networkService.Stop();
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
