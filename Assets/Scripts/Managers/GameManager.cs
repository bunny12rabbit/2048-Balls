using System.Collections.Generic;
using Data;
using Generics;
using UnityEngine;

namespace Managers
{
    public class GameManager : SingletonBehaviourGeneric<GameManager>
    {
        [SerializeField] private DataBase dataBase;

        [SerializeField] private uint rangeToSpawnCount = 4;
        
        
        public DataBase DataBase => dataBase;

        public List<uint> GetCriteriaRangeToSpawn() => DataBase.BallData.GetCriteriaRangeToSpawn(rangeToSpawnCount);
    }
}