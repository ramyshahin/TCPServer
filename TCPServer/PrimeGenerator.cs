using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    class PrimeGenerator
    {
        static Random rnd = new Random();

        protected static bool isPrime(int num)
        {
            if (num < 2)
            {
                return false;
            }

            int d = 2;
            while (d * d <= num)
            {
                if (num % d == 0)
                {
                    return false;
                }

                d++;
            }

            return true;
        }

        static public int GetRandomPrime()
        {
            bool prime = false;
            int num = 0;

            while(!prime)
            {
                num = rnd.Next();
                prime = isPrime(num);
#if DEBUG
                Console.WriteLine("{0}\t{1}", num, prime);
#endif
            }

            return num;
        }
    }
}
