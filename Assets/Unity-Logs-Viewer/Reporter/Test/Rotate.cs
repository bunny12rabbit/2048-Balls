using UnityEngine;

public class Rotate : MonoBehaviour
{
    private Vector3 _angle;

    private void Start()
    {
        _angle = transform.eulerAngles;
    }

    private void Update()
    {
        _angle.y += Time.deltaTime * 100;
        transform.eulerAngles = _angle;
    }
}