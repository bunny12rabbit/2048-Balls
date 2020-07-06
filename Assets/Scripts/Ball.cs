using Pixelplacement;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(TMP_Text))]
public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody2D myRigidbody;
    [SerializeField] private Collider2D myCollider;
    [SerializeField] private TMP_Text label;

    [Space]
    [SerializeField] private uint number = 2;
    [SerializeField] private float moveTowardsDuration = 0.5f;
    [SerializeField] private GameObject vfx;

    
    public Ball MatchedBall { get; private set; }

    public bool IsMatched { get; private set; }

    public uint Number
    {
        get => number;
        private set
        {
            number = value;
            label.text = number.ToString();
        }
    }

    private void OnValidate()
    {
        if (uint.Parse(label.text) != number)
        {
            label.text = number.ToString();
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (label == null)
        {
            label = GetComponent<TMP_Text>();
        }

        if (myCollider == null)
        {
            myCollider = GetComponent<Collider2D>();
        }

        if (myRigidbody == null)
        {
            myRigidbody = GetComponent<Rigidbody2D>();
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag(Tags.BALL))
        {
            return;
        }

        var otherBall = other.gameObject.GetComponent<Ball>();

        if (!IsMatched && otherBall && otherBall.Number == Number)
        {
            MatchedBall = otherBall;
            MatchBalls();
        }
    }

    private void MatchBalls()
    {
        IsMatched = true;
        SwitchPhysics(false);
        
        if (!MatchedBall.IsMatched)
        {
            MoveTowardsOther();
        }
    }

    private void MoveTowardsOther()
    {
        Tween.Position(transform, MatchedBall.transform.position, moveTowardsDuration, 0, Tween.EaseInOutStrong,
            Tween.LoopType.None, null, OnTweenAnimationComplete);
    }

    private void SwitchPhysics(bool state)
    {
        myCollider.enabled = state;
        myRigidbody.simulated = state;
    }

    private void OnTweenAnimationComplete()
    {
        // show vfx and return back to pool

        gameObject.SetActive(false);
        MatchedBall.gameObject.SetActive(false);
        Instantiate(vfx, transform.position, Quaternion.identity);
        Destroy(gameObject, moveTowardsDuration);
        Destroy(MatchedBall.gameObject, moveTowardsDuration);
    }
}