using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace ZanyStreams
{
    public interface IStreamPublisher
    {
        IEnumerable<Stream> GetStreams();
    }

    public interface IStream<T>
    {
        IObservable<T> GetData();
        string Name { get; set; }
    }

    public class Stream<T> : IStream<T>
    {
        public string Name { get; set; }

        private Func<IObservable<T>> getData;

        public IObservable<T> GetData()
        {
            return getData();
        }

        public Stream(string name, Func<IObservable<T>> getData)
        {
            Name = name;
            this.getData = getData;
        }
    }

    public interface IStream : IStream<object> { }

    public class Stream : Stream<object>
    {
        public Stream(string name, Func<IObservable<object>> getData) : base(name, getData) { }
    }
}
