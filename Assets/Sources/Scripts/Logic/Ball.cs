using Controllers;
using Data;
using Generics;
using InputSystems;
using Managers;
using Pixelplacement;
using UnityEngine;

namespace Logic
{
    public class Ball : MatchableItemGeneric<uint>
    {
        private const float HEIGHT_DIFFERENCE = 0.1f;
        private const string MatchedBallColorString = "#0087FF";
        private const string SameInstanceIdColorString = "red";

        #region Variables

        [SerializeField] private CollisionHandlerNumeric collisionHandler;
        [SerializeField] private LineRenderer lineRenderer;

        [SerializeField] private float moveTowardsDuration = 0.5f;

        private Ball _matchedBall;

        private string _initialName;

        #endregion

        private BallMatchableData BallData => (BallMatchableData)Data;

        public override CollisionHandlerGeneric<uint> CollisionHandler
        {
            get => collisionHandler;
            protected set => collisionHandler = (CollisionHandlerNumeric)value;
        }

        public override void OnMatch(IMatchableItem<uint> matchedObject)
        {
            MatchBalls(matchedObject);
        }

        public void SetLineRendererActive(bool state)
        {
            lineRenderer.enabled = state;
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
            SwitchCollisionDetection(this, true);
            _matchedBall = null;
            Data = null;

            SetLineRendererActive(true);
        }

        private void OnRelease()
        {
            SetLineRendererActive(false);
        }

        private void MatchBalls(IMatchableItem<uint> matchedBall)
        {
            if (IsInvalidMatch())
                return;

            _matchedBall = (Ball)matchedBall;

            SwitchCollisionDetection(_matchedBall, false);
            SwitchCollisionDetection(this, false);

            var thisInstanceId = GetInstanceID();
            var matchedBallInstanceId = _matchedBall.GetInstanceID();
            var isSameInstanceId = thisInstanceId == matchedBallInstanceId;
            var matchedBallInstanceIdColorString = isSameInstanceId ? SameInstanceIdColorString : "white";

            DebugWrapper.Log(
                $"<color=green>{name}.IsMatched={Data.IsMatched}</color> id={thisInstanceId} collides with " +
                $"<color={MatchedBallColorString}>{_matchedBall.gameObject.name}.IsMatched={_matchedBall.Data.IsMatched}</color>" +
                $"<color={matchedBallInstanceIdColorString}> id={matchedBallInstanceId}</color>");

            _matchedBall.Data.IsMatched = true;
            Data.IsMatched = true;
            
            MoveTowardsOther(_matchedBall);

            bool IsInvalidMatch()
            {
                var position = transform.position;
                var matchedBallPosition = matchedBall.transform.position;

                bool isIncorrectPosition = Mathf.Abs(position.y - matchedBallPosition.y) > HEIGHT_DIFFERENCE &&
                                           position.y - matchedBallPosition.y < HEIGHT_DIFFERENCE;

                bool isSameBall = ReferenceEquals(matchedBall, _matchedBall);

                return GameManager.Instance.IsWin || matchedBall.Data.IsMatched || Data.IsMatched || _matchedBall != null || isSameBall || isIncorrectPosition;
            }
        }

        private void MoveTowardsOther(IMatchableItem<uint> matchedBall)
        {
            Tween.Position(transform, matchedBall.transform.position, moveTowardsDuration, 0, Tween.EaseInOutStrong,
                Tween.LoopType.None, null, OnMoveAnimationComplete);
        }

        private void OnMoveAnimationComplete()
        {
            UpdateData();

            if (IsWinCondition())
                GameManager.Instance.WinGame();
            else
                AudioManager.Instance.PlayFxSound(AudioFxTypes.Collapse);

            SpawnMatchVfx();

            PushMatchedBallBackToPool();
            ItemSpawner.Instance.UpdateMaxSpawnedCriteria(Data.Criteria);
            ResetBallState();
            SwitchCollisionDetection(this, true);
        }

        private void PushMatchedBallBackToPool()
        {
            _matchedBall.gameObject.name = _initialName;
            _matchedBall.gameObject.PushBackToPool();
        }

        private void SwitchPhysics(bool state) => CollisionHandler.SwitchPhysics(state);

        private static void SwitchCollisionDetection(Ball target, bool isActive)
        {
            target.SwitchPhysics(isActive);
            
            foreach (Transform childTransform in target.transform)
            {
                childTransform.gameObject.layer =
                    Constants.LayersNames.GetLayer(isActive ? Constants.LayersNames.Layers.Ball : Constants.LayersNames.Layers.IgnorePhysics);
            }
        }

        private bool IsWinCondition() =>
            Data?.Criteria >= GameManager.Instance.WinCriteria;

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
                    return;

                previousColor = BallData.Color;
                Data.UpdateData();
            }
            else
            {
                Data = new BallMatchableData(criteria);
                _initialName = name;
            }

            label.text = Data.Criteria.ToString();
            spriteRenderer.color = BallData.Color;
            name = _initialName + Data.Criteria;

            UpdateLocalScale(BallData.LocalScale);
        }
    }
}