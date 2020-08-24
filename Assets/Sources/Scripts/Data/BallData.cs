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

        public List<uint> GetCriteriaRangeToSpawn(int amount) =>
            matchableDataList.GetRange(0, amount).Select(t => t.Criteria).ToList();

        public uint GetMaxCriteriaAvailable() => matchableDataList.Max(t => t.Criteria);
    }
}