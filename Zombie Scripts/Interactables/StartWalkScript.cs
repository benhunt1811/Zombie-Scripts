using DG.Tweening;
using StarterAssets;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;
using UnityEngine.Timeline;

public class StartWalkScript : Interactable
{
    private GameObject Player;
    private FirstPersonController playerFirstPersonController;
    private SplineAnimate playerSplineAnimator;

    private GameObject NPC;
    private SplineAnimate NPCSpline;

    private Animator npcAnimator;
    private Animator playerAnimator;

    [SerializeField]
    private bool isSplineMoving;

    public GameObject questionMark;

    private PlayableDirector timeline;

    [SerializeField]
    private bool canInteract;

    [SerializeField]
    private bool willTurn;

    public SplineContainer playerWalkRoute;
    public SplineContainer npcWalkRoute;

    public GameObject testCube;

    public bool isWalking;
    public bool isSlowingDown;

    private float lerpTime;

    private float NPCAnimSpeed;
    private float PlayerAnimSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcAnimator = GetComponent<Animator>();

        timeline = GetComponentInChildren<PlayableDirector>();

        Player = PlayerScript.Instance.gameObject;
        playerAnimator = Player.GetComponentInChildren<Animator>();

        NPC = this.gameObject;
        NPCSpline = NPC.GetComponent<SplineAnimate>();

        canInteract = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        npcAnimator.SetFloat("Speed", NPCAnimSpeed);
        playerAnimator.SetFloat("Speed", PlayerAnimSpeed);



        if (isWalking && lerpTime < 0.8)
        {
            NPCAnimSpeed = Mathf.Lerp(1, 2, lerpTime / 0.2f);
            PlayerAnimSpeed = Mathf.Lerp(1, 2, lerpTime / 0.2f);
            lerpTime += Time.deltaTime;
        }

        if (isSlowingDown && lerpTime < 0.8)
        {
            NPCAnimSpeed = Mathf.Lerp(2, 1, lerpTime / 0.2f);
            PlayerAnimSpeed = Mathf.Lerp(2, 1, lerpTime / 0.2f);
            lerpTime += Time.deltaTime;
        }

        if (isWalking && lerpTime > 0.8)
        {
            isWalking = false;
            lerpTime = 0;
        }

        if (isSlowingDown && lerpTime > 0.8)
        {
            isSlowingDown = false;
            lerpTime = 0;
        }

    }

    public override void Interact()
    {
        if (isSplineMoving != true && canInteract)
        {
            isSplineMoving = true;
            questionMark.gameObject.SetActive(false);

            playerSplineAnimator = Player.GetComponent<SplineAnimate>();

            playerSplineAnimator.Container = playerWalkRoute;

            Vector3 secondSplinePoint = (playerSplineAnimator.splineContainer.Spline.ToArray()[1].Position);
            Vector3 startPosition = playerSplineAnimator.splineContainer.transform.position;
            Vector3 startRotation = Quaternion.LookRotation((startPosition + secondSplinePoint) - startPosition).eulerAngles;

            Debug.Log(startPosition.x);
            Debug.Log(startPosition.z);

            Debug.Log(startRotation.y);

            var tween = Player.transform.DOMove(startPosition, 3f);
            Player.transform.DORotate(startRotation, 3f).onComplete = StartWalk;

            if (willTurn)
            {
                npcAnimator.SetTrigger("Turn");
            }

            playerFirstPersonController = Player.GetComponent<FirstPersonController>();
            playerFirstPersonController.enabled = false;
        }

        if (isSplineMoving != true && canInteract != true) npcAnimator.SetTrigger("TalkingOne");
    }


    private void StartWalk()
    {
        isWalking = true;
        NPCSpline.Container = npcWalkRoute;
        playerSplineAnimator = Player.GetComponent<SplineAnimate>();
        playerSplineAnimator.enabled = true;
        playerSplineAnimator.Play();

        NPCSpline.Play();
        NPCSpline.Completed += SplineFinished;

        if (timeline)
        {
            timeline.Play();
        }
    }


    public override void ShowInteractable()
    {
        if (isSplineMoving != true)
        {
            questionMark.gameObject.SetActive(true);
        }
    }

    public override void HideInteractable()
    {
        if (isSplineMoving != true)
        {
            questionMark.gameObject.SetActive(false);
        }
    }

    private void SplineFinished()
    {
        canInteract = false;
        playerSplineAnimator.enabled = false;
        playerSplineAnimator.Container = null;
        playerSplineAnimator.Pause();
        playerSplineAnimator.ElapsedTime = 0;
        playerFirstPersonController = Player.GetComponent<FirstPersonController>();
        playerFirstPersonController.enabled = true;
        SlowDown();

        npcAnimator.SetBool("IsWalking", false);

        isSplineMoving = false;
    }

    public void RunUp()
    {
        isWalking = true;
    }

    public void SlowDown()
    {
        isSlowingDown = true;
    }
}
