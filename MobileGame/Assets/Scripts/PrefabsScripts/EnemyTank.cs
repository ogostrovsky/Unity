using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Xml.Serialization;

public class EnemyTank : MonoBehaviour
{
    [SerializeField] private float _HP;
    [SerializeField] private float _DMG;

    [SerializeField] private float _minMoveSpeed;
    [SerializeField] private float _maxMoveSpeed;

    [SerializeField] private float _minRotateSpeed;
    [SerializeField] private float _maxRotateSpeed;

    [SerializeField] private float _minDistanceToReact;
    [SerializeField] private float _MaxDistanceToReact;

    [SerializeField] private float _difficultyIncrease;

    [SerializeField] private GameObject _player;
    private bool _playerCanBeAttacked = false; 
    
    private List<SideRay> _sideRaysList; //список боковых сенсоров
    private Ray _aimRay; //рей который направлен вперёд
    private Gun _gun; //пушка танка
    private Renderer[] _rendsArr;

    private void Awake()
    {
        
    }



    private void OnEnable()
    {
        _sideRaysList = new List<SideRay>(GetComponentsInChildren<SideRay>()); //получаем боковые рейкасты
        _player = GameObject.FindGameObjectWithTag("Player"); // находим плеера на сцене 
        _aimRay = GetComponentInChildren<Ray>(); //цепляем рейкаст для пушки 
        _gun = GetComponentInChildren<Gun>(); //цепляем пушку 
        _rendsArr = GetComponentsInChildren<Renderer>();
        ColorRandomizer();


        _hp = _HP;//обновляем хп.
        StartCoroutine(StateMachene()); //запускаем основную логику моба
        StartCoroutine(PositionTrecker()); // паралельная проверка состояния моба в пространстве
    }

    //точка входа для логики моба
    private IEnumerator StateMachene()
    {
        while (true)
        {
            if (_playerCanBeAttacked)
            {   // если игрок в области поражения начинаем его бомбить
                yield return StartCoroutine(Battle());
            }

            else if (GetRaysStatus())
            {   // пока лучи не вылезли за край карты поочерёдно включаем - то мув - то айдл
                yield return StartCoroutine(Move());
                yield return StartCoroutine(Idle());
            }
            
            else if(!GetRaysStatus())
            {   //нащупали пропасть, сворачиваем 
                yield return StartCoroutine(Turn());
            }
            yield return new WaitForSeconds(0.04f);
        }
    }

