using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Network
{
    public class Dispatcher
    {
        public List<Action> pending = new List<Action>();

        private static Dispatcher instance;

        public static Dispatcher Instance
        {
            get
            {
                if (instance == null)
                    instance = new Dispatcher();
                return instance;
            }
        }
        public Dispatcher()
        {

        }

        public void Invoke(Action fn)
        {
            lock (pending)
            {
                pending.Add(fn);
            }
        }

        public void InvokePending()
        {
            lock (pending)
            {
                foreach (var action in pending)
                {
                    action();
                }

                pending.Clear();
            }
        }

    }
}
