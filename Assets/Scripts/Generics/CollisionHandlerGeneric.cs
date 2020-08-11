using InputSystems;
using UnityEngine;
using static Constants;

namespace Generics
{
    public abstract class CollisionHandlerGeneric<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private TagsNames.Tags comparingTag;

        [SerializeField] private float gravityScale = 1.7f;

        [SerializeField] private Rigidbody2D myRigidbody;

        [SerializeField] private LayerMask layerMask;


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

        public void MoveToPosition(Vector3 initialRigidbodyPosition, Vector3 newPositionOffset)
        {
            var position = myRigidbody.transform.position;
            myRigidbody.velocity = new Vector2(initialRigidbodyPosition.x + newPositionOffset.x - position.x, 0) *
                                   (InputController.Sensitivity * Time.fixedDeltaTime);
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            ProcessCollision(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            ProcessCollision(other);
        }

        private void ProcessCollision(Collision2D other)
        {
            if (!TryGetIMatchable(other, out var otherMatchable))
            {
                return;
            }

            if (otherMatchable != null)
            {
                _matchable.OnCollisionDetected(otherMatchable);
            }
        }

        private bool TryGetIMatchable(Collision2D other, out IMatchableItem<T> otherMatchable)
        {
            if (!other.gameObject.CompareTag(TagsNames.GetTag(comparingTag)))
            {
                otherMatchable = null;
                return false;
            }

            otherMatchable = other.transform.GetComponent<IMatchableItem<T>>();
            return true;
        }
    }
}