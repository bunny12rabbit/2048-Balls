using System;
using Interfaces;
using Managers;
using StaticTools;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct BallMatchableData : IMatchableData<uint>
    {
        [SerializeField] private uint criteria;
        [SerializeField] private Color color;
        [SerializeField] private Vector2 localScale;

        public BallMatchableData(uint number)
        {
            int index = StaticUtilities.GetPowerOfTwo(number) - 1;
            var data = GameManager.Instance.DataBase.BallData.GetData(index);

            criteria = data.Criteria;
            color = data.Color;
            localScale = data.LocalScale;
            IsMatched = false;
        }

        public uint Criteria
        {
            get => criteria;
            private set => criteria = value;
        }

        public bool IsMatched { get; set; }


        public Color Color
        {
            get => color;
            private set => color = value;
        }

        public Vector2 LocalScale
        {
            get => localScale;
            private set => localScale = value;
        }

        public void UpdateData()
        {
            int index = StaticUtilities.GetPowerOfTwo(Criteria == default ? 2 : Criteria);
            var data = GameManager.Instance.DataBase.BallData.GetData(index);

            Criteria = data.Criteria;
            Color = data.Color;
            LocalScale = data.LocalScale;
        }
    }
}