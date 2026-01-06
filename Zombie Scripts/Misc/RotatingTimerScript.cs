using DG.Tweening;
using UnityEngine;

public class RotatingTimerScript : MonoBehaviour
{
    [Header("Rotate Values")]
    private Vector3 rotateVector = new Vector3(0, 360, 0);
    private float rotateLength = 2f;


    void Start()
    {   
        transform.DORotate(rotateVector, rotateLength, RotateMode.FastBeyond360).SetRelative(true)
           .SetEase(Ease.Linear).SetLoops(-1);
    }
}
