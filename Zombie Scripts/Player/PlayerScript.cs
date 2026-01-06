using Cinemachine;
using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("General")]
    public GameObject startingPoint;
    public GameObject cameraRoot;
    [SerializeField] private GameObject playerMesh;
    public PlayerHealth playerHealth;
    private GameStateScript gameState;

    [Header("Gun")]
    [HideInInspector] public PlayerGunSelector playerGun;
    public GameObject gunHolder;
    public Vector3 gunMeleeRotation;
    public Vector3 gunMeleePosition;
    private bool isStartRot;
    Quaternion target = Quaternion.Euler(0, 0, 0);

    [Header("Melee")]
    public GameObject hitBox;
    [HideInInspector] public bool isMeleeing;
    private float meleeSpeed = 0.2f;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI ammoUpdateText;
    public TextMeshProUGUI killCountText;
    public BulletIconsScript bulletIconsScript;
    public Slider dashSlider;
    public TimerController timer;
    public TextMeshProUGUI timeAddedText;

    public int killCount;
    private int killTarget = 13;

    [Header("Inputs and Controllers")]
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public StarterAssetsInputs _input;

    [Header("Cameras")]
    public Camera mainCamera;
    public CinemachineVirtualCamera virtualCamera;


    public static PlayerScript Instance { get; private set; }
    public static bool InstanceFound => Instance != null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        else
            Destroy(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

        // Sets the player to the spawn position
        this.transform.position = startingPoint.transform.position;

        playerGun = GetComponent<PlayerGunSelector>();
        characterController = GetComponent<CharacterController>();
        playerGun = GetComponent<PlayerGunSelector>();
        _input = GetComponent<StarterAssetsInputs>();
        playerHealth = GetComponent<PlayerHealth>();
        
        gameState = GameStateScript.Instance;

        ReloadScriptableObject.onReload += UpdateAmmoText;  // Good practice to de-allocate them somewhere (normally in OnDestroy as well)
        GunScriptableObject.onEnemyHit += timer.AddTime;

        // Resets the timeAddedText
        timeAddedText.DOFade(0, 0);

        Cursor.lockState = CursorLockMode.Locked;

        killCount = 0;
    }

    private void OnDisable()
    {
        ReloadScriptableObject.onReload -= UpdateAmmoText;
        GunScriptableObject.onEnemyHit -= timer.AddTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Resets the player gun to its location once it moves
        if (isStartRot)
        {
            gunHolder.transform.localRotation = Quaternion.Lerp(transform.localRotation, target, Time.deltaTime * 2);
        }
    }

    public void LookLeft() => cameraRoot.transform.DORotate(new Vector3(0, -60, 0), 2);

    public void LookFront() => cameraRoot.transform.DORotate(new Vector3(0, 0, 0), 2);

    public void MakeMeshInvis()
    {
        playerMesh.SetActive(false);
        gunHolder.gameObject.SetActive(true);

        playerGun.currentGunObject.transform.localRotation = Quaternion.Euler(0, 80, 0);
    }

    public void MakeMeshVis()
    {
        playerMesh.SetActive(true);
        gunHolder.gameObject.SetActive(false);
    }

    private void UpdateAmmoText()
    {
        ammoText.text = playerGun.activeGun._gunAmmo + " / " + playerGun.activeGun._gunAmmoReserve;
    }

    public void UpdateKillCount()
    {
        killCount += 1;
        killCountText.text = killCount.ToString();

        if (killCount == killTarget)
        {
            if (gameState)
            {
                gameState.GameWin("Congratulations!", FormatToMinSec(), killCount);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                this.gameObject.SetActive(false);
            }
        }
    }

    public string FormatToMinSec()
    {
        float mins = Mathf.FloorToInt(timer.timeLeft / 60);
        float secs = Mathf.FloorToInt(timer.timeLeft % 60);

        string timeString = string.Format("{0:00}:{1:00}", mins, secs);

        return timeString;
    }

    public void Melee()
    {
        if (isMeleeing == false && playerGun.activeGun.isReloading == false && playerGun.activeGun.isZoomedIn == false)
        {
            isMeleeing = true;
            hitBox.gameObject.SetActive(true);
            gunHolder.transform.DOLocalMove(gunMeleePosition, meleeSpeed);
            gunHolder.transform.DOLocalRotate(gunMeleeRotation, meleeSpeed).OnComplete(() =>
            {
                gunHolder.transform.DOLocalRotate(new Vector3(0, 0, 0), meleeSpeed);
                gunHolder.transform.DOLocalMove(new Vector3(0, 0, 0), meleeSpeed).OnComplete(() =>
                {
                    hitBox.gameObject.SetActive(false);
                    isMeleeing = false;
                });
            });
        }
    }

    public void ShowAddedTime(float TimeAdded)
    {
        timeAddedText.text = "+ " + TimeAdded.ToString() + "s";
        timeAddedText.DOFade(1, 0.1f).OnComplete(() => timeAddedText.DOFade(0, 2f));
    }
}
