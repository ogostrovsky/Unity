using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [Space(15)]
    [SerializeField] protected float    damage;
    [SerializeField] private float      bulletspeed;
    [SerializeField] private float      bulletLifeTime;
    [SerializeField] private GameObject bullet;
    protected Gun gun;



    [SerializeField] private float       hp;

    [SerializeField] protected float     savedHP;
    protected float     HP
    {
        get => hp;

        set
        {
            hp = value;

            if (hp <= 0) { hp = 0; }
            HpBarUI.value = hp;
        }
    }

    protected Slider    HpBarUI;

    protected virtual void  OnEnable()
    {
        GunInit();
        HpBarInit();
    }
    protected virtual void  FixedUpdate()
    {
        DeathByFallController();
    }
    private void            DeathByFallController()
    {
        if (transform.position.y <= -50f)
        {
            ReduceHP(HP);
        }
    }

    private void GunInit()
    {
        gun = GetComponentInChildren<Gun>();
        gun.OnInit(damage, bulletspeed, bulletLifeTime, bullet);
    }


    protected virtual void  HpBarInit()
    {
        HpBarUI = GetComponentInChildren<Slider>();
        HP = savedHP;
        HpBarUI.maxValue = HP;
        HpBarUI.value = HP;
    }
    protected void          ReduceHP(float damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            onBeforeDeath();
        }
    }
    protected virtual void  onBeforeDeath()
    {
        gameObject.SetActive(false);
    }
    private void            OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            ReduceHP(other.GetComponent<Bullet>().Damage);
        }
    }
}
