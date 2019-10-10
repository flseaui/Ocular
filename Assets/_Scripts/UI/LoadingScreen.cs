using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    private const float MinTimeToShow = 3f;

    private AsyncOperation _currentLoadingOperation;

    private bool _isLoading;

    [SerializeField] private RectTransform barFillRectTransform;
    private Vector3 _barFillLocalScale;

    [SerializeField] private TextMeshProUGUI percentLoadedText;

    private float _timeElapsed;

    private Animator _animator;

    private bool _didTriggerFadeOutAnim;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _barFillLocalScale = barFillRectTransform.localScale;
        _animator = GetComponent<Animator>();
        Hide();
    }

    private void Update()
    {
        if (_isLoading)
        {
            SetProgress(_currentLoadingOperation.progress);
            if (_currentLoadingOperation.isDone && !_didTriggerFadeOutAnim)
            {
                _animator.SetTrigger("Hide");
                _didTriggerFadeOutAnim = true;
            }
            else
            {
                _timeElapsed += Time.deltaTime;

                if (_timeElapsed >= MinTimeToShow)
                {
                    _currentLoadingOperation.allowSceneActivation = true;
                }
            }
        }
    }

    private void SetProgress(float progress)
    {
        _barFillLocalScale.x = progress;
        barFillRectTransform.localScale = _barFillLocalScale;
        percentLoadedText.text = Mathf.CeilToInt(progress * 100).ToString() + "%";
    }

    public void StartLoading()
    {
        _currentLoadingOperation = SceneManager.LoadSceneAsync("Game");
        _currentLoadingOperation.allowSceneActivation = false;
        _isLoading = true;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetProgress(0f);
        _timeElapsed = 0f;
        _animator.SetTrigger("Show");
        _didTriggerFadeOutAnim = false;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _currentLoadingOperation = null;
        _isLoading = false;
    }
}