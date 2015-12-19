using System.Collections.ObjectModel;

namespace CSharpChatClient.Model
{
    public class ExternalUserList : ObservableCollection<ExternalUser>
    {
        public ExternalUserList() : base()
        {
        }
    }
}