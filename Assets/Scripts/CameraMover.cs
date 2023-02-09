using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
