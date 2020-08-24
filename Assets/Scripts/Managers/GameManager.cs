using System.Collections.Generic;
using Data;
using Generics;
using UnityEngine;

namespace Managers
{
    public class GameManager : SingletonBehaviourGeneric<GameManager>
    {
        [SerializeField] private DataBase dataBase;

        [SerializeField] private int rangeToSpawnCount = 4;


        public DataBase DataBase => dataBase;

        public int RangeToSpawnCount => rangeToSpawnCount;
        
        public bool IsWin { get; private set; }

        public List<uint> GetCriteriaRangeToSpawn() => DataBase.BallData.GetCriteriaRangeToSpawn(RangeToSpawnCount);

        public void WinGame()
        {
            IsWin = true;
            
            AudioManager.Instance.PlayFxSound(AudioFxTypes.Win);
            GameFieldManager.Instance.OpenGates();
        }

        protected override void Awake()
        {
            base.Awake();

            QualitySettings.vSyncCount = 1;
        }
    }
}