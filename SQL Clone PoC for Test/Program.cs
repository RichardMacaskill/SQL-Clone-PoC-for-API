using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RedGate.SqlClone.Client.Api;

namespace SQL_Clone_PoC_for_Test
{
    internal class Program
    {
        private static void  Main(string[] args)
        {
            var exerciser = new CloneExerciser();
            exerciser.Exercise().Wait();
        }
    }

    internal class CloneExerciser
    {
        private readonly CancellationTokenSource  _cancellationTokenSource = new CancellationTokenSource();
        private readonly SqlCloneClient _cloneClient = new SqlCloneClient();

        public async Task Exercise()
        {
            Console.WriteLine("Connecting to SQL Clone Server");
            var cancellationToken = _cancellationTokenSource.Token;
            await _cloneClient.Connect(new Uri("http://rm-win10-sql201.testnet.red-gate.com:14145"), cancellationToken);
            Console.WriteLine("Connected");
            var image = await _cloneClient.GetImage("StackOverflow Jan 2017", cancellationToken);
            var sqlInstances = await _cloneClient.GetSqlServerInstances(cancellationToken);
            foreach (var sqlInstance in sqlInstances)
            {
                Console.WriteLine("New clone requested for instance {0} on server {1}", sqlInstance.Instance, sqlInstance.Server);
                var operation = await _cloneClient.CreateClone(image, "_SO_Latest", sqlInstance, cancellationToken);
                await _cloneClient.WaitForOperationToComplete(operation, TimeSpan.MaxValue, cancellationToken);
                Console.WriteLine("Created clone for instance {0} on server {1}" , sqlInstance.Instance,sqlInstance.Server);
            }
            
            Console.WriteLine("Finished. Press any key.");
            Console.ReadLine();
        }


        }


}
