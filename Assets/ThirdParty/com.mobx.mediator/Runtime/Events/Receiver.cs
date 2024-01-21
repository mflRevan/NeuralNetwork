using System;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace MobX.Mediator.Events
{
    public class Receiver : IReceiver
    {
        #region Member Variables

        public int Count { get; private set; }
        public Action this[int index] => _listener[index];

        private Action[] _listener;

        #endregion


        #region Ctor

        public Receiver(int initialCapacity)
        {
            _listener = new Action[initialCapacity];
        }

        public Receiver()
        {
            _listener = new Action[8];
        }

        #endregion


        //--------------------------------------------------------------------------------------------------------------


        #region Add Listener

        /// <inheritdoc />
        public bool AddUnique(Action listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <inheritdoc />
        public void Add(Action listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;

            Count++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
        public bool Contains(Action listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <inheritdoc />
        public bool Remove(Action listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i] == listener)
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void RemoveAt(int index)
        {
            --Count;
            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;
            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseInternal()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i]();
            }
        }

        #endregion
    }

    public class Receiver<T> : IReceiver<T>
    {
        #region Member Variables

        public Action<T> this[int index] => _listener[index];

        public int Count { get; private set; }
        private Action<T>[] _listener;

        #endregion


        #region Ctor

        public Receiver(int initialCapacity)
        {
            _listener = new Action<T>[initialCapacity];
        }

        public Receiver()
        {
            _listener = new Action<T>[8];
        }

        #endregion


        //--------------------------------------------------------------------------------------------------------------


        #region Add Listener

        /// <inheritdoc />
        public bool AddUnique(Action<T> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <inheritdoc />
        public void Add(Action<T> listener)
        {
            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;

            Count++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
        public bool Contains(Action<T> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <inheritdoc />
        public bool Remove(Action<T> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void RemoveAt(int index)
        {
            --Count;
            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;
            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseInternal(T arg)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](arg);
            }
        }

        #endregion
    }

    public class Receiver<T1, T2> : IReceiver<T1, T2>
    {
        #region Member Variables

        public Action<T1, T2> this[int index] => _listener[index];

        public int Count { get; private set; }
        private Action<T1, T2>[] _listener;

        #endregion


        #region Ctor

        public Receiver(int initialCapacity)
        {
            _listener = new Action<T1, T2>[initialCapacity];
        }

        public Receiver()
        {
            _listener = new Action<T1, T2>[8];
        }

        #endregion


        //--------------------------------------------------------------------------------------------------------------


        #region Add Listener

        /// <inheritdoc />
        public bool AddUnique(Action<T1, T2> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <inheritdoc />
        public void Add(Action<T1, T2> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;

            Count++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
        public bool Contains(Action<T1, T2> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <inheritdoc />
        public bool Remove(Action<T1, T2> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void RemoveAt(int index)
        {
            --Count;
            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;
            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseInternal(T1 first, T2 second)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](first, second);
            }
        }

        #endregion
    }

    public class Receiver<T1, T2, T3> : IReceiver<T1, T2, T3>
    {
        #region Member Variables

        public Action<T1, T2, T3> this[int index] => _listener[index];

        public int Count { get; private set; }
        private Action<T1, T2, T3>[] _listener;

        #endregion


        #region Ctor

        public Receiver(int initialCapacity)
        {
            _listener = new Action<T1, T2, T3>[initialCapacity];
        }

        public Receiver()
        {
            _listener = new Action<T1, T2, T3>[8];
        }

        #endregion


        //--------------------------------------------------------------------------------------------------------------


        #region Add Listener

        /// <inheritdoc />
        public bool AddUnique(Action<T1, T2, T3> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <inheritdoc />
        public void Add(Action<T1, T2, T3> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;

            Count++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2, T3>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
        public bool Contains(Action<T1, T2, T3> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <inheritdoc />
        public bool Remove(Action<T1, T2, T3> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void RemoveAt(int index)
        {
            --Count;
            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;
            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseInternal(T1 first, T2 second, T3 third)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](first, second, third);
            }
        }

        #endregion
    }

    public class Receiver<T1, T2, T3, T4> : IReceiver<T1, T2, T3, T4>
    {
        #region Member Variables

        public Action<T1, T2, T3, T4> this[int index] => _listener[index];

        public int Count { get; private set; }
        private Action<T1, T2, T3, T4>[] _listener;

        #endregion


        #region Ctor

        public Receiver(int initialCapacity)
        {
            _listener = new Action<T1, T2, T3, T4>[initialCapacity];
        }

        public Receiver()
        {
            _listener = new Action<T1, T2, T3, T4>[8];
        }

        #endregion


        //--------------------------------------------------------------------------------------------------------------


        #region Add Listener

        /// <inheritdoc />
        public bool AddUnique(Action<T1, T2, T3, T4> listener)
        {
            if (Contains(listener))
            {
                return false;
            }

            Add(listener);
            return true;
        }

        /// <inheritdoc />
        public void Add(Action<T1, T2, T3, T4> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= Count)
            {
                IncreaseCapacity();
            }

            _listener[Count] = listener;

            Count++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2, T3, T4>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        /// <inheritdoc />
        public bool Contains(Action<T1, T2, T3, T4> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region Remove Listener

        /// <inheritdoc />
        public bool Remove(Action<T1, T2, T3, T4> listener)
        {
            for (var i = 0; i < Count; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void RemoveAt(int index)
        {
            --Count;
            for (var i = index; i < Count; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[Count] = null;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;
            for (var i = _listener.Length - 1; i >= 0; i--)
            {
                _listener[i] = null;
            }
        }

        /// <inheritdoc />
        public void ClearInvalid()
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                if (_listener[i] is null || _listener[i].Target == null)
                {
                    RemoveAt(i);
                }
            }
        }

        #endregion


        #region Raise

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void RaiseInternal(T1 first, T2 second, T3 third, T4 fourth)
        {
            for (var i = Count - 1; i >= 0; i--)
            {
                _listener[i](first, second, third, fourth);
            }
        }

        #endregion
    }
}
