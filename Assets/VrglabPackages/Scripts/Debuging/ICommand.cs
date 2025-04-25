using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppEngine
{
    public interface ICommand
    {
        void Execute(string[] args, KeyValuePair<string, string>[] options);
    }
}
