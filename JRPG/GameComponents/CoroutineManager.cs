using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG
{
    public static class CoroutineManager
    {
        public static List<IEnumerator> _nextToAddCoroutines;
        public static List<IEnumerator> _currentCoroutines;
        public static List<IEnumerator> _nextCoroutines;

        public static void Initialise()
        {
            _nextToAddCoroutines = new List<IEnumerator>();
            _currentCoroutines = new List<IEnumerator>();
            _nextCoroutines = new List<IEnumerator>();
        }

        public static void StartCoroutine(IEnumerator source)
        {
            _nextToAddCoroutines.Add(source);
            source.MoveNext();
        }

        public static float DeltaTime;
        public static void Update(float deltatime)
        {
            DeltaTime = deltatime;
            foreach (IEnumerator coroutine in _currentCoroutines)
            {
                if (!coroutine.MoveNext())
                {
                    // this coroutine has finished.
                    continue;
                }

                if (coroutine.Current == null)
                {
                    // the coroutine yielded null, so run it next frame.
                    _nextCoroutines.Add(coroutine);
                    continue;
                }
            }
            _currentCoroutines = _nextCoroutines.ToList();
            _nextCoroutines.Clear();
            _currentCoroutines.AddRange(_nextToAddCoroutines);
            _nextToAddCoroutines.Clear();
        }
    }
}
