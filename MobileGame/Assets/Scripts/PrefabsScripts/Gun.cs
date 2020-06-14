using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;
    [SerializeField] private float _bulletLifeTime;

    public void Shoot()
    {
        var obj = PoolManager.Instance.GetObj(PoolType.BULLETS);
        obj.transform.position = this.transform.position;
        obj.transform.rotation = this.transform.rotation;

        var sriptObj = obj.GetComponent<Bullet>();

        sriptObj.Damage = _damage;
        sriptObj.Speed = _speed;
        sriptObj.LifeTime = _bulletLifeTime;

        obj.SetActive(true);
    }
}
