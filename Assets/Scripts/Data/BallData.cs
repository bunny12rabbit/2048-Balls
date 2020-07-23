using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "BallDataCollection", menuName = "Ball Data Collection")]
    public class BallData : ScriptableObject
    {
        [SerializeField] private List<BallMatchableData> matchableDataList = new List<BallMatchableData>();

        public BallMatchableData GetData(int index)
        {
            index = Mathf.Clamp(index, 0, matchableDataList.Count);

            return matchableDataList[index];
        }
    }
}