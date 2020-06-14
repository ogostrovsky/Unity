using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed    { get; set; }
    public float Damage   { get; set; }
    public float LifeTime { get; set; }

    private void OnEnable()
    {
        
        StartCoroutine(Lifetime());
    }

    private void OnTriggerEnter(Collider other) // если пуля во что-то влетела - вырубаем её
    {
        var rig = other.GetComponent<Rigidbody>();
        {
            if(rig != null)
            {
                rig.AddForce(this.transform.up * 1000);
            }
        }
        
        gameObject.SetActive(false);
    }

    private IEnumerator Lifetime()
    {
        while (true)
        {
            this.transform.Translate(new Vector3(0, Speed * Time.deltaTime, 0));// пуля летит

            if (LifeTime < 0)
            {
                this.gameObject.SetActive(false);
            }
            LifeTime -= Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
    }
}
