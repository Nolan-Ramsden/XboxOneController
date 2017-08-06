using System;
using System.Threading.Tasks;

namespace XboxOneController
{
    public interface IXboxController
    {
        /// <summary>
        /// Turns the XBOX on
        /// </summary>
        /// <returns>A task that awaits the completion of the on command</returns>
        /// <remarks>This command is prone to failing (not sure why), so it is retried internally a few times</remarks>
        Task TurnOn();

        /// <summary>
        /// Turns the XBOX off
        /// </summary>
        /// <returns>A task that awaits the completion of the on command</returns>
        /// <remarks>This command is not yet implemented</remarks>
        Task TurnOff();

        /// <summary>
        /// Checks if the XBOX is on
        /// </summary>
        /// <returns>A task that awaits the completion of the ping command</returns>
        /// <remarks>Throws a timeout exception if the XBOX is off</remarks>
        Task Ping();
    }
}
