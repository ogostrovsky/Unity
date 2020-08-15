using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private ScriptableObject Father;

    private GameObject bullet;
    private float _damage;
    private float _speed;
    private float _bulletLifeTime;

    public void OnInit(float damage, float speed, float bulletLifeTime, GameObject bullet)
    {
        _damage = damage;
        _speed = speed;
        _bulletLifeTime = bulletLifeTime;
        this.bullet = bullet;
    }


    public void Shoot()
    {
        var obj = PoolManager.Instance.GetObj(bullet);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;

        var sriptObj = obj.GetComponent<Bullet>();

        sriptObj.Damage = _damage;
        sriptObj.Speed = _speed;
        sriptObj.LifeTime = _bulletLifeTime;

        obj.SetActive(true);
    }
}
