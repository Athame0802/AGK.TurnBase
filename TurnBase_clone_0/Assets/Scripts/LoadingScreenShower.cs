using Cysharp.Threading.Tasks;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenShower : MonoBehaviour
{
    public static LoadingScreenShower Instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingSlider;

    private AsyncOperation currentOperation;
    private bool isSubscribed = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        loadingScreen.SetActive(false);
    }

    private void Update()
    {
        TrySubscribeOnSceneNetworkManager();

        if (currentOperation != null)
        {
            loadingSlider.value = currentOperation.progress;
        }
    }

    private void OnDestroy()
    {
        TryUnsubscribeOnSceneNetworkManager();
    }

    private void TrySubscribeOnSceneNetworkManager()
    {
        if (isSubscribed) return;

        if (!CheckNetworkSceneManagerExist())
            return;

        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
        isSubscribed = true;
    }

    private void TryUnsubscribeOnSceneNetworkManager()
    {
        if (!isSubscribed) return;

        if (!CheckNetworkSceneManagerExist())
            return;

        NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
        isSubscribed = false;
    }

    private bool CheckNetworkSceneManagerExist()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null;
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.Load:
                ShowLoadingScreen(sceneEvent.AsyncOperation);
                break;
            case SceneEventType.LoadEventCompleted:
                HideLoadingScreen();
                break;
        }
    }

    private void ShowLoadingScreen(AsyncOperation operation)
    {
        currentOperation = operation;

        loadingSlider.value = 0f;
        loadingScreen.SetActive(true);
    }

    private void HideLoadingScreen()
    {
        currentOperation = null;

        loadingSlider.value = 1f;
        loadingScreen.SetActive(false);
    }

    public IProgress<float> SettingProgressBarForSolo()
    {
        IProgress<float> progress = Progress.Create<float>((p) =>
        {
            loadingSlider.value = p;
        });

        return progress;
    }
}
