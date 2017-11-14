using SimpleProtocol.Contract;

namespace SimpleProtocol.Engine
{
    public class LoginNullImpl : ILogin
    {
        public LoginNullImpl(string p_DefaultValue = null)
        {
            Login = p_DefaultValue;
        }

        public string Login { get; }
    }
}