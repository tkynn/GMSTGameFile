using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Opens the Pause scene additively when Escape is pressed during gameplay.
/// </summary>
public sealed class PauseInputRouter : MonoBehaviour
{
    private const string PauseSceneName = "Pause";
    private const string MainMenuSceneName = "MainMenu";
    private const string SettingsSceneName = "Settings";

    private static PauseInputRouter instance;
    private AudioSource audioSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (instance != null)
            return;

        var routerObject = new GameObject(nameof(PauseInputRouter));
        DontDestroyOnLoad(routerObject);
        instance = routerObject.AddComponent<PauseInputRouter>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public static void PlaySound(AudioClip clip)
    {
        if (clip != null && instance != null && instance.audioSource != null)
            instance.audioSource.PlayOneShot(clip);
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (SceneManager.GetSceneByName(PauseSceneName).isLoaded)
            return;

        string activeSceneName = SceneManager.GetActiveScene().name;
        if (activeSceneName == MainMenuSceneName || activeSceneName == SettingsSceneName)
            return;

        if (!Application.CanStreamedLevelBeLoaded(PauseSceneName))
        {
            Debug.LogError($"The '{PauseSceneName}' scene must be added to Build Settings to open the pause menu.");
            return;
        }

        Time.timeScale = 0f;
        PauseManager.paused = true;
        SceneManager.LoadSceneAsync(PauseSceneName, LoadSceneMode.Additive);
    }
}
