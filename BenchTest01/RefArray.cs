using System;
using System.Runtime.CompilerServices;

namespace BenchTest01
{
    public class RefArray<T> where T : struct
    {
        public RefArray (int initialSize = 16)
        {
            Count = 0;
            _data = new T[initialSize];
            _dataLength = initialSize;
        }

        public int Count { get ; private set; }

        public int Add(ref T data)
        {
            // Check grow
            CheckGrow();

            _data[Count] = data;

            return Count++;
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index < 0 || index >= _dataLength)
                {
                    throw new IndexOutOfRangeException();
                }

                return ref _data[index];
            }
        }

        private void CheckGrow()
        {
            if (Count == _dataLength)
            {
                var newLength = (int)(_data.Length * 1.5f);
                var newArray = new T[newLength];
                _data.CopyTo(newArray, 0);
                _data = newArray;
                _dataLength = newLength;
            }
        }

        private T[] _data;
        private int _dataLength;
    }
}