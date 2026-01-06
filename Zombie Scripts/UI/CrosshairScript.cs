using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairScript : MonoBehaviour
{
    [Header("Components")]
    private RectTransform Crosshair;
    public FirstPersonController firstPersonController;

    [Header("Size Fields")]
    public float restingSize;
    public float maxSize;
    public float maxWalkSize;
    public float maxRunSize;
    private float currentSize;

    [Header("Speed Fields")]
    public float speed;
    public float normalSpeed;
    public float midAirSpeed;
    private bool isRunning => (Input.GetKey(KeyCode.LeftShift));


    private void Start()
    {
        Crosshair = GetComponent<RectTransform>();
    }

    private void Update()
    {

        if (IsMoving || firstPersonController.Grounded != true)
        {
            currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * speed);
        }

        else
        {
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);
        }

        Crosshair.sizeDelta = new Vector2(currentSize, currentSize);

        if(isRunning)
        {
            maxSize = maxRunSize;
        }

        if (isRunning == false && firstPersonController.Grounded == true)
        {
            maxSize = maxWalkSize;
            speed = normalSpeed;
        }

        if (firstPersonController.Grounded != true)
        {
            speed = midAirSpeed;
            maxSize = maxRunSize;
        }


    }

    bool IsMoving
    {
        get
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetMouseButton(0))
            {
                return true;
            }

            else
            {
                return false;
            }

        }
    }
}
