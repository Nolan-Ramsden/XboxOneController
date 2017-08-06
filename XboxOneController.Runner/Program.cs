using System;
using System.Threading.Tasks;

namespace XboxOneController.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            IXboxController controller = new XboxController("10.0.0.82", "XXXXXXXXXXXX");

            Run(controller).GetAwaiter().GetResult();
        }

        static async Task Run(IXboxController controller)
        {
            await controller.TurnOn();

            await controller.Ping();

            await controller.TurnOff();
        }
    }
}