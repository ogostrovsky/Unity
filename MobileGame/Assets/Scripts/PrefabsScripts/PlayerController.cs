using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : Unit
{
    [Space(15)]
    [SerializeField] private Joystick   joystick;
    [SerializeField] private GameObject Camera;

    [Space (15)]
    [SerializeField] private float          _moveSpeed;
    [SerializeField] private float          _rotateSpeed;

    [Space(15)]
    [SerializeField] private float          _shootCoolDown;

    [Space(15)]
    [SerializeField] private Slider hpBar;

    protected override void FixedUpdate()
    {
        MovingController();
        base.FixedUpdate();
    }

    protected override void onBeforeDeath()
    {
        Camera.transform.parent = null;
        Score.Instance.ShowHighest();
        base.onBeforeDeath();
    }

    private void MovingController()
    {
        if (joystick.Horizontal != 0) {
            transform.Rotate(new Vector3(0, joystick.Horizontal * _rotateSpeed * Time.deltaTime));
        }

        if (joystick.Vertical != 0) { 
            transform.Translate(new Vector3(0, 0, joystick.Vertical * _moveSpeed * Time.deltaTime));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            GetComponentInChildren<Mesh>();



        }
    }

    private float _timeFromLastShot = 0f;
    public void Fire() {
        if (Time.time > _timeFromLastShot + _shootCoolDown) {
            gun.Shoot();
            _timeFromLastShot = Time.time;
        }
    }

    protected override void HpBarInit()
    {
        HpBarUI = hpBar;
        hpBar.maxValue = HP;
        hpBar.value = HP;
    }
}
