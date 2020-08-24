using Controllers;
using Generics;
using UnityEngine;

namespace Managers
{
    public class GameFieldManager : SingletonBehaviourGeneric<GameFieldManager>
    {
        [SerializeField] private Rigidbody2D gatesLeftHinge;
        [SerializeField] private Rigidbody2D gatesRightHinge;
        [SerializeField] private Rigidbody2D lockRigidbody;

        [SerializeField] private Transform ballLockSpawnPoint;

        private MatchableItemGeneric<uint> _ballLock;


        public void OpenGates()
        {
            if (gatesLeftHinge == null || gatesRightHinge == null)
            {
                string nullFieldName =
                    gatesLeftHinge == null ? nameof(gatesLeftHinge) : nameof(gatesRightHinge);
                Debug.LogError($"::{nameof(GameFieldManager)}:: {nullFieldName} reference is null. Assign all fields!");
                return;
            }

            _ballLock.SpawnWinVfx();
            _ballLock.gameObject.PushBackToPool();

            gatesLeftHinge.isKinematic = false;
            gatesRightHinge.isKinematic = false;
            lockRigidbody.isKinematic = false;
        }

        private void Start() => _ballLock = ItemSpawner.Instance.SpawnBallLock(ballLockSpawnPoint.position);

    }
}