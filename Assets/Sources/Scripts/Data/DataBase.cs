using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct DataBase
    {
        public BallData BallData => ballData;


        [SerializeField] private BallData ballData;
    }
}