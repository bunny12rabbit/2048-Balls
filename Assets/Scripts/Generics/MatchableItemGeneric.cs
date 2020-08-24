using Interfaces;
using Managers;
using ObjectPools;
using TMPro;
using UnityEngine;

namespace Generics
{
    public abstract class MatchableItemGeneric<T> : MonoBehaviour, IMatchableItem<T> where T : struct
    {
        [SerializeField] protected string matchVfxPath = "Prefabs/CFX2_RockHit";
        [SerializeField] protected string winVfxPath = "Prefabs/CFX_MagicPoof";
        
        [SerializeField] protected TMP_Text label;
        
        [SerializeField] protected SpriteRenderer spriteRenderer;
        
        protected Color previousColor;

        
        private Transform _spriteRendererTransform;
        
        
        public new Transform transform { get; private set; }

        //TODO: Remove from here
        public abstract CollisionHandlerGeneric<T> CollisionHandler { get; protected set; }

        public virtual IMatchableData<T> Data { get; protected set; }


        protected virtual void Awake()
        {
            transform = GetComponent<Transform>();
            InitializeComponents();
        }

        protected void SpawnMatchVfx()
        {
            var vfx = SpawnVfx(matchVfxPath);
            
            var particleSystemManager = vfx.GetComponent<ParticleSystemManager>();

            if (particleSystemManager)
            {
                particleSystemManager.ChangeColor(previousColor);
            }
        }

        public void SpawnWinVfx() => SpawnVfx(winVfxPath);

        public void OnCollisionDetected(IMatchableItem<T> other)
        {
            if (GameManager.Instance.IsWin)
            {
                return;
            }
            
            if (CheckMatch(other))
            {
                OnMatch(other);
            }
        }

        protected void InitializeComponents()
        {
            CollisionHandler.DoActionWithCheckReference(() => CollisionHandler.Initialize(this));
            
            spriteRenderer.DoActionWithCheckReference(() =>
                _spriteRendererTransform = spriteRenderer.GetComponent<Transform>());
        }

        public bool CheckMatch(IMatchableItem<T> matchableItem) => Equals((MatchableItemGeneric<T>)matchableItem);

        public abstract void OnMatch(IMatchableItem<T> matchedObject);
        public abstract void UpdateData(T criteria = default);

        public void UpdateLocalScale(Vector2 scale) => _spriteRendererTransform.localScale = scale;

        private GameObject SpawnVfx(string vfxPath) =>
            GameObjectPool.GetObjectFromPool(vfxPath, transform.position, Quaternion.identity);
        
        #region Equals operators override

        public override bool Equals(object other) => this == (MatchableItemGeneric<T>)other;

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(MatchableItemGeneric<T> item1, MatchableItemGeneric<T> item2)
        {
            if (item1 is null || item2 is null)
            {
                return false;
            }

            return item1.Data.Criteria.Equals(item2.Data.Criteria);
        }

        public static bool operator !=(MatchableItemGeneric<T> ball1, MatchableItemGeneric<T> ball2)
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