using System;
using System.Linq;

namespace SignalRTestingApplication.CommunicationDefiners
{
    public class InteractiveCommunicationDefiner : CommunicationDefiner
    {
        private const int MinTypeCode = 1;
        private const int MaxTypeCode = 4;

        public void DemandServerUrl()
        {
            Console.WriteLine("Type server's url.");
            var url = Console.ReadLine()?.Trim();

            while (!IsProperUrl(url))
            {
                Console.WriteLine("Type proper server's url.");
                url = Console.ReadLine()?.Trim();
            }

            Url = url;
        }

        public void DemandMethodName()
        {
            Console.WriteLine("Type name of method to receive from server.");
            var name = Console.ReadLine()?.Trim();

            while (!IsProperMethodName(name))
            {
                Console.WriteLine("Type proper method name - no empty, without spaces inside text.");
                name = Console.ReadLine()?.Trim();
            }

            MethodName = name;
        }

        public void DemandParametersNumber()
        {
            Console.WriteLine("Type number of parameters <1;8>.");
            var num = Console.ReadLine()?.Trim();

            while (!IsProperParametersNumberString(num))
            {
                Console.WriteLine("Type proper number of parameters <1;8>");
                num = Console.ReadLine()?.Trim();
            }

            ParametersNumber = Int32.Parse(num);
        }

        public void DemandParametersTypes()
        {
            Console.WriteLine(
                "Choose types of method's arguments. Order is important.\n[1]. bool\n[2]. int\n[3]. double\n[4]. string");

            for (var i = 0; i < ParametersNumber; i++)
            {
                Console.WriteLine($"Parameter no. {i + 1}");

                int chosenInt;
                var chosen = Console.ReadLine()?.Trim();
                while (!IsProperTypeCode(chosen))
                {
                    Console.WriteLine($"Type proper number - <{MinTypeCode};{MaxTypeCode}>.");
                    chosen = Console.ReadLine()?.Trim();
                }

                chosenInt = Int32.Parse(chosen);

                switch (chosenInt)
                {
                    case 1:
                        ParametersTypes.Add(typeof(bool));
                        break;
                    case 2:
                        ParametersTypes.Add(typeof(int));
                        break;
                    case 3:
                        ParametersTypes.Add(typeof(double));
                        break;
                    case 4:
                        ParametersTypes.Add(typeof(string));
                        break;
                    default:
                        Console.WriteLine($"You can choose from {MinTypeCode} to {MaxTypeCode}.");
                        break;
                }
            }
        }

        private static bool IsProperUrl(string url)
        {
            if (null == url)
                return false;
            if (url.Equals(string.Empty))
                return false;
            return true;
        }

        private static bool IsProperMethodName(string name)
        {
            if (null == name)
                return false;
            if (name.Equals(string.Empty))
                return false;
            if (name.Contains(" "))
                return false;
            return true;
        }

        private static bool IsProperParametersNumberString(string number)
        {
            if (null == number)
                return false;

            int parsedLine;
            if (!Int32.TryParse(number, out parsedLine))
                return false;

            if (parsedLine > MaxParametersNumber || parsedLine < MinParametersNumber)
                return false;

            return true;
        }

        private static bool IsProperTypeCode(string number)
        {
            if (null == number)
                return false;

            if (number.Equals(string.Empty))
                return false;

            if (!number.All(Char.IsDigit))
                return false;

            var code = Int32.Parse(number);
            if (code < MinTypeCode || code > MaxTypeCode)
                return false;

            return true;
        }
    }
}