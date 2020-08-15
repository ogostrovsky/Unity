using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfAssemblingUnit : MonoBehaviour
{
    private List<KeyValuePair<Vector3, Quaternion>> initialPos = new List<KeyValuePair<Vector3, Quaternion>>(8);
    private List<Rigidbody> RBelements = new List<Rigidbody>(8);
    private void Awake()
    {
        RBelements.AddRange(GetComponentsInChildren<Rigidbody>());
        foreach (var i in RBelements) {
            initialPos.Add(new KeyValuePair<Vector3, Quaternion>(i.transform.localPosition, i.transform.localRotation));
        }
    }

    private void OnEnable()
    {
        for (var i = 0; i < RBelements.Count; i++)
        {
            RBelements[i].velocity = Vector3.zero;
            RBelements[i].transform.localPosition = initialPos[i].Key;
            RBelements[i].transform.localRotation = initialPos[i].Value;
        }
    }
}
