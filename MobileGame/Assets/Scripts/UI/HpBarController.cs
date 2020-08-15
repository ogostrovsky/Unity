using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarController : MonoBehaviour
{
    private Transform camerPosition;

    void Start()
    {
        camerPosition = FindObjectOfType<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(camerPosition);
    }
}
