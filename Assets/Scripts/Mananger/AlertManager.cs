using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlertManager : Singleton<AlertManager>
{
    [Header("UI")]
    [SerializeField] private GameObject alertPopup;
    private float fadeDuration = 2.0f; // 페이드아웃 지속 시간
    private bool isFadingOut = false;
    private float fadeOutTimer = 0f;

    string currentSceneName;

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {

        if(currentSceneName != SceneManager.GetActiveScene().name)
        {
            currentSceneName = SceneManager.GetActiveScene().name;
            StopAllCoroutines();
            SetAlertPopup();
        }
    }

    public void AlertPopup(string msg)
    {
        alertPopup.SetActive(true);
        alertPopup.GetComponent<CanvasGroup>().alpha = 1;
        alertPopup.GetComponentInChildren<TextMeshProUGUI>().text = msg;

        // 페이드아웃 시작
        isFadingOut = true;
        fadeOutTimer = 0f;
    }
    public void FadeoutAlert()
    {
        if (isFadingOut)
        {
            // 경과 시간 계산
            fadeOutTimer += Time.deltaTime;

            // alpha 값 계산 (0에서 fadeDuration까지 시간이 지나면 alpha는 0으로 감소)
            alertPopup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, fadeOutTimer / fadeDuration);

            // 페이드아웃 완료 시
            if (fadeOutTimer >= fadeDuration)
            {
                isFadingOut = false;
                alertPopup.SetActive(false); // 팝업을 비활성화
            }
        }
    }
    public void SetAlertPopup()
    {
        alertPopup.GetComponent<CanvasGroup>().alpha = 0;
        alertPopup.GetComponentInChildren<TextMeshProUGUI>().text = "";
        alertPopup.SetActive(false);
    }
}
