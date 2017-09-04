using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Exceptions
{
    public class LoginException :Exception
    {

        public ClientLoginStateEnum LoginType { get; private set; }


        public LoginException(string message, ClientLoginStateEnum loginType) : base(message)
        {
            LoginType = loginType;
        }

    }
}
