using Generics;
using UnityEngine;

namespace Managers
{
    public class GameFieldManager : MonoBehaviour
    {
        private static GameFieldManager _instance;

        [SerializeField] private Rigidbody2D gatesLeftHinge;
        [SerializeField] private Rigidbody2D gatesRightHinge;

        [SerializeField] private Transform ballLockSpawnPoint;

        private MatchableItemGeneric<uint> _ballLock;


        public static void OpenGates()
        {
            if (_instance.gatesLeftHinge == null || _instance.gatesRightHinge == null)
            {
                string nullFieldName =
                    _instance.gatesLeftHinge == null ? nameof(gatesLeftHinge) : nameof(gatesRightHinge);
                Debug.LogError($"::{nameof(GameFieldManager)}:: {nullFieldName} reference is null. Assign all fields!");
                return;
            }

            _instance._ballLock.SpawnWinVfx();
            _instance._ballLock.gameObject.PushBackToPool();

            _instance.gatesLeftHinge.isKinematic = false;
            _instance.gatesRightHinge.isKinematic = false;
        }

        private void Awake() => InitializeInstance();

        private void Start() => _ballLock = ItemSpawner.Instance.SpawnBallLock(ballLockSpawnPoint.position);

        private void InitializeInstance()
        {
            if (_instance)
            {
                Destroy(this);
                return;
            }

            _instance = this;
        }
    }
}