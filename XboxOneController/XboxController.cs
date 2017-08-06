using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XboxOneController
{
    public class XboxController : IXboxController
    {
        public int Port { get; }

        public IPAddress IP { get; }

        public string LiveId { get; }

        public IPEndPoint Endpoint { get; }

        public int TimeoutMs { get; set; } = 1000;

        /// <summary>
        /// Creates a new instance of the XBOX controller
        /// </summary>
        /// <param name="ipAddress">The IP Address of the XBOX one (Settings->Network->Advanced)</param>
        /// <param name="liveId">The LiveID of the the XBOX one (Settings->System->LiveID)</param>
        /// <param name="port">The port the XBOX is listening on (Default is 5050)</param>
        public XboxController(string ipAddress, string liveId, int port = 5050)
        {
            this.Port = port;
            this.LiveId = liveId;
            this.IP = IPAddress.Parse(ipAddress);
            this.Endpoint = new IPEndPoint(IP, Port);
        }

        /// <summary>
        /// Turns the XBOX off
        /// </summary>
        /// <returns>A task that awaits the completion of the on command</returns>
        /// <remarks>This command is not yet implemented</remarks>
        public Task TurnOff()
        {
            throw new NotImplementedException("Power Off is not completed yet! Feel free to contribute at "
                + @"https://github.com/Nolan-Ramsden/XboxOneController.git");
        }

        /// <summary>
        /// Turns the XBOX on
        /// </summary>
        /// <returns>A task that awaits the completion of the on command</returns>
        /// <remarks>This command is prone to failing (not sure why), so it is retried internally a few times</remarks>
        public async Task TurnOn()
        {
            // Construct the ON command payload,
            // taken from https://github.com/arcreative/xbox-on/blob/master/index.js#L48
            var body = Encoding.UTF8.GetBytes("\x00" + (char)this.LiveId.Length + this.LiveId + "\x00");
            var bodyLength = Encoding.UTF8.GetBytes(char.ToString((char)body.Length));
            var headerStart = HexStringToByteArray("dd0200");
            var headerEnd = HexStringToByteArray("0000");
            var header = CombineBytes(headerStart, bodyLength, headerEnd);
            var payload = CombineBytes(header, body);

            // Try 5 times to turn on and get a response
            const int NUM_TRIES = 5;
            using (var client = new UdpClient())
            {
                for (int i = 0; i < NUM_TRIES; i++)
                {
                    var sendResponse = client.SendAsync(
                        payload,
                        payload.Length,
                        this.Endpoint
                    );
                    try
                    {
                        var recvResponse = await AwaitUdpClientTask(client.ReceiveAsync());
                        break;
                    }
                    catch (TimeoutException) { }
                }
            }
            // Xbox is on, or out of tries. Send a ping to double check either way
            await Ping();
        }

        /// <summary>
        /// Checks if the XBOX is on
        /// </summary>
        /// <returns>A task that awaits the completion of the ping command</returns>
        /// <remarks>Throws a timeout exception if the XBOX is off</remarks>
        public async Task Ping()
        {
            // Shortcut the PING command payload,
            // taken from https://github.com/Schamper/xbox-remote-power/blob/master/xbox-remote-power.py#L5
            const string PING_PACKET = "dd00000a000000000000000400000002";
            byte[] payload = HexStringToByteArray(PING_PACKET);

            using (var client = new UdpClient())
            {
                var sendResponse = await client.SendAsync(
                    payload, 
                    payload.Length, 
                    this.Endpoint
                );
                var recvResponse = await AwaitUdpClientTask(client.ReceiveAsync());
            }
        }

        /// <summary>
        /// Runs a task with a timeout, used for recieving packets from client.RecieveAsync
        /// </summary>
        /// <typeparam name="TResult">The response you are expecting</typeparam>
        /// <param name="task">The task to run</param>
        /// <returns>The response from the task you're running, or a timeout exception</returns>
        protected async Task<TResult> AwaitUdpClientTask<TResult>(Task<TResult> task)
        {
            var timeoutCanceller = new CancellationTokenSource();
            var first = await Task.WhenAny(task, Task.Delay(TimeoutMs, timeoutCanceller.Token));
            if (first == task)
            {
                timeoutCanceller.Cancel();
                await task;
                return task.Result;
            }
            throw new TimeoutException($"Timeout of {TimeoutMs} ms reached");
        }

        /// <summary>
        /// Converts a string of hex characters to a byte array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>a byte array representing the hex string</returns>
        protected static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Concatenates an arbitrary amount of byte arrays to a single array
        /// </summary>
        /// <param name="arrays">The byte arrays</param>
        /// <returns>a byte array of all the args glued together</returns>
        protected static byte[] CombineBytes(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
