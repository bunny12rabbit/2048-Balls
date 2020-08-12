using Data;
using Generics;
using InputSystems;
using Managers;
using ObjectPools;
using Pixelplacement;
using TMPro;
using UnityEngine;

namespace Logic
{
    public class Ball : MatchableItemItemGeneric<uint>
    {
        private const float HEIGHT_DIFFERENCE = 0.1f;

        #region Variables

        [SerializeField] private TMP_Text label;
        [SerializeField] private CollisionHandlerNumeric collisionHandler;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private float moveTowardsDuration = 0.5f;
        [SerializeField] private string vfxPath = "Prefabs/CFX2_RockHit";
        [SerializeField] private string winVfxPath = "Prefabs/CFX_MagicPoof";

        private Ball _matchedBall;

        private string _initialName;
        private Color _previousColor;

        #endregion

        #region Properties

        private BallMatchableData BallData => (BallMatchableData)Data;
        private Transform SpriteRendererTransform { get; set; }


        public override CollisionHandlerGeneric<uint> CollisionHandler
        {
            get => collisionHandler;
            protected set => collisionHandler = (CollisionHandlerNumeric)value;
        }

        #endregion

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
            SwitchPhysics(true);
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
            if (IsInvalidMatch())
            {
                return;
            }

            _matchedBall = (Ball)matchedBall;

            _matchedBall.Data.IsMatched = true;
            _matchedBall.SwitchPhysics(false);

            SwitchPhysics(false);
            Data.IsMatched = true;

            DebugWrapper.Log(
                $"<color=green>{name}.IsMatched={Data.IsMatched}</color> collides with " +
                $"<color=blue>{_matchedBall.gameObject.name}.IsMatched={_matchedBall.Data.IsMatched}</color>");

            MoveTowardsOther(_matchedBall);

            bool IsInvalidMatch()
            {
                var position = transform.position;
                var matchedBallPosition = matchedBall.transform.position;

                return GameManager.Instance.IsWin || matchedBall.Data.IsMatched ||
                       _matchedBall != null && !ReferenceEquals(matchedBall, _matchedBall) ||
                       Mathf.Abs(position.y - matchedBallPosition.y) > HEIGHT_DIFFERENCE &&
                       position.y - matchedBallPosition.y < HEIGHT_DIFFERENCE;
            }
        }

        private void MoveTowardsOther(IMatchableItem<uint> matchedBall)
        {
            Tween.Position(transform, matchedBall.transform.position, moveTowardsDuration, 0, Tween.EaseInOutStrong,
                Tween.LoopType.None, null, OnMoveTweenAnimationComplete);
        }

        private void OnMoveTweenAnimationComplete()
        {
            UpdateData();

            bool isWinCondition = IsWinCondition();

            if (isWinCondition)
            {
                GameManager.Instance.WinGame();
            }
            else
            {
                AudioManager.Instance.PlayFxSound(AudioFxTypes.Collapse);
            }

            SpawnVfx();

            ItemSpawner.Instance.UpdateMaxSpawnedCriteria(Data.Criteria);
            PushMatchedBallBackToPool();
            ResetBallState();
            SwitchPhysics(true);
        }

        private void PushMatchedBallBackToPool()
        {
            _matchedBall.gameObject.name = _initialName;
            _matchedBall.gameObject.PushBackToPool();
        }

        private void SpawnVfx()
        {
            var vfx = GameObjectPool.GetObjectFromPool(GameManager.Instance.IsWin ? winVfxPath : vfxPath,
                transform.position, Quaternion.identity);

            var particleSystemManager = vfx.GetComponent<ParticleSystemManager>();

            if (particleSystemManager)
            {
                particleSystemManager.ChangeColor(_previousColor);
            }
        }

        private void SwitchPhysics(bool state) => CollisionHandler.SwitchPhysics(state);

        private bool IsWinCondition() =>
            Data?.Criteria >= GameManager.Instance.DataBase.BallData.GetMaxCriteriaAvailable();

        private void ResetBallState()
        {
            Data.IsMatched = false;
            _matchedBall = null;
        }

        public override void UpdateData(uint criteria = default)
        {
            if (criteria == default)
            {
                if (Data == null)
                {
                    return;
                }

                _previousColor = BallData.Color;
                Data.UpdateData();
            }
            else
            {
                Data = new BallMatchableData(criteria);
                _initialName = name;
            }

            label.text = Data.Criteria.ToString();
            spriteRenderer.color = BallData.Color;
            SpriteRendererTransform.localScale = BallData.LocalScale;
            name = _initialName + Data.Criteria;
        }
    }
}