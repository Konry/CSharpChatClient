using System.Collections.ObjectModel;

namespace CSharpChatClient.Model
{
    public class ExtendedUserList : ObservableCollection<ExtendedUser>
    {
        public ExtendedUserList() : base()
        {
        }
    }
}