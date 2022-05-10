using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public static UI instance;
    [SerializeField] private Animator _textAnimator;
    [SerializeField] private TextMeshProUGUI _ScoreText;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _winScreen;

    private int _score;

    private void Awake()
    {
        instance = this;
    }

    public void AddScore(int growthAmount)
    {
        _score += growthAmount;
        _ScoreText.text = _score.ToString();
        _textAnimator.SetTrigger("text");
    }

    public void GameOverUI()
    {
        _gameOverScreen.SetActive(true);
        
        var rectTransformPosition = _ScoreText.rectTransform.anchoredPosition;
        rectTransformPosition.y = -520;
        _ScoreText.rectTransform.anchoredPosition = rectTransformPosition;
        
        _ScoreText.text = "Score: " + _score;
    }
    public void GameWinUI(int multiplyValue)
    {
        _winScreen.SetActive(true);
        
        var rectTransformPosition = _ScoreText.rectTransform.anchoredPosition;
        rectTransformPosition.y = -520;
        _ScoreText.rectTransform.anchoredPosition = rectTransformPosition;
        
        _ScoreText.text = "Score: " + _score * multiplyValue;
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
