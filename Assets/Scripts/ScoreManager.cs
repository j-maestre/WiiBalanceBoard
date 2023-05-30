using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public float ScoreMultiplyer;
    public TextMeshProUGUI text;
    private float score;

    public TextMeshProUGUI gameOvetText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        score += Time.deltaTime * ScoreMultiplyer;
        text.text = Mathf.Round(score).ToString();
    }

    public void SetPoints()
    {
        gameOvetText.text = text.text;
    }
}
