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

        /// <summary>
        /// Initiate the controllerModul
        /// </summary>
        private void InitiateController()
        {
            Logger.LogInfo("New Program start");
            fileService = new FileService(this);
            networkService = new NetworkService(this);
            graphicControl = new GraphicalInterfaceController(this, chatForm);

            Logger.LogInfo("Start network service.");
            networkService.Start();
        }

        /// <summary>
        /// Stops the connections and running threads in the controller
        /// </summary>
        internal void Stop()
        {
            Logger.LogInfo("Stop program controller.");
            networkService.Stop();
        }

        public ChatForm ChatForm
        {
            get { return chatForm; }
        }

        public GraphicalInterfaceController GraphicControl
        {
            get { return graphicControl; }
        }

        public NetworkService NetworkService
        {
            get { return networkService; }
        }

        public FileService FileService
        {
            get { return fileService; }
        }
    }
}