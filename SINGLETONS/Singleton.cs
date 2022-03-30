using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Singelton tutorials and code used:
 * https://blog.mzikmund.com/2019/01/a-modern-singleton-in-unity/
 * https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
 * 
*/

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    protected bool persistent = true;

    protected static bool threadSafe = true;

    private static readonly object _lock = new object();

    private static T _current;
    public static T Current
    {
        get
        {
            if (threadSafe)
            {
                lock (_lock)
                {
                    return GetSingleton();
                }
            }

            return GetSingleton();
        }
    }

    //TODO: make a more robust solution that returns this version when
    //changing scenes or closing player? Or different calls to things when
    //this is happening vs being normally destroyed?

    /// <summary>
    /// Used to get the current when calling the singleton from any OnDestroy
    /// in case the singleton was already destroyed
    /// which only happens when stopping the game or changing scenes
    /// </summary>
    public static T OnDestroyCurrent
    {
        get
        {
            if (threadSafe)
            {
                lock (_lock)
                {
                    return _current;
                }
            }

            return _current;
        }
    }

    private static T GetSingleton()
    {
        if (_current == null)
        {
            _current = CreateSingleton();
        }
        return _current;
    }

    private static T CreateSingleton()
    {
        //First find all singletons that may be in the scene
        T[] instances = FindObjectsOfType<T>();
        int count = instances.Length;
        if (count > 0)
        {
            //delete others that shouldn't be there
            for (int i = 1; i < count; i++)
            {
                DeleteSingletonDuplicate(instances[i]);
            }
            return instances[0];
        }

        //If none in the scene, make one now
        Debug.Log($"No Monobehavior Singleton found for type: {typeof(T).Name} in the scene! Creating one...");
        var ownerObject = new GameObject($"{typeof(T).Name} (runtime singleton)");
        var instance = ownerObject.AddComponent<T>();
        return instance;
    }

    private static void DeleteSingletonDuplicate(T instance)
    {
        Debug.LogWarning($"[{nameof(T)}] There should not be more than one singleton of each type in the scene." +
                    $"Deleting duplicate singleton of type {typeof(T).Name}: {instance.name}");
        Destroy(instance.gameObject);
    }

    protected void Awake()
    {
        lock (_lock)
        {
            //In case we reach here before awake of other objects that reference it during awake,
            //creates singleton now instead of later,
            //makes sure we delete duplicates
            GetSingleton();

            //only one will get here because duplicates will be deleted in GetSingleton
            if (persistent)
            {
                DontDestroyOnLoad(gameObject);
            }


            OnAwake();
        }

    }

    protected virtual void OnAwake() { }
}




/*
 * Pervious singleton:
 * 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static public T Current = null;

    abstract protected T GetSelf();

    private void Awake()
    {
        if (Current == null)
        {
            Current = GetSelf();
        }
        else if (Current != GetSelf())
        {
            Destroy(gameObject);
        }
    }
}

*/