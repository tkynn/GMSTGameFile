using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Turns the existing Pause scene into a clean overlay UI when it is loaded additively.
/// </summary>
public sealed class PauseSceneMenu : MonoBehaviour
{
    [SerializeField] private Sprite buttonBackground;
    [Header("Pause Menu Sound Effects")]
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonPressSound;
    [SerializeField] private AudioClip pauseOpenSound;
    [SerializeField] private AudioClip pauseCloseSound;

    private const string PauseSceneName = "Pause";
    private const string SettingsSceneName = "Settings";

    private void Awake()
    {
        Time.timeScale = 0f;
        PauseManager.paused = true;
        PreparePauseScene();
    }

    private void Start()
    {
        BuildMenu();
        PauseInputRouter.PlaySound(pauseOpenSound);
    }

    private void PreparePauseScene()
    {
        Scene pauseScene = SceneManager.GetSceneByName(PauseSceneName);
        if (!pauseScene.IsValid() || !pauseScene.isLoaded)
            return;

        foreach (GameObject root in pauseScene.GetRootGameObjects())
        {
            if (root == gameObject)
                continue;

            root.SetActive(false);
        }

        // Reuse the gameplay EventSystem where available so the additive scene does not
        // introduce a second active EventSystem.
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        bool anotherEventSystemExists = false;
        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem.gameObject != gameObject && eventSystem.isActiveAndEnabled)
            {
                anotherEventSystemExists = true;
                break;
            }
        }

        if (anotherEventSystemExists)
        {
            EventSystem ownEventSystem = GetComponent<EventSystem>();
            if (ownEventSystem != null)
                ownEventSystem.enabled = false;

            BaseInputModule ownInputModule = GetComponent<BaseInputModule>();
            if (ownInputModule != null)
                ownInputModule.enabled = false;
        }
    }

    private void BuildMenu()
    {
        GameObject canvasObject = new GameObject("Pause Menu Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject backdrop = new GameObject("Dim Background", typeof(RectTransform), typeof(Image));
        backdrop.transform.SetParent(canvasObject.transform, false);
        StretchToParent(backdrop.GetComponent<RectTransform>());
        backdrop.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.1f, 0.72f);

        GameObject titleObject = new GameObject("Paused Title", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleObject.transform.SetParent(backdrop.transform, false);
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(0f, 290f);
        titleRect.sizeDelta = new Vector2(700f, 80f);

        TextMeshProUGUI title = titleObject.GetComponent<TextMeshProUGUI>();
        title.text = "PAUSED";
        title.font = TMP_Settings.defaultFontAsset;
        title.fontSize = 52f;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.97f, 0.94f, 0.82f);
        title.raycastTarget = false;

        GameObject buttonList = new GameObject("Pause Menu Buttons", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        buttonList.transform.SetParent(backdrop.transform, false);
        RectTransform listRect = buttonList.GetComponent<RectTransform>();
        listRect.anchorMin = new Vector2(0.5f, 0.5f);
        listRect.anchorMax = new Vector2(0.5f, 0.5f);
        listRect.pivot = new Vector2(0.5f, 0.5f);
        listRect.anchoredPosition = new Vector2(0f, -35f);
        listRect.sizeDelta = new Vector2(460f, 0f);

        VerticalLayoutGroup layout = buttonList.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = buttonList.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        Button resumeButton = null;
        string[] labels = { "Resume", "Save", "Load Save", "Character List", "Settings", "Quit to Main Menu" };
        foreach (string label in labels)
        {
            Button button = CreateButton(buttonList.transform, label);
            if (label == "Resume")
            {
                resumeButton = button;
                button.onClick.AddListener(ResumeGame);
            }
            else if (label == "Settings")
            {
                button.onClick.AddListener(OpenSettings);
            }
        }

        EventSystem eventSystem = EventSystem.current;
        if (eventSystem != null && resumeButton != null)
            eventSystem.SetSelectedGameObject(resumeButton.gameObject);
    }

    private Button CreateButton(Transform parent, string label)
    {
        GameObject buttonObject = new GameObject(label + " Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.sprite = buttonBackground;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
        image.color = Color.white;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation { mode = Navigation.Mode.Automatic };
        button.onClick.AddListener(() => PauseInputRouter.PlaySound(buttonPressSound));

        EventTrigger eventTrigger = buttonObject.AddComponent<EventTrigger>();
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener(_ => PauseInputRouter.PlaySound(buttonHoverSound));
        eventTrigger.triggers.Add(pointerEnter);

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = 68f;
        layoutElement.minHeight = 68f;

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        StretchToParent(labelObject.GetComponent<RectTransform>());

        TextMeshProUGUI text = labelObject.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = 29f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.12f, 0.12f, 0.1f);
        text.raycastTarget = false;

        return button;
    }

    private void ResumeGame()
    {
        PauseInputRouter.PlaySound(pauseCloseSound);
        Time.timeScale = 1f;
        PauseManager.paused = false;

        if (SceneManager.GetSceneByName(PauseSceneName).isLoaded)
            SceneManager.UnloadSceneAsync(PauseSceneName);
    }

    private void OpenSettings()
    {
        PauseInputRouter.PlaySound(pauseCloseSound);
        Time.timeScale = 1f;
        PauseManager.paused = false;
        SceneManager.LoadScene(SettingsSceneName, LoadSceneMode.Single);
    }

    private static void StretchToParent(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
