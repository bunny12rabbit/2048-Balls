using System.Collections;
using Factories;
using Generics;
using InputSystems;
using Logic;
using UnityEngine;

namespace Managers
{
    public class ItemSpawner : SingletonBehaviourGeneric<ItemSpawner>
    {
        private const float SPAWN_DELAY = 1f;


        [SerializeField] private Transform spawnPoint;

        [Space] [SerializeField] private string itemPath = "Prefabs/Ball";


        private MatchableItemFactoryGeneric<uint> _factory = new MatchableItemFactoryGeneric<uint>();

        private bool _isSpawnRunning;


        private void Start()
        {
            InputListener.OnRelease += OnNeedToSpawnNewItem;
            SpawnNewItem();
        }

        private void OnNeedToSpawnNewItem()
        {
            SpawnNewItemWithDelay(SPAWN_DELAY);
        }

        private void SpawnNewItem()
        {
            var item = _factory.ConstructItem(itemPath, spawnPoint.position);
            InputController.Instance.UpdateTarget((Ball)item);
        }

        private void SpawnNewItemWithDelay(float delay)
        {
            if (_isSpawnRunning)
            {
                return;
            }

            StartCoroutine(SpawnWithDelayCoroutine(delay));
        }

        private IEnumerator SpawnWithDelayCoroutine(float delay)
        {
            _isSpawnRunning = true;
            yield return Yielders.WaitForSeconds(delay);

            SpawnNewItem();

            _isSpawnRunning = false;
        }
    }
}