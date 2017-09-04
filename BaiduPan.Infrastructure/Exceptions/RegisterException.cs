using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Exceptions
{
    public class RegisterException : Exception
    {

        public RegisterStateEnum RegisterType { get; private set; }

        public RegisterException(string message, RegisterStateEnum registerType) :base(message)
        {
            RegisterType = registerType;
        }


    }
}
