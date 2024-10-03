using System;

namespace FP700Win.Interfaces
{
    public interface IMessageAggregator
    {
        IObservable<T> GetStream<T>();
        void Publish<T>(T payload);
    }

}
