using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class MapGenManager : Singleton<MapGenManager>
{
    [SerializeField] private GameObject player; 
    [SerializeField] private GameObject section;
    [SerializeField] private GameObject cube;
    [SerializeField] private GameObject enemyTank;


    [SerializeField] private float      _cubeMinSize; //для рандомайзера размера кубов на сцене
    [SerializeField] private float      _cubeMaxSize; 
    [SerializeField] private float      _chanceToSpawnCube;// шанс спавна куба на секции

    [SerializeField] private float      _chanceToSpawnTank;

    [SerializeField] private int        _ancher;      // для того чтоб устанавливать дальность геренации карты 
    [SerializeField] private int        _howFarSpawnCustomObj; // как далеко от стартовой секции спавнить танки и кубы
    [SerializeField] private int        _sectionShift;// для рандомайзер-создателя секций рандомные сдвиги влево-вправо некст текции

    private void FixedUpdate()
    {
        if (player.transform.position.z > _ancher)
        {
            CreateSection();
            _ancher += (int)section.transform.localScale.z; //пододвигаем якорь на длину секции 
        }
    }



    private int _indexZ = 0;//чтоб всегда знать где была заспавнена последняя секция
    private int _shiftX = 0;//чтоб знать сдвиг последней секции, (сдвиг рандомится каждый раз от места последней секции)
    private void CreateSection()
    {
        var obj = PoolManager.Instance.GetObj(section);//спавним секцию
        obj.transform.position = new Vector3(_shiftX, 0, _indexZ);//даём ей сдвиг по Z и рандомим сдвиг X
        obj.SetActive(true);

        SectionFiller(); // наполняем секцю пропсами и мобами

        _shiftX = Random.Range(_shiftX - _sectionShift, _shiftX + _sectionShift);//рандомим сдвиг
        _indexZ += (int)section.transform.localScale.z;                         // двигаемся вперёд на ширину секции
    }

    private void SectionFiller()
    {
        if (_indexZ > _howFarSpawnCustomObj)// ориентируясь на дальность плеера начинаем наполнять карту
        {
            switch (Random.Range(0, 2))//исключаю возможость спавна 2 объектов в 1 клетке
            {
                case 0:
                    PutObjOnSection(_indexZ, _shiftX, _chanceToSpawnTank, enemyTank);
                    break;
                case 1:
                    PutObjOnSection(_indexZ, _shiftX, _chanceToSpawnCube, cube, _cubeMinSize, _cubeMaxSize);
                    break;
            }
        }
    }

    private void PutObjOnSection(int Z, int X, float chance, GameObject obj)//для мобов заточено
    {
        for (var i = 0; i < 5; i++)//каждая секция условно разбита на 5 участков, для потенциального спавна чего-либо
        {
            if (Random.value < chance)
            {
                var temp = PoolManager.Instance.GetObj(obj);
                var Y = (temp.transform.localScale.y / 2) + (section.transform.localScale.y / 2);// определяем на какую высоту ставить объект 
                temp.transform.position = new Vector3(X - 20, Y, Z);// -20 по иксу чтоб начать с самого старта секции (костыль)
                temp.transform.eulerAngles = new Vector3(0, Random.Range(0,360), 0);//рандомим ротейшн 
                temp.SetActive(true);
            }
            X += (int)section.transform.localScale.x / 5;// двигаемся по секции относительно размера секции
        }
    }

    private void PutObjOnSection(int Z, int X, float chance, GameObject obj, float sizeMin, float sizeMax)//для пропстов заточено
    {
        for (var i = 0; i < 5; i++)
        {//секция условно делится на 5 частей где может спавниться объект
            if (Random.value < chance) // шанс спавна на каждой секции
            {
                var temp = PoolManager.Instance.GetObj(obj);
                temp.transform.localScale = new Vector3(Random.Range(_cubeMinSize, _cubeMaxSize), Random.Range(_cubeMinSize, _cubeMaxSize), Random.Range(_cubeMinSize, _cubeMaxSize));//рандомим сайз
                var Y = (temp.transform.localScale.y / 2) + (section.transform.localScale.y / 2);// определяем на какую высоту ставить объект 
                temp.transform.position = new Vector3(X - 20, Y, Z);
                temp.SetActive(true);
            }
            X += (int)section.transform.localScale.x / 5;
        }
    }
}