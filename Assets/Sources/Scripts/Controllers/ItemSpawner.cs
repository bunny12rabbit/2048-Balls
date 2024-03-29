﻿using System.Collections;
using System.Collections.Generic;
using Factories;
using Generics;
using InputSystems;
using Logic;
using Managers;
using UnityEngine;

namespace Controllers
{
    public class ItemSpawner : SingletonBehaviourGeneric<ItemSpawner>
    {
        private const float SPAWN_DELAY = 1f;


        [SerializeField] private string itemPath = "Prefabs/Ball";

        [SerializeField] private Transform spawnPoint;


        private readonly MatchableItemFactoryGeneric<uint> _factory = new MatchableItemFactoryGeneric<uint>();

        private List<uint> _criteriaRangeToSpawn = new List<uint>();

        private bool _isSpawnRunning;
        private bool _isFirstSpawn = true;
        
        private uint _maxSpawnedCriteria;


        public void UpdateMaxSpawnedCriteria(uint criteria)
        {
            bool isNewMax = criteria > _maxSpawnedCriteria &&
                            criteria <= _criteriaRangeToSpawn[GameManager.Instance.RangeToSpawnCount - 1];

            if (isNewMax)
                _maxSpawnedCriteria = criteria;
        }

        public MatchableItemGeneric<uint> SpawnBallLock(Vector3 position)
        {
            uint criteria = GameManager.Instance.DataBase.BallData.GetMaxCriteriaAvailable();
            var ballLock = (MatchableItemGeneric<uint>)_factory.ConstructItem(itemPath, position, criteria);
            ballLock.CollisionHandler.SwitchPhysics(false);

            int maxAvailableCriteriaIndex = GameManager.Instance.RangeToSpawnCount - 1;
            var scale = GameManager.Instance.DataBase.BallData.GetData(maxAvailableCriteriaIndex).LocalScale;
            ballLock.UpdateLocalScale(scale);

            (ballLock as Ball)?.SetLineRendererActive(false);

            return ballLock;
        }

        private void Start()
        {
            InputListener.OnRelease += OnNeedToSpawnNewItem;

            _criteriaRangeToSpawn = GameManager.Instance.GetCriteriaRangeToSpawn();
            SpawnNewItem();

            _isFirstSpawn = false;
        }

        private void OnNeedToSpawnNewItem()
        {
            if (GameManager.Instance.IsWin)
                return;

            SpawnNewItemWithDelay(SPAWN_DELAY);
        }

        private void SpawnNewItemWithDelay(float delay)
        {
            if (_isSpawnRunning)
                return;

            StartCoroutine(SpawnWithDelayCoroutine(delay));
        }

        private IEnumerator SpawnWithDelayCoroutine(float delay)
        {
            _isSpawnRunning = true;
            yield return Yielders.WaitForSeconds(delay);

            SpawnNewItem();

            _isSpawnRunning = false;
        }

        private void SpawnNewItem()
        {
            uint randomItemCriteria = GetRandomItemCriteria();
            var item = _factory.ConstructItem(itemPath, spawnPoint.position, randomItemCriteria);

            InputController.Instance.UpdateTarget((Ball)item);
        }

        private uint GetRandomItemCriteria()
        {
            int maxIndex = _isFirstSpawn ? 1 : _criteriaRangeToSpawn.IndexOf(_maxSpawnedCriteria) + 1;
            return _criteriaRangeToSpawn[Random.Range(0, maxIndex)];
        }

        private void OnDestroy()
        {
            InputListener.OnRelease -= OnNeedToSpawnNewItem;
        }
    }
}