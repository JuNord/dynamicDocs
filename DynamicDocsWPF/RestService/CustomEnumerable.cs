using System.Collections;
using System.Collections.Generic;

namespace RestService
{
    public class CustomEnumerable<T> : IEnumerator<T>, IEnumerable<T>
    {
        private int _index;
        private readonly List<T> _elements;


        public CustomEnumerable(List<T> elements)
        {
            _elements = elements;
            _index = -1;
        }
        
        public void Dispose()
        {
            Reset();
        }

        public bool HasNext()
        {
            return _index < _elements.Count - 1;
        }
        
        public bool MoveNext()
        {
            if (_index + 1 < _elements.Count)
            {
                _index++;
                return true;
            }
            else return false;
        }

        public bool MoveBack()
        {
            if (_index - 1 >= 0)
            {
                _index--;
                return true;
            }
            else return false;
        }

        public void Reset()
        {
            _index = -1;
        }

        public T Current => _elements[_index];

        object IEnumerator.Current => Current;

        public IEnumerator<T> GetEnumerator() => this;
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}