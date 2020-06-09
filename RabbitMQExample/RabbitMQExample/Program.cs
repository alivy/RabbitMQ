using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var from = new DateTime(2020, 1, 31);
            var date = from.AddMonths(1);
        }
    }
}
