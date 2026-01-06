using DG.Tweening;
using UnityEngine;

public class GunPickupAnimationScript : MonoBehaviour
{
    [Header("Dotween Object")]
    public Tween moveTween;

    [Header("Dotween Values")]
    private Vector3 movePosition;
    private float moveHeight = 0.75f;
    private float animSpeed = 2;

    void Start()
    {
        movePosition = new Vector3(transform.localPosition.x, moveHeight, transform.localPosition.z);
        moveTween =  transform.DOLocalMove(movePosition, animSpeed).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}