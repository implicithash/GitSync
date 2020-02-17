using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW.Navigator.SCM.GitRepo.Sync
{
    public class ObservableQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        /// <summary>
        /// Triggers an event when a queue changes
        /// </summary>
        public event Action Changed;

        /// <summary>
        /// Defines whether it is possible to update
        /// </summary>
        public bool CanExecute { get; set; }
        protected virtual void OnChanged() => Changed?.Invoke();

        /// <summary>
        /// Adds an object to the end of the queue,
        /// fires the event of updating app configuration
        /// </summary>
        /// <param name="item"></param>
        public virtual void Enqueue(T item)
        {
            _queue.Enqueue(item);
            OnChanged();
        }
        public int Count => _queue.Count;

        /// <summary>
        /// Tries to remove and return the object at the beginning of the queue,
        /// removing of the object fires the event of updating app configuration
        /// </summary>
        /// <returns></returns>
        public virtual T Dequeue()
        {
            if (!CanExecute) return default(T);
            _queue.TryDequeue(out var item);
            OnChanged();
            return item;
        }
    }
}
