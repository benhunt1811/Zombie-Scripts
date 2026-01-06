using DG.Tweening;
using System.Drawing;
using TMPro;
using UnityEngine;

public class HitTextScript : MonoBehaviour
{
    [Header("Dotween Values")]
    public float bigScale;
    public float smallScale;
    public float length;

    [Header("Text Values")]
    private TMP_Text text;
    public Color32 textColour;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.faceColor = textColour;
        this.transform.DOScale(bigScale, length).OnComplete(() => this.transform.DOScale(smallScale, length));
        float DeathTimer = length * 2;
        Destroy(gameObject, DeathTimer);
    }
}