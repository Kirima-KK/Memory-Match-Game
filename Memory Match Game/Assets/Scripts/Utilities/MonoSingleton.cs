using UnityEngine;

namespace MemoryMatch.Utilities
{
    public abstract class MonoSingleton : MonoBehaviour
    {
        public abstract void Destroy();
    }

    public class MonoSingleton<T> : MonoSingleton where T : MonoBehaviour
    {
        private static bool isDestroyed = false;
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if(isDestroyed)
                {
                    return null;
                }

                if(instance == null)
                {
                    instance = FindAnyObjectByType<T>();

                    if(FindObjectsByType(typeof(T)).Length > 1)
                    {
                        print("has duplicated singleton");
                        return instance;
                    }

                    if(instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = instance.GetType().Name;
                    }
                }

                return instance;
            }

            private set { instance = value; }
        }

        public static bool HasInstance { get { return instance != null; } }

        public virtual void Awake()
        {
            if(instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
            isDestroyed = false;
        }

        public virtual void OnDestroy()
        {
            if(instance == this)
            {
                isDestroyed = true;
            }
        }

        public override void Destroy()
        {
            instance = null;
            Destroy(gameObject);
        }
    }
}