using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using System.Linq;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private GameObject _section;
    [SerializeField] private GameObject _cube;
    [SerializeField] private GameObject _enemyTank;
    [SerializeField] private GameObject _bullet;

    private Dictionary<PoolType, Pool> _poolsDict;// словарь пулов

    private void Awake()
    {
        _poolsDict = new Dictionary<PoolType, Pool>() //инициализируем словарь для пулов c нужными плами
        {
            { PoolType.SECTIONS,    new Pool(_section)      },
            { PoolType.CUBES,       new Pool(_cube)         },
            { PoolType.TANKENEMY,   new Pool(_enemyTank)    },
            { PoolType.BULLETS,     new Pool(_bullet)       },
        };
    }

    public GameObject GetObj(PoolType type)
    {
        return _poolsDict[type].GetFromPool();
    }
}

public class Pool : MonoBehaviour
{
    private List<GameObject> _pool;
    private GameObject _obj;

    public Pool(GameObject obj)
    {
        _obj = obj;
        _pool = new List<GameObject>();
    }

    public void AddToPool(GameObject otherObj)
    {
            otherObj.transform.parent = GameObject.Find("MapSubjects").transform;
            _pool.Add(otherObj);
    }

    public GameObject GetFromPool()
    {
        var obj = _pool.FirstOrDefault(x => !x.activeSelf);
        if (obj == null)
        {
            var newObj = Instantiate(_obj);
            AddToPool(newObj);
            return newObj;
        }
        return obj;
    }
}

public enum PoolType //все допустимые пулы
{
    SECTIONS,
    CUBES,
    TANKENEMY,
    BULLETS
}
