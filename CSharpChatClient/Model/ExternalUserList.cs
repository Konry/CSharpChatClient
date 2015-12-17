using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Model
{
    public class ExternalUserList : ObservableCollection<ExternalUser>
    {
        public ExternalUserList() : base()
        {

        }
    }
}
