using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private Transform playerReference;
    [SerializeField] private Transform groundReference;


    [Header("Variables")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;


    private PlayerMovement pMovement;
    private Rigidbody rb;

    private float slideTimer;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