    private IEnumerator PositionTrecker()
    {
        while (true)
        {
            if (transform.position.y < -50)
            {//выключаем моба если он свалился с платформы 
                Dying();
                gameObject.SetActive(false);
            }

            var distToPlayer = Vector3.Distance(_player.transform.position, this.transform.position);
            if (distToPlayer < _minDistanceToReact)//даём знеать можно ли атаковать плеера
            {
                if (ObstaclesChecker())
                {
                    _playerCanBeAttacked = true;
                }
                else
                {
                    _playerCanBeAttacked = false;
                }
            }
            else
            {
                _playerCanBeAttacked = false;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator Move()
    {   //рандомим продолжительность движения вперёд
        var _time = SmallRandomizer(1f, 4f); 
        while (_time > 0 && GetRaysStatus() && !_playerCanBeAttacked)
        {
            _time -= Time.deltaTime;
            MoveForvard();
            yield return new WaitForSeconds(0.04f);
        }
    }

    private IEnumerator Idle()
    {
        int _actionsAmount = SmallRandomizer(1, 4);//рандомим количество действий что будет совершено в айдле
        int _switcher;      //рандомим следующее действие которое будем совершать

        while (_actionsAmount > 0 && GetRaysStatus() && !_playerCanBeAttacked)
        {
            _switcher = SmallRandomizer(0, 4);
            switch (_switcher)
            {
                case 0: //поворачиваем рандомное количество времени стоя
                    yield return StartCoroutine(SimpleCorutine(0.3f, 0.5f, () => RotateLeft()));
                    break;

                case 1: //поворачиваем рандомное количество времени в другую сторону стоя
                    yield return StartCoroutine(SimpleCorutine(0.3f, 0.5f, () => RotateRight()));
                    break;

                case 2: //поворачиваем и едем рандомное количество времени
                    yield return StartCoroutine(SimpleCorutine(0.3f, 0.8f, () => MoveForvard(), () => RotateLeft()));
                    break;

                case 3://поворачиваем и едем рандомное количество времени в другую сторону
                    yield return StartCoroutine(SimpleCorutine(0.3f, 0.8f, () => MoveForvard(), () => RotateRight()));
                    break;
            }
            _actionsAmount--;
            yield return new WaitForSeconds(0.04f);
        }
    }

    private IEnumerator Turn()
    {
        var _str = GetTrigerredRayName(); // узнаём откуда вспышка
        var _time = SmallRandomizer(0.1f, 0.3f); // рандомим время которое поворот будет продолжаться после того как рей уже залез на платформу чтоб избежать дёрганья моба вдоль края карты

        while (_time > 0 && !_playerCanBeAttacked)
        {
            if (GetRaysStatus())
            {   //когда луч заехал на платформу начинаем отсчитывать время до перехода в другое состояние
                _time -= Time.deltaTime;
            }
            ChangeDirection(_str);
            yield return new WaitForSeconds(0.04f);
        }
    }

    private IEnumerator Battle()
    {
        while (_player.activeInHierarchy && _playerCanBeAttacked)
        {
            var targetRotation = Quaternion.LookRotation(_player.transform.position - transform.position);//выбираем куда поворачивать
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _minRotateSpeed * Time.deltaTime);

            if (_aimRay.FindedTag == "Player")
            {//если упёрлись лучом в плеера начинаем стрелять рандомное количество раз 
                yield return StartCoroutine(SimpleCorutine(1, 5, () => _gun.Shoot()));
            }
            yield return new WaitForSeconds(0.04f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {   //понималка того влетела ли в танк пуля
        if (other.gameObject.tag == "Bullet")
        {
            ReduceHP(other.GetComponent<Bullet>().Damage);//узнаём урон пули
        }
    }

    private string[] _avoidable = {"EnemyTank", "Cube", "Player"}; //список сущностей которое моб должен избегать
    private bool GetRaysStatus()
    {   //определяет статус всех реев на родителе
        foreach (var ray in _sideRaysList)
        {
            if (!ray.State || _avoidable.Contains(ray.FindedTag))
            {
                return false;
            }
        }
        return true;
    }
    private string GetTrigerredRayName()
    {
        foreach (var ray in _sideRaysList)
        {
            if (!ray.State || _avoidable.Contains(ray.FindedTag))
            {//возвращаем имя луча от которого нужно поворачивать танк в противоположную сторону
                return ray.name;
            }
        }
        return "noName";
    }

    private void ChangeDirection(string str)
    {   //основываясь на местоположении сигнала определяет в какую сторону разворачивать танк
        if (str.Equals("RightRay"))
        {
            RotateLeft();
        }
        else if (str.Equals("LeftRay"))
        {
            RotateRight();
        }
    }

   private void ColorRandomizer()
    {
        var Color = new Color(SmallRandomizer(0f, 1f), SmallRandomizer(0f, 1f), SmallRandomizer(0f, 1f), 1);
        foreach (var rend in _rendsArr)
        {
            rend.material.color = Color;
        }
    }

    private void DifficultyIncreasing()
    {
        _HP = _HP * _difficultyIncrease;
        _DMG = _DMG * _difficultyIncrease;
        if(_minMoveSpeed < _maxMoveSpeed)
        {
            _minMoveSpeed += _difficultyIncrease;
        }
        if(_minRotateSpeed < _maxRotateSpeed)
        {
            _minRotateSpeed = _minRotateSpeed * _difficultyIncrease;
        }
        if(_minDistanceToReact < _MaxDistanceToReact)
        {
            _minDistanceToReact = _minDistanceToReact * _difficultyIncrease;
        }
        if (this.transform.localScale.x < 2.5)
        {
            this.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    private float _hp;
    private void ReduceHP(float damage)
    {
        _hp -= damage;
        if(_hp <= 0)
        {
            Dying();
        }
    }
    private void Dying()
    {
        Score.Instance.KilledTanksScore += _HP;
        DifficultyIncreasing();

        this.gameObject.SetActive(false);
    }

    private void MoveForvard()
    {
        this.transform.Translate(new Vector3(0, 0, _minMoveSpeed * Time.deltaTime));
    }
    private void RotateLeft()
    {
        this.transform.Rotate(new Vector3(0, -_minRotateSpeed * Time.deltaTime, 0));
    }
    private void RotateRight()
    {
        this.transform.Rotate(new Vector3(0, _minRotateSpeed * Time.deltaTime, 0));
    }

    private bool ObstaclesChecker()
    {
        RaycastHit hit;
        var pos = new Vector3(this.transform.position.x, this.transform.position.y + 2, this.transform.position.z);

        Physics.Raycast(pos, _player.transform.position - pos, out hit, _minDistanceToReact);
        Debug.DrawRay(pos, (_player.transform.position - pos), Color.yellow);

        if (hit.collider != null)
        {
            if(hit.transform.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    private int SmallRandomizer(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    private float SmallRandomizer(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    private IEnumerator SimpleCorutine(int min, int max, Action Method)
    {
        var _times = SmallRandomizer(min, max);
        while (_times > 0)
        {
            Method();
            _times--;
            yield return new WaitForSeconds(0.3f);
        }
    }
    private IEnumerator SimpleCorutine(float min, float max, Action Method)
    {
        var _time = SmallRandomizer(min, max);
        while (_time > 0)
        {
            Method();
            _time -= Time.deltaTime;
            yield return new WaitForSeconds(0.04f);
        }
    }
    private IEnumerator SimpleCorutine(float min, float max, Action Method1, Action Method2)
    {
        var _time = SmallRandomizer(min, max);
        while (_time > 0)
        {
            Method1();
            Method2();
            _time -= Time.deltaTime;
            yield return new WaitForSeconds(0.04f);
        }
    }
}