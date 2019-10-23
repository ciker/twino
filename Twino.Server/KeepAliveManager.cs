using System;

namespace Twino.Server
{
    internal class KeepAliveManager
    {
        private TimeoutHandler[] _timeoutHandlers;

        /// <summary>
        /// If true, keep alive manager and timeout handlers are running
        /// </summary>
        public bool IsRunning { get; private set; }
        private int _nextIndex;

        /// <summary>
        /// Creates timeout handlers (vCPU x 2 handlers) and runs them
        /// </summary>
        public void Start(int timeoutMilliseconds)
        {
            if (IsRunning)
                throw new InvalidOperationException("Keep Alive Manager is already running");

            int count = Environment.ProcessorCount * 2;
            _timeoutHandlers = new TimeoutHandler[count];
            
            IsRunning = true;
            _nextIndex = 0;
            Random rnd = new Random();
            
            for (int i = 0; i < _timeoutHandlers.Length; i++)
            {
                TimeoutHandler handler = new TimeoutHandler(timeoutMilliseconds, rnd.Next(500, 1500));
                _timeoutHandlers[i] = handler;
                handler.Start();
            }
        }

        /// <summary>
        /// Stops all timeout handlers
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
            
            foreach (var handler in _timeoutHandlers)
            {
                if (handler == null)
                    continue;

                handler.Stop();
            }
        }

        /// <summary>
        /// Adds new connection to keep alive manager.
        /// This connection's timeout will be set in this method and starts it's timeout span
        /// </summary>
        public void Add(HandshakeInfo info)
        {
            int i = _nextIndex++;
            
            if (_nextIndex >= _timeoutHandlers.Length)
                _nextIndex = 0;
            
            if (i >= _timeoutHandlers.Length)
                i = 0;

            _timeoutHandlers[i].Add(info);
        }
    }
}