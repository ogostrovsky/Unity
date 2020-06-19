using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private Button   fireButt;
    [Space]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _HP;
    [SerializeField] private float _shootCoolDown;
    [SerializeField] private GameObject Camera;


    private Gun _gun;

    private void Start()
    {
        _gun = GetComponentInChildren<Gun>();
    }

    private void FixedUpdate()
    {
        Controller();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            ReduceHP(other.GetComponent<Bullet>().Damage);//узнаём урон пули
        }
    }

    private void ReduceHP(float damage)
    {
        _HP -= damage;
        if (_HP <= 0)
        {
            OnDead();
            this.gameObject.SetActive(false);
        }
    }

    private void OnDead()
    {
        Camera.transform.parent = null;
        Score.Instance.ShowHighest();
    }

    private float _time = 0;
    private void Controller()
    {
        if (joystick.Horizontal != 0) {
            transform.Rotate(new Vector3(0, joystick.Horizontal * _rotateSpeed * Time.deltaTime, 0));
        }

        if (joystick.Vertical != 0) { 
            transform.Translate(new Vector3(0, 0, joystick.Vertical * _moveSpeed * Time.deltaTime));
        }

        if (transform.position.y <= -50f)
            OnDead();
    } 

    public void Shoting() {
        if (Time.time > _time + _shootCoolDown) {
            _gun.Shoot();
            _time = Time.time;
        }
    }
}
