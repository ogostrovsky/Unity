using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class MapGenManager : Singleton<MapGenManager>
{
    [SerializeField] private GameObject _player; // игрока цепляю
    [SerializeField] private GameObject _section; // секция 

    [SerializeField] private float      _cubeMinSize; //для рандомайзера размера кубов на сцене
    [SerializeField] private float      _cubeMaxSize; 
    [SerializeField] private float      _chanceToSpawnCube;// шанс спавна куба на секции

    [SerializeField] private float      _chanceToSpawnTank;

    [SerializeField] private int        _ancher;      // для того чтоб устанавливать дальность геренации карты 
    [SerializeField] private int        _howFarSpawnCustomObj; // как далеко от стартовой секции спавнить танки и кубы
    [SerializeField] private int        _sectionShift;// для рандомайзер-создателя секций рандомные сдвиги влево-вправо некст текции

    public void Start()
    {   
        StartCoroutine(InspectorCorutine());
    }

    private IEnumerator InspectorCorutine()
    {   //чекает якорь и генерирует карту на необходимую дальность вперёд
        while (true)
        {
            if (_player.transform.position.z > _ancher)
            {
                CreateSection();
                _ancher += (int)_section.transform.localScale.z; //пододвигаем якорь на длину секции 
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private int _indexZ = 0;//чтоб всегда знать где была заспавнена последняя секция
    private int _shiftX = 0;//чтоб знать сдвиг последней секции, (сдвиг рандомится каждый раз от места последней секции)
    private void CreateSection()
    {
        var obj = PoolManager.Instance.GetObj(PoolType.SECTIONS);//спавним секцию
        obj.transform.position = new Vector3(_shiftX, 0, _indexZ);//даём ей сдвиг по Z и рандомим сдвиг X
        obj.SetActive(true);

        SectionFiller(); // наполняем секцю пропсами и мобами

        _shiftX = Random.Range(_shiftX - _sectionShift, _shiftX + _sectionShift);//рандомим сдвиг
        _indexZ += (int)_section.transform.localScale.z;                         // двигаемся вперёд на ширину секции
    }

    private void SectionFiller()
    {
        if (_indexZ > _howFarSpawnCustomObj)// ориентируясь на дальность плеера начинаем наполнять карту
        {
            switch (Random.Range(0, 2))//исключаю возможость спавна 2 объектов в 1 клетке
            {
                case 0:
                    PutObjOnSection(_indexZ, _shiftX, _chanceToSpawnTank, PoolType.TANKENEMY);
                    break;
                case 1:
                    PutObjOnSection(_indexZ, _shiftX, _chanceToSpawnCube, PoolType.CUBES, _cubeMinSize, _cubeMaxSize);
                    break;
            }
        }
    }

    private void PutObjOnSection(int Z, int X, float chance, PoolType type)//для мобов заточено
    {
        for (var i = 0; i < 5; i++)//каждая секция условно разбита на 5 участков, для потенциального спавна чего-либо
        {
            if (Random.value < chance)
            {
                var obj = PoolManager.Instance.GetObj(type);
                var Y = (obj.transform.localScale.y / 2) + (_section.transform.localScale.y / 2);// определяем на какую высоту ставить объект 
                obj.transform.position = new Vector3(X - 20, Y, Z);// -20 по иксу чтоб начать с самого старта секции (костыль)
                obj.transform.eulerAngles = new Vector3(0, Random.Range(0,360), 0);//рандомим ротейшн 
                obj.SetActive(true);
            }
            X += (int)_section.transform.localScale.x / 5;// двигаемся по секции относительно размера секции
        }
    }

    private void PutObjOnSection(int Z, int X, float chance, PoolType type, float sizeMin, float sizeMax)//для пропстов заточено
    {
        for (var i = 0; i < 5; i++)
        {//секция условно делится на 5 частей где может спавниться объект
            if (Random.value < chance) // шанс спавна на каждой секции
            {
                var obj = PoolManager.Instance.GetObj(type);
                obj.transform.localScale = new Vector3(Random.Range(_cubeMinSize, _cubeMaxSize), Random.Range(_cubeMinSize, _cubeMaxSize), Random.Range(_cubeMinSize, _cubeMaxSize));//рандомим сайз
                var Y = (obj.transform.localScale.y / 2) + (_section.transform.localScale.y / 2);// определяем на какую высоту ставить объект 
                obj.transform.position = new Vector3(X - 20, Y, Z);
                obj.SetActive(true);
            }
            X += (int)_section.transform.localScale.x / 5;
        }
    }
}