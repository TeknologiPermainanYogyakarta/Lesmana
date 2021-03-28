using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// Singleton should not be destroyed. if it is destroyed, it's because the application is shutting down!
    /// </summary>
    private static bool _isApplicationShuttingDown = false;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            //            if(_isApplicationShuttingDown == true)
            //            {
            //#if UNITY_EDITOR
            //                Debug.LogWarning($"Singleton {typeof(T).Name} is already Destroyed. This means the Application is Shutting Down!");
            //#endif
            //                return null;
            //            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;

                    if (_instance == null)
                    {
                        T instanceObj = Resources.Load<T>($"Managers/{typeof(T).Name}");
                        if (instanceObj != null && !_isApplicationShuttingDown)
                        {
                            GameObject objInstantiated = Instantiate(instanceObj.gameObject);
                            _instance = objInstantiated.GetComponent<T>();
                        }
                    }

                    if (_instance == null && !_isApplicationShuttingDown)
                    {
                        GameObject singletonObj = new GameObject();
                        _instance = singletonObj.AddComponent<T>();
                        singletonObj.name = typeof(T).ToString() + " (Singleton)";
                    }
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        _isApplicationShuttingDown = true;
    }

    public virtual void Initialization()
    {

    }
}