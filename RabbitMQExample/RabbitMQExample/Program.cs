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
            //var from = new DateTime(2020, 1, 31);
            //var date = from.AddMonths(1);
            //var list = new List<int>() { 1, 2, 3, 4, 5, 6 };
            //var result = string.Join($",Evaluation-", list);

            var testAtion = Summation(x => x.KeyValue("1", 2));

          
        }

        private static Test Summation(Action<ITest> action)
        {
            var testAtion = new Test();
            action(testAtion);
            return testAtion;
        }

      
    }


    public interface ITest
    {
        void KeyValue(string A, int B);
        int SumFunc(int A, int B);
    }

    public class Test : ITest
    {
        public void KeyValue(string A, int B)
        {
            Console.WriteLine("{A}:{B}");
        }

        public int SumFunc(int A, int B)
        {
            return A + B;
        }
    }
}
