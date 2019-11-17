using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NolanHodge.Threader
{
    /// <summary>
    /// IThreader interfaces need supply only a few methods
    /// </summary>
    public interface IThreader
    {
        /// <summary>
        /// Action is called by the Threader class during parallel execution.
        /// </summary>
        void Action();
        /// <summary>
        /// Name is used by Threader to maintain the state of the IThreader running in parallel.
        /// </summary>
        string Name { get; set; }
    }

    /// <summary>
    /// A Threader is an object that maintains Task states and supplies them to a user like they are Threads.
    /// </summary>
    public class Threader
    {
        Dictionary<string, List<Task>> Tasks;
        Dictionary<string, CancellationTokenSource> Tokens;

        public Threader()
        {
            Tasks = new Dictionary<string, List<Task>>();
            Tokens = new Dictionary<string, CancellationTokenSource>();
        }

        /// <summary>
        /// Spawn a new thread running the Action supplied by the IThreader.
        /// Multiple IThreaders under the same name can run together, although they will be killed together unless given unique names.
        /// </summary>
        /// <param name="threader"> The IThreader object to be maintained by Threader </param>
        public void Spawn(IThreader threader)
        {
            if (!Tasks.ContainsKey(threader.Name))
            {
                Tasks[threader.Name] = new List<Task>();
                Tokens[threader.Name] = new CancellationTokenSource();
            }

            Task T = new Task(threader.Action, Tokens[threader.Name].Token, TaskCreationOptions.LongRunning);

            Tasks[threader.Name].Add(T);

            T.Start();
        }

        /// <summary>
        /// Killed will return the stopped/running state of a IThreader object.
        /// </summary>
        /// <param name="threader">The IThreader object being maintained by Threader</param>
        /// <returns></returns>
        public bool Killed(IThreader threader)
        {
            if (!Tokens.ContainsKey(threader.Name))
            {
                try
                {
                    return Tokens[threader.Name].IsCancellationRequested;
                }
                catch { return true; }
            }

            return false;
        }

        /// <summary>
        /// Kill will send a signal to stop all IThreader objects under the name IThreader.Name.
        /// </summary>
        /// <param name="threader">The IThreader object being maintained by Threader</param>
        public void Kill(IThreader threader)
        {
            if (Tokens.ContainsKey(threader.Name))
            {
                Tokens[threader.Name].Cancel();
            }
        }

        /// <summary>
        /// KillAll will send a signal to stop all of the IThreader objects maintained by Threader.
        /// </summary>
        public void KillAll()
        {
            foreach (CancellationTokenSource C in Tokens.Values)
            {
                C.Cancel();
            }
        }
    }
}
