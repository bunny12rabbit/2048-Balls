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

        public List<uint> GetCriteriaRangeToSpawn() => DataBase.BallData.GetCriteriaRangeToSpawn(RangeToSpawnCount);
    }
}