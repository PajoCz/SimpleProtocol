using System.Threading;
using SimpleProtocol.Contract;

namespace SimpleProtocol.Engine
{
    public class LoginThreadCurrentPrincipal : ILogin
    {
        public string Login => Thread.CurrentPrincipal?.Identity?.Name;
    }
}