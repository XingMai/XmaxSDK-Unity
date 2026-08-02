/*
MIT License

Copyright (c) 2017 Marc

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace bytertc
{
    /// <summary>
    /// thread schedule
    /// </summary>
    public class Loom : MonoBehaviour
    {
        public static int maxThreads = 8;
        static int numThreads;

        private static Loom _current;
        public static Loom Current
        {
            get
            {
                Initialize();
                return _current;
            }
        }

        static bool initialized;

        //#### only need to call once when initialize.
        public static void Initialize()
        {
            if (!initialized)
            {

                if (!Application.isPlaying)
                    return;
                initialized = true;
                GameObject g = new GameObject("Loom");
                //#### never destroy
                DontDestroyOnLoad(g);
                _current = g.AddComponent<Loom>();
            }

        }

        private List<Action> _actions = new List<Action>();
        public struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }
        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        public static void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }
        public static void QueueOnMainThread(Action action, float time)
        {
            if (time != 0)
            {
                if (Current != null)
                {
                    lock (Current._delayed)
                    {
                        Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                    }
                }
            }
            else
            {
                if (Current != null)
                {
                    lock (Current._actions)
                    {
                        Current._actions.Add(action);
                    }
                }
            }
        }

        public static Thread RunAsync(Action a)
        {
            Initialize();
            while (numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }

        }


        void OnDisable()
        {
            if (_current == this)
            {

                _current = null;
            }
        }



        // Use this for initialization  
        void Start()
        {

        }

        List<Action> _currentActions = new List<Action>();

        // Update is called once per frame  
        void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }
            foreach (var a in _currentActions)
            {
                a();
            }
            lock (_delayed)
            {
                _currentDelayed.Clear();
                foreach (var item in _delayed)
                {
                    if (item.time <= Time.time)
                    {
                        _currentDelayed.Add(item);
                    }
                }
                foreach (var item in _currentDelayed)
                    _delayed.Remove(item);
            }
            foreach (var delayed in _currentDelayed)
            {
                delayed.action();
            }
        }
    }
}
