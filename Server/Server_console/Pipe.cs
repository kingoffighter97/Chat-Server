using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;

namespace Server_console
{
    class Pipe
    {
        public string Name { get; set; }
        public NamedPipeServerStream pipe { get; set; }
    }
}
