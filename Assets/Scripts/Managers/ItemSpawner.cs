using System.Collections;
using System.Collections.Generic;
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


        private readonly MatchableItemFactoryGeneric<uint> _factory = new MatchableItemFactoryGeneric<uint>();

        private List<uint> _criteriaRangeToSpawn = new List<uint>();

        private bool _isSpawnRunning;


        private void Start()
        {
            InputListener.OnRelease += OnNeedToSpawnNewItem;

            _criteriaRangeToSpawn = GameManager.Instance.GetCriteriaRangeToSpawn();
            SpawnNewItem();
        }

        private void OnNeedToSpawnNewItem()
        {
            SpawnNewItemWithDelay(SPAWN_DELAY);
        }

        private void SpawnNewItem()
        {
            uint randomItemCriteria = GetRandomItemCriteria();
            var item = _factory.ConstructItem(itemPath, spawnPoint.position, randomItemCriteria);
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

        private uint GetRandomItemCriteria() => _criteriaRangeToSpawn[Random.Range(0, _criteriaRangeToSpawn.Count)];
    }
}