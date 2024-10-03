using FP700Win.Interfaces;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace FP700Win.Implementation
{
    public class MessageAggregator : IMessageAggregator
    {
        private readonly object _observablesByTypeKeyLock = new object();

        private readonly Dictionary<string, Tuple<object, object>> _observablesByTypeKey = new Dictionary<string, Tuple<object, object>>();

        public IObservable<T> GetStream<T>()
        {
            string key = typeof(T).ToString();
            IObservable<T> stream;
            lock (_observablesByTypeKeyLock)
            {
                if (_observablesByTypeKey.ContainsKey(key))
                {
                    stream = (IObservable<T>)_observablesByTypeKey[key].Item2;
                }
                else
                {
                    Subject<T> obj = (Subject<T>)Activator.CreateInstance(typeof(Subject<>).MakeGenericType(typeof(T)), new object[0]);
                    stream = obj.Publish().RefCount();
                    Tuple<object, object> tuple = new Tuple<object, object>(obj, stream);
                    _observablesByTypeKey.Add(key, tuple);
                }
            }
            return stream;
        }

        public void Publish<T>(T payload)
        {
            string key = typeof(T).ToString();
            Tuple<object, object> tuple;
            lock (_observablesByTypeKeyLock)
            {
                _observablesByTypeKey.TryGetValue(key, out tuple);
            }
            if (tuple != null)
            {
                ((Subject<T>)tuple.Item1).OnNext(payload);
            }
        }
    }
}
