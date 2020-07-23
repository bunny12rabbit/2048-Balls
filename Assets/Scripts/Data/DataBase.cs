using Generics;
using UnityEngine;

namespace Data
{
    public class DataBase : SingletonBehaviourGeneric<DataBase>
    {
        public BallData BallData => ballData;


        [SerializeField] private BallData ballData;
    }
}