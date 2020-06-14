using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.time > _time + _shootCoolDown)
            {
                _gun.Shoot();
                _time = Time.time;
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(new Vector3(0, 0, _moveSpeed * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(new Vector3(0, 0, -_moveSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Rotate(new Vector3(0, -_rotateSpeed * Time.deltaTime, 0));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.Rotate(new Vector3(0, _rotateSpeed * Time.deltaTime, 0));
        }
    } 
}
