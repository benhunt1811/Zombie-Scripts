using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FractureForceScript : MonoBehaviour
{
    private UnfreezeFragment unfreeze;
    private Rigidbody body;
    private GameObject player;

    public Rigidbody[] objectList { get; private set; }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        unfreeze = GetComponent<UnfreezeFragment>();    
    }

    public void ApplyForce()
    {
        unfreeze.unfreezeAll = true;
        body.AddForce(player.transform.forward * Random.Range(5, 100), ForceMode.Impulse);
    }

    public void Force()
    {
        objectList = GetComponentsInChildren<Rigidbody>();

        if (PlayerScript.InstanceFound)
            player = PlayerScript.Instance.gameObject;

        // Gets all objects in the list and applies force
        for (var i = 0; i < objectList.Length; i++)
        {
            objectList[i].constraints = RigidbodyConstraints.None;
            objectList[i].AddForce(player.transform.forward * Random.Range(50, 75), ForceMode.Impulse);
            objectList[i].freezeRotation = false;
        }
    }
}