using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDestroyableObject : MonoBehaviour
{
    [SerializeField] private int _distanceToDestroyThisObject = 200;
    private GameObject _player;

    private void OnEnable()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogWarning("im " + name + " and my player is null");
            return;
        }
        StartCoroutine(Inspector());
    }

    IEnumerator Inspector()
    {
        var delay = new WaitForSeconds(0.2f);
        while (true)
        {
            if(Vector3.Distance(_player.transform.position, transform.position) > _distanceToDestroyThisObject)
            {
                gameObject.SetActive(false);
            }
            else if(transform.position.y < -50)
            {
                gameObject.SetActive(false);
            }
            yield return delay;
        }
    }

    private void OnDisable()
    {
        PoolManager.Instance.PutObj(gameObject);
    }
}
