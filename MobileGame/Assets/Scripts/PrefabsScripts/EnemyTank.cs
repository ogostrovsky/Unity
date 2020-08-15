using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Xml.Serialization;

public class EnemyTank : Unit
{
    [Space(5)]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float distanceToReact = 5;
    [SerializeField] private float difficultyIncrease = 5;

    [Space(5)]
    [SerializeField] private float maxDamage = 5;
    [SerializeField] private float maxMoveSpeed = 5;
    [SerializeField] private float maxRotationSpeed = 5;
    [SerializeField] private float maxDistanceToReact = 5;
    [SerializeField] private float maxLocalScale = 5;

    [Space(5)]
    private GameObject          player;
    private bool                playerCanBeAttacked;
    private bool                sideRaysAlarm;
    private bool                isTurning;
    [SerializeField] private GameObject TankDoll;

    protected override void OnEnable()
    {
        base.OnEnable();

        player = GameObject.Find("Player");

        BoolsInit();

        DollInit();

        ColorInit();

        StartCoroutine(PlayerCanBeAttackedTracker());

        StartCoroutine(PathFinderController());

        StartCoroutine(StateMachene());
    }

    private void BoolsInit()
    {
        playerCanBeAttacked = false;
        sideRaysAlarm = false;
        isTurning = false;
    }
    private IEnumerator StateMachene()
    {
        while (true)
        {
            if (playerCanBeAttacked)    yield return StartCoroutine(Battle());
            else if (!sideRaysAlarm)    yield return StartCoroutine(Idle());
            yield return null;
        }
    }
    private IEnumerator PlayerCanBeAttackedTracker()
    {
        var delay = new WaitForSeconds(0.2f);
        while (true)
        {
            var distToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distToPlayer < distanceToReact)
            {
                if (ObstaclesChecker())
                {
                    playerCanBeAttacked = true;
                }
                else
                {
                    playerCanBeAttacked = false;
                }
            }
            else
            {
                playerCanBeAttacked = false;
            }
            yield return delay;
        }
    }
    private IEnumerator Idle()
    {
        switch (UnityEngine.Random.Range(0, 6))
        {
            case 0: //поворачиваем рандомное количество времени стоя
                yield return StartCoroutine(IdleCorutine(0.3f, 0.5f, () => RotateLeft()));
                break;

            case 1: //поворачиваем рандомное количество времени в другую сторону стоя
                yield return StartCoroutine(IdleCorutine(0.3f, 0.5f, () => RotateRight()));
                break;

            case 2: //поворачиваем и едем рандомное количество времени
                yield return StartCoroutine(IdleCorutine(0.3f, 0.8f, () => MoveForvard(), () => RotateLeft()));
                break;

            case 3://поворачиваем и едем рандомное количество времени в другую сторону
                yield return StartCoroutine(IdleCorutine(0.3f, 0.8f, () => MoveForvard(), () => RotateRight()));
                break;

            case 4:
                yield return StartCoroutine(IdleCorutine(1f, 3f, () => MoveForvard()));
                break;

            case 5:
                yield return StartCoroutine(IdleCorutine(2f, 5f, () => MoveForvard()));
                break;
        }
    }
    private IEnumerator Battle()
    {
        var delay = new WaitForSeconds(0.02f);
        while (playerCanBeAttacked) 
        {
            var rayOrigin = transform.position + transform.forward * 2;
            if (RayCast(rayOrigin, transform.forward, distanceToReact * 2) == "Player")
            {
                yield return StartCoroutine(FireCorutine(1, 5, 0.3f, () => gun.Shoot()));
            }
            var startRotation = transform.rotation;
            var targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(startRotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return delay;
        }
    }
    private IEnumerator FireCorutine(int min, int max, float attackSpeed, Action Method)
    {
        var delay = new WaitForSeconds(attackSpeed);
        var iterations = UnityEngine.Random.Range(min, max);
        while (iterations > 0 && playerCanBeAttacked)
        {
            Method();
            iterations--;
            yield return delay;
        }
    }
    private IEnumerator IdleCorutine(float min, float max, Action Method)
    {
        var delay = new WaitForSeconds(0.02f);
        var _time = UnityEngine.Random.Range(min, max);
        while (_time > 0 && !playerCanBeAttacked && !sideRaysAlarm)
        {
            Method();
            _time -= Time.deltaTime;
            yield return delay;
        }
    }
    private IEnumerator IdleCorutine(float min, float max, Action Method1, Action Method2)
    {
        var _time = UnityEngine.Random.Range(min, max);
        var delay = new WaitForSeconds(0.02f);
        while (_time > 0 && !playerCanBeAttacked && !sideRaysAlarm)
        {
            Method1();
            Method2();
            _time -= Time.deltaTime;
            yield return delay;
        }
    }
    private IEnumerator PathFinderController()
    {
        var delay = new WaitForSeconds(0.1f);
        while (true)
        {
            var leftrayOrigin = transform.position + transform.forward * 3 + transform.right * -2 + transform.up * 15;
            var rightrayOrigin = transform.position + transform.forward * 3 + transform.right * 2 + transform.up * 15;

            var rayLeft = RayCast(leftrayOrigin, Vector3.down, 30);
            var rayRight = RayCast(rightrayOrigin, Vector3.down, 30);

            if (!isTurning && !playerCanBeAttacked)
            {
                if (!String.Equals(rayLeft, "Section"))
                {
                    StartCoroutine(TurnCoroutine(true));
                }
                else if (!String.Equals(rayRight, "Section"))
                {
                    StartCoroutine(TurnCoroutine(false));
                }
            }
            else if (rayLeft != "Section" || rayRight != "Section")
            {
                sideRaysAlarm = true;
            }
            else
            {
                sideRaysAlarm = false;
            }
            yield return delay;
        }
    }
    private IEnumerator TurnCoroutine(bool left)
    {
        isTurning = true;
        var delay = new WaitForSeconds(0.02f);
        var addTurnTime = UnityEngine.Random.Range(0.2f, 0.4f);

        while (addTurnTime > 0 && !playerCanBeAttacked)
        {
            if (left) RotateRight();
            else      RotateLeft();

            if (!sideRaysAlarm) addTurnTime -= Time.deltaTime;
            yield return delay;
        }
        isTurning = false;
    }
    private string RayCast(Vector3 origin, Vector3 direction, float range)
    {
        RaycastHit hit;
        Physics.Raycast(origin, direction, out hit, range);
        if (hit.collider == null)
        {
            Debug.DrawRay(origin, direction * range, Color.red);
            return null;
        }
        else
        {
            Debug.DrawRay(origin, direction * range, Color.white);
            return hit.collider.tag;
        }
    }
    protected override void onBeforeDeath()
    {
        Score.Instance.KilledTanksScore += savedHP;
        DifficultyIncreasing();
        base.onBeforeDeath();
        TankExplodeOnDie();
    }

    private void TankExplodeOnDie()
    {
        TankDoll.transform.position = transform.position;
        TankDoll.transform.rotation = transform.rotation;
        TankDoll.SetActive(true);

        var explosionCenter = explosionCenterPointRandomizer();
        var explosionPower = UnityEngine.Random.Range(500f, 2500f);
        foreach (var i in TankDoll.GetComponentsInChildren<Rigidbody>())
        {
            i.AddExplosionForce(explosionPower, explosionCenter, 5f);
        }
    }

    private Vector3 explosionCenterPointRandomizer()
    {
        return transform.position 
            + new Vector3(
                UnityEngine.Random.Range(-1.5f, 1.5f),
                UnityEngine.Random.Range(-1.5f, 0),
                UnityEngine.Random.Range(-1.5f, 1.5f));
    }
    private void ColorInit()
    {
        var rendsArr = GetComponentsInChildren<Renderer>().Concat(TankDoll.GetComponentsInChildren<Renderer>()).ToArray(); // combining 2 arrays doll and tank's
        var Color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1);
        foreach (var rend in rendsArr)
        {
            rend.material.color = Color;
        }
    }
    private void MoveForvard()
    {
        transform.Translate(new Vector3(0, 0, moveSpeed * Time.deltaTime));
    }
    private void RotateLeft()
    {
        transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0));
    }
    private void RotateRight()
    {
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
    }
    private bool ObstaclesChecker()
    {
        var rayOrigin = transform.position + transform.up * 2;
        var direction = (player.transform.position - rayOrigin).normalized;
        var rayInfo = RayCast(rayOrigin, direction, distanceToReact);
        if (rayInfo != null)
        {
            if (String.Equals(rayInfo, "Player"))
            {
                return true;
            }
        }
        return false;
    }
    private void DifficultyIncreasing()
    {
        savedHP *= difficultyIncrease;

        damage    *= difficultyIncrease;

        if (moveSpeed               < maxMoveSpeed)        moveSpeed              += difficultyIncrease;
        if (rotationSpeed           < maxRotationSpeed)    rotationSpeed          += difficultyIncrease;    
        if (distanceToReact         < maxDistanceToReact)  distanceToReact        += difficultyIncrease;
        if (transform.localScale.x  < maxLocalScale)       transform.localScale   += new Vector3(0.1f, 0.1f, 0.1f);
    }
    private void DollInit()
    {
        TankDoll = PoolManager.Instance.GetObj(TankDoll);
        TankDoll.transform.localScale = transform.localScale;
    }

    private void OnDisable()
    {
        PoolManager.Instance.PutObj(gameObject);
    }
}