using UnityEngine;

namespace Generics
{
    public abstract class CollisionHandlerGeneric<T> : MonoBehaviour where T : struct
    {
        private const float FORCE_AMOUNT = 500f;
        
        
        [SerializeField] private TagsManager.Tags comparingTag;

        [SerializeField] private float gravityScale = 1.7f;

        [SerializeField] private Collider2D myCollider;

        [SerializeField] private Rigidbody2D myRigidbody;


        private MatchableItemItemGeneric<T> _matchable;

        public void Initialize(MatchableItemItemGeneric<T> matchable)
        {
            _matchable = matchable;
        }

        public void SwitchPhysics(bool state)
        {
            myRigidbody.isKinematic = !state;
            myRigidbody.gravityScale = !state ? 0 : gravityScale;
            myRigidbody.velocity = Vector2.zero;
        }

        public void SwitchPhysicsSimulation(bool state) => myRigidbody.gravityScale = !state ? 0 : gravityScale;

        protected void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag(TagsManager.GetTag(comparingTag)))
            {
                return;
            }
        
            var otherMatchable = other.transform.GetComponent<IMatchableItem<T>>();

            if (otherMatchable != null)
            {
                _matchable.OnCollisionDetected(otherMatchable);
            }
        }

        public void MoveToPosition(Vector3 initialRigidbodyPosition, Vector3 newPositionOffset)
        {
            var position = myRigidbody.transform.position;
            myRigidbody.velocity = new Vector2(initialRigidbodyPosition.x + newPositionOffset.x - position.x, 0) *
                                   (FORCE_AMOUNT * Time.fixedDeltaTime);
        }
    }
}