using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    #region Singleton
    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    #endregion

    [SerializeField] private Image dialogUI;
    [SerializeField] private Image image;
    [SerializeField] private Text text;
    [SerializeField] private Dialog[] dialog;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject inventoryHUD;

    private Coroutine typingCoroutine;

    /// <summary>
    /// 현재 대화 인덱스
    /// </summary>
    private int currentIndex = 0;

    /// <summary>
    /// 종료 인덱스
    /// </summary>
    private int endIndex = 0;

    /// <summary>
    /// 대화 종료 알림
    /// </summary>
    public event Action OnDialogFinished;

    /// <summary>
    /// 대화 실행
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    public void Show(int startIndex, int endIndex)
    {
        if (startIndex < 0 || startIndex >= dialog.Length) return;
        if (endIndex < startIndex) return;

        /* 이동 불가 */
        Player.Instance.Movement.CanMove = false;

        this.currentIndex = startIndex;
        this.endIndex = Mathf.Min(endIndex, dialog.Length - 1);

        inventoryHUD.GetComponent<UIAnimator>().Close();

        image.sprite = dialog[startIndex].sprite;

        dialogUI.GetComponent<UIAnimator>().Show();
        StartCoroutine(ShowDelay(image, 0.1f));

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(dialog[startIndex].text, dialog[startIndex].typingSpeed));
    }

    /// <summary>
    /// 다음 대화 실행
    /// </summary>
    public void Next()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            text.text = dialog[currentIndex].text;
            typingCoroutine = null;
            return;
        }

        if (currentIndex >= endIndex)
        {
            Close();
            return;
        }

        currentIndex++;

        image.sprite = dialog[currentIndex].sprite;

        typingCoroutine = StartCoroutine(TypeText(dialog[currentIndex].text, dialog[currentIndex].typingSpeed));
    }

    /// <summary>
    /// 대화 종료
    /// </summary>
    public void Close()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        OnDialogFinished?.Invoke();

        inventoryHUD.GetComponent<UIAnimator>().Show();

        image.GetComponent<UIAnimator>().Close();
        StartCoroutine(CloseDelay(dialogUI, 0.1f));

        /* 이동 가능 */
        Player.Instance.Movement.CanMove = true;
    }

    /// <summary>
    /// 텍스트 타이핑 효과
    /// </summary>
    /// <param name="content">대화 텍스트</param>
    /// <param name="speed">타이핑 속도</param>
    /// <returns></returns>
    private IEnumerator TypeText(string content, float speed)
    {
        text.text = "";

        foreach (char c in content)
        {
            text.text += c;
            yield return new WaitForSeconds(speed);
        }

        typingCoroutine = null;
    }

    private IEnumerator ShowDelay(Image ui, float delay)
    {
        yield return new WaitForSeconds(delay);
        ui.GetComponent<UIAnimator>().Show();
    }

    private IEnumerator CloseDelay(Image ui, float delay)
    {
        yield return new WaitForSeconds(delay);
        ui.GetComponent<UIAnimator>().Close();
    }

    private void Start()
    {
        nextButton.onClick.AddListener(() => Next());
    }
}