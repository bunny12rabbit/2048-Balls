using Data;
using Generics;
using InputSystems;
using ObjectPools;
using Pixelplacement;
using TMPro;
using UnityEngine;

namespace Logic
{
    public class Ball : MatchableItemItemGeneric<uint>
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CollisionHandlerNumeric collisionHandler;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private float moveTowardsDuration = 0.5f;
        [SerializeField] private string vfxPath = "Prefabs/CFX2_RockHit";

        private Ball _matchedBall;
        
        private bool _isInTransition;
        
        private string _initialName;


        public BallMatchableData BallData => (BallMatchableData)Data;
        private Transform SpriteRendererTransform { get; set; }


        public override CollisionHandlerGeneric<uint> CollisionHandler
        {
            get => collisionHandler;
            protected set => collisionHandler = (CollisionHandlerNumeric)value;
        }

        public override void OnMatch(IMatchableItem<uint> matchedObject)
        {
            MatchBalls(matchedObject);
        }


        public override void InitializeComponents()
        {
            if (CollisionHandler)
            {
                CollisionHandler.Initialize(this);
            }
            else
            {
                DebugWrapper.LogError($"{nameof(CollisionHandler)} is not found! Assign the reference!",
                    DebugColors.Red);
            }

            SpriteRendererTransform = spriteRenderer.GetComponent<Transform>();
        }

        private void OnEnable()
        {
            InputListener.OnRelease += OnRelease;
            UpdateData();
        }

        private void OnDisable()
        {
            InputListener.OnRelease -= OnRelease;
            SetDefaultState();
        }

        private void SetDefaultState()
        {
            _isInTransition = false;
            _matchedBall = null;
            Data = null;

            lineRenderer.enabled = true;
        }

        private void OnRelease()
        {
            lineRenderer.enabled = false;
        }

        private void MatchBalls(IMatchableItem<uint> matchedBall)
        {
            if (matchedBall.Data.IsMatched)
            {
                return;
            }

            Data.IsMatched = true;

            SwitchPhysics(false);
            _matchedBall = (Ball)matchedBall;
            _matchedBall.SwitchPhysics(false);

            DebugWrapper.LogError(
                $"{name}.IsMatched={Data.IsMatched} collides with {_matchedBall.gameObject.name}.IsMatched={_matchedBall.Data.IsMatched}",
                DebugColors.Red);
            MoveTowardsOther(_matchedBall);
        }

        private void MoveTowardsOther(IMatchableItem<uint> matchedBall)
        {
            Tween.Position(transform, matchedBall.transform.position, moveTowardsDuration, 0, Tween.EaseInOutStrong,
                Tween.LoopType.None, null, OnMoveTweenAnimationComplete);
        }

        private void OnMoveTweenAnimationComplete()
        {
            GameObjectPool.GetObjectFromPool(vfxPath, transform.position, Quaternion.identity);
            _matchedBall.gameObject.PushBackToPool();
            StartBallTransition();
        }

        private void SwitchPhysics(bool state) => CollisionHandler.SwitchPhysics(state);

        private void StartBallTransition()
        {
            _isInTransition = true;
            UpdateData();
            
            Tween.LocalScale(SpriteRendererTransform, BallData.LocalScale, 0.2f, 0,
                Tween.EaseBounce);
            Tween.Color(spriteRenderer, BallData.Color, 0.2f, 0, Tween.EaseInOutStrong, Tween.LoopType.None, null,
                OnTransitionTweenComplete);
        }

        private void OnTransitionTweenComplete()
        {
            _isInTransition = false;
            Data.IsMatched = false;
            _matchedBall = null;
            SwitchPhysics(true);
        }

        public override void UpdateData(uint criteria = default)
        {
            if (criteria == default)
            {
                if (Data == null)
                {
                    return;
                }

                Data.UpdateData();
            }
            else
            {
                Data = new BallMatchableData(criteria);
                _initialName = name;
            }

            label.text = Data.Criteria.ToString();

            if (!_isInTransition)
            {
                spriteRenderer.color = BallData.Color;
                SpriteRendererTransform.localScale = BallData.LocalScale;
            }

            name = _initialName + Data.Criteria;
        }
    }
}