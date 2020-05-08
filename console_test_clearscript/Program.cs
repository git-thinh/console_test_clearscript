using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console_test_clearscript
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineJS._init();
                        
            while (true) {
                string json = EngineJS.Execute("function_1");
                Console.WriteLine(json);
                Console.ReadKey();
            }
        }
    }
}
