using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1sServerWidget
{
    internal class CreateV83ComConnector : Exception
    {
        internal CreateV83ComConnector(string message) : base(message) { }
    }

    internal class ConnectAgentException : Exception 
    {
        internal ConnectAgentException(string message) : base(message) { }
    }

    internal class WorkingProcessException  : Exception
    {
        internal WorkingProcessException(string message) : base(message) { }
    }

    internal class RegistrykeyNotFoundException : Exception
    {
        internal RegistrykeyNotFoundException(string message) : base(message) { }
    }
}
