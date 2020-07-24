using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "BallDataCollection", menuName = "Ball Data Collection")]
    public class BallData : ScriptableObject
    {
        [SerializeField] private List<BallMatchableData> matchableDataList = new List<BallMatchableData>();

        public BallMatchableData GetData(int index)
        {
            index = Mathf.Clamp(index, 0, matchableDataList.Count - 1);

            return matchableDataList[index];
        }

        public List<uint> GetCriteriaRangeToSpawn(uint amount) =>
            matchableDataList.GetRange(0, (int)amount).Select(t => t.Criteria).ToList();
    }
}