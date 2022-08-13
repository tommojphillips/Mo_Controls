using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSCLoader;
using UnityEngine;

namespace TommoJProductions.MoControls
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Written, 09.08.2022

        public static T instance { get; private set; }

        public SingletonMonoBehaviour() 
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                string message = $"cannot create more than one instance of {nameof(T)}";
                ModConsole.Error(message);
                throw new InvalidOperationException(message);
            }
        }
    }
}
