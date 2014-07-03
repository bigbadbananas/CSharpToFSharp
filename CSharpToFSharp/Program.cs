using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientUtils.FsharpLib;

namespace CSharpToFSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int ans1 = SomeMathShit.factorial1(6);
            int ans2 = SomeMathShit.factorial2(6);
            int ans3 = SomeMathShit.factorial3(6);
            int ans4 = SomeMathShit.factorial4(6);

            Console.WriteLine("First F# factorial: " + ans1);
            Console.WriteLine("Second F# factorial: " + ans2);
            Console.WriteLine("Third F# factorial: " + ans3);
            Console.WriteLine("Fourth F# factorial: " + ans4);
        }
    }
}
