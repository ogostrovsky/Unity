using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using System.Linq;
using System;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, Queue<GameObject>> poolsDict;

    private void Awake()
    {
        poolsDict = new Dictionary<string, Queue<GameObject>>();
    }

    public GameObject GetObj(GameObject obj){
        var name = obj.name + "(Clone)";
        if (!poolsDict.ContainsKey(name)) poolsDict.Add(name, new Queue<GameObject>());

        if (!poolsDict[name].Any())
        {
            var temp = Instantiate(obj);
            temp.SetActive(false);
            return temp;
        }
        else return poolsDict[name].Dequeue();
    }

    public void PutObj(GameObject obj)
    {
        poolsDict[obj.name].Enqueue(obj);
    }
}