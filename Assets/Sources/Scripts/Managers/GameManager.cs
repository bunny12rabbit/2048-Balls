using System;
using System.Collections.Generic;
using Data;
using Generics;
using UI;
using UnityEngine;

namespace Managers
{
    public class GameManager : SingletonBehaviourGeneric<GameManager>
    {
        [SerializeField] private DataBase dataBase;

        [SerializeField] private int rangeToSpawnCount = 4;
        
        [SerializeField] private bool isCustomWinCriteria;

        [DrawIf(nameof(isCustomWinCriteria), true, DrawIfAttribute.DisablingTypes.ReadOnly)]
        [SerializeField] private uint winCriteria;


        public DataBase DataBase => dataBase;

        public int RangeToSpawnCount => rangeToSpawnCount;
        
        public bool IsWin { get; private set; }

        public uint WinCriteria => winCriteria;

        public List<uint> GetCriteriaRangeToSpawn() => DataBase.BallData.GetCriteriaRangeToSpawn(RangeToSpawnCount);

        public void WinGame()
        {
            IsWin = true;
            
            AudioManager.Instance.PlayFxSound(AudioFxTypes.Win);
            GameFieldManager.Instance.OpenGates();
            WindowManager.Instance.ShowWindow(typeof(VictoryWindow));
        }

        protected override void Awake()
        {
            base.Awake();

            QualitySettings.vSyncCount = 1;
        }

        private void OnValidate()
        {
            if (isCustomWinCriteria)
            {
                return;
            }

            winCriteria = (uint)Mathf.Clamp(winCriteria, DataBase.BallData.GetData(1).Criteria,
                DataBase.BallData.GetMaxCriteriaAvailable());
        }
    }
}