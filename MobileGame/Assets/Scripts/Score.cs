using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Score : Singleton<Score>
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _highestScoreText;
    [SerializeField] private Button   ResetButton;
    [SerializeField] private Button   ToMenuButton;

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
            _scoreText.SetText("Score: " + OverallScore);

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
        _highestScoreText.SetText("Highest Score: " + (int)HighestScore);
        _highestScoreText.gameObject.SetActive(true);

        ResetButton.gameObject.SetActive(true);
        ToMenuButton.gameObject.SetActive(true);
    }

    public void ReloadScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void MoveToMenu() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
    }
}

