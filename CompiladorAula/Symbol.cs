using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAula
{
    public class Symbol
    {
        public string Name { get; }
        public TokenTypes Type { get; }
        public bool Initialized { get; set; }

        public Symbol(string name, TokenTypes type, bool initialized)
        {
            Name = name;
            Type = type;
            Initialized = initialized;
        }
    }
}
