using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SideRay : MonoBehaviour
{
    [SerializeField] private float _rayRange;

    public bool State { get; set; }
    public string FindedTag { get;set; }

    private void OnEnable()
    {
        StartCoroutine(rayCast());
    }

    private IEnumerator rayCast()
    {
        RaycastHit hit;

        while (true)
        {
            Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.down), out hit, _rayRange);

            if (hit.collider != null)
            {
                Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.down) * _rayRange, Color.green);
                FindedTag = hit.transform.tag; 
                State = true;
            }
            
            else
            {
                Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.down) * _rayRange, Color.red);
                State = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}