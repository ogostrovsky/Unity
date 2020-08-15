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
            if(rig != null) rig.AddForce(transform.up * 1000);
        }
        gameObject.SetActive(false);
    }

    private IEnumerator Lifetime()
    {
        var vect = new Vector3(0, Speed, 0);
        while (true)
        {
            transform.Translate(vect * Time.deltaTime);// пуля летит
            if (LifeTime < 0)
            {
                gameObject.SetActive(false);
            }
            LifeTime -= Time.deltaTime;
            yield return null;
        }
    }

    private void OnDisable(){
        PoolManager.Instance.PutObj(gameObject);
    }
}
