using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;

public class Score : Singleton<Score>
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _highestScoreText;

    [SerializeField] private GameObject _player;

    private int OverallScore
    {
        get
        {
            return (int)(KilledTanksScore + DistanceZ);
        }
    }

    public float KilledTanksScore { get; set; }

    private float HighestScore { get; set; }

    private float _distanceZ = 0;
    private float DistanceZ
    {
        get
        {
            if (_distanceZ < _player.transform.position.z)
            {
                _distanceZ = _player.transform.position.z;
            }
            return _distanceZ;
        }
    }

    private void Awake()
    {
        HighestScore = PlayerPrefs.GetFloat("HighestScore");
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScoreController());
    }

    // Update is called once per frame
    private IEnumerator ScoreController()
    {
        while (true)
        {
            _scoreText.text = "Score: " + OverallScore;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CheckHighest()
    {
        if (HighestScore < OverallScore)
        {
            HighestScore = OverallScore;
            PlayerPrefs.SetFloat("HighestScore", HighestScore);
        }
    }

    public void ShowHighest()
    {

        CheckHighest();
        _scoreText.fontSize = 160;


        _highestScoreText.text = "Highest Score: " + (int)HighestScore;
        _highestScoreText.gameObject.SetActive(true);
    }
}

