using TMPro;
using UnityEngine;

public class AlertManager : Singleton<AlertManager>
{
    [Header("UI")]
    [SerializeField] private GameObject alertPopup;
    private float fadeDuration = 2.0f; // ���̵�ƿ� ���� �ð�
    private bool isFadingOut = false;
    private float fadeOutTimer = 0f;

    public void AlertPopup(string msg)
    {
        alertPopup.SetActive(true);
        alertPopup.GetComponent<CanvasGroup>().alpha = 1;
        alertPopup.GetComponentInChildren<TextMeshProUGUI>().text = msg;

        // ���̵�ƿ� ����
        isFadingOut = true;
        fadeOutTimer = 0f;
    }
    public void FadeoutAlert()
    {
        if (isFadingOut)
        {
            // ��� �ð� ���
            fadeOutTimer += Time.deltaTime;

            // alpha �� ��� (0���� fadeDuration���� �ð��� ������ alpha�� 0���� ����)
            alertPopup.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, fadeOutTimer / fadeDuration);

            // ���̵�ƿ� �Ϸ� ��
            if (fadeOutTimer >= fadeDuration)
            {
                isFadingOut = false;
                alertPopup.SetActive(false); // �˾��� ��Ȱ��ȭ
            }
        }
    }
    public void SetAlertPopup()
    {
        alertPopup.SetActive(false);
        alertPopup.GetComponent<CanvasGroup>().alpha = 0;
        alertPopup.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }
}
