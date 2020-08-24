using UnityEngine;

namespace Generics
{
    public class SingletonBehaviourPersistentGeneric<T> : MonoBehaviour where T : SingletonBehaviourPersistentGeneric<T>
    {
        public static T Instance { get; private set; }
        
        protected virtual void Awake()
        {
            InitializeInstance();
        }

        private void InitializeInstance()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
    }
}