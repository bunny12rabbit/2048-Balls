using Interfaces;
using UnityEngine;

namespace Generics
{
    public abstract class MatchableItemItemGeneric<T> : MonoBehaviour, IMatchableItem<T> where T : struct
    {
        public new Transform transform { get; private set; }
        
        public abstract CollisionHandlerGeneric<T> CollisionHandler { get; protected set; }
        
        public virtual IMatchableData<T> Data { get; protected set; }

        
        protected virtual void Awake()
        {
            transform = GetComponent<Transform>();
            InitializeComponents();
        }

        public void OnCollisionDetected(IMatchableItem<T> other)
        {
            if (CheckMatch(other))
            {
                OnMatch(other);
            }
        }

        public abstract void InitializeComponents();

        public bool CheckMatch(IMatchableItem<T> matchableItem) => ((MatchableItemItemGeneric<T>)matchableItem).Equals(this);

        public abstract void OnMatch(IMatchableItem<T> matchedObject);
        public abstract void UpdateData(T criteria = default);

        #region Equals operators override

        public override bool Equals(object other) => (MatchableItemItemGeneric<T>)other == this;

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(MatchableItemItemGeneric<T> item1, MatchableItemItemGeneric<T> item2)
        {
            if (item1 is null || item2 is null)
            {
                return false;
            }

            return item1.Data.Criteria.Equals(item2.Data.Criteria);
        }

        public static bool operator !=(MatchableItemItemGeneric<T> ball1, MatchableItemItemGeneric<T> ball2)
        {
            if (ball1 is null || ball2 is null)
            {
                return false;
            }

            return ball1.Data.Criteria.Equals(ball2.Data.Criteria) == false;
        }

        #endregion
    }
}