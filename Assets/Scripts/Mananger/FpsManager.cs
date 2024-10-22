using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsManager : Singleton<FpsManager>
{
    public TextMeshProUGUI fpsText;
    private int frameCount = 0;
    private float deltaTime = 0.0f;
    private float updateInterval = 1f; // 0.5초 간격으로 갱신

    void Update()
    {
        frameCount++;
        deltaTime += Time.deltaTime;

        if (deltaTime > updateInterval)
        {
            float fps = frameCount / deltaTime; // FPS 평균 계산
            fpsText.text = $"{Mathf.RoundToInt(fps)}";

            // 초기화
            frameCount = 0;
            deltaTime = 0.0f;
        }
    }
}
