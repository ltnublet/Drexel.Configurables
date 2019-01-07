using System;

namespace Drexel.Configurables.Example.Contracts
{
    public class ConsoleInstance
    {
        public int Read()
        {
            return Console.Read();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Write(string value)
        {
            Console.Write(value);
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}
