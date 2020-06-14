using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour
{
    private GameObject _player;
    // Start is called before the first frame update
    private void OnEnable()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Inspector());
    }

    IEnumerator Inspector()
    {
        while (true)
        {
            if(Vector3.Distance(_player.transform.position, this.transform.position) > 220)
            {
                this.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
