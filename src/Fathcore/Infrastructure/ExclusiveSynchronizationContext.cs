using System;
using System.Collections.Generic;
using System.Threading;

namespace Fathcore.Infrastructure
{
    /// <summary>
    /// Represents exclusive synchronization context.
    /// </summary>
    internal class ExclusiveSynchronizationContext : SynchronizationContext
    {
        private bool _done;
        readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);
        readonly Queue<Tuple<SendOrPostCallback, object>> _items =
            new Queue<Tuple<SendOrPostCallback, object>>();

        /// <summary>
        /// Gets or sets an inner exception.
        /// </summary>
        public Exception InnerException { get; set; }

        /// <summary>
        /// Dispatches a synchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="SendOrPostCallback"/> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException("We cannot send to our same thread");
        }

        /// <summary>
        /// Dispatches an asynchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="SendOrPostCallback"/> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            lock (_items)
            {
                _items.Enqueue(Tuple.Create(d, state));
            }
            _workItemsWaiting.Set();
        }

        /// <summary>
        /// Begin message loop.
        /// </summary>
        public void BeginMessageLoop()
        {
            while (!_done)
            {
                Tuple<SendOrPostCallback, object> task = null;
                lock (_items)
                {
                    if (_items.Count > 0)
                    {
                        task = _items.Dequeue();
                    }
                }
                if (task != null)
                {
                    task.Item1(task.Item2);
                    if (InnerException != null) // the method threw an exeption
                    {
                        throw new AggregateException("TaskExtensions.RunSync method threw an exception.", InnerException);
                    }
                }
                else
                {
                    _workItemsWaiting.WaitOne();
                }
            }
        }

        /// <summary>
        /// End message loop.
        /// </summary>
        public void EndMessageLoop()
        {
            Post(_ => _done = true, null);
        }

        /// <summary>
        /// Creates a copy of the synchronization context.
        /// </summary>
        /// <returns>A new <see cref="SynchronizationContext"/> object.</returns>
        public override SynchronizationContext CreateCopy()
        {
            return this;
        }
    }
}
