using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    
    public event Action OnGameEnd;
    public NetworkVariable<int> DigitInput;

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
        }
    }

    public async UniTask GameStart(int digit = 4)
    {
        await LoadSceneAsync(Scenes.InGame);
        
        DigitInput.Value = digit;
        RandomNumberGenerator generator = new(DigitInput.Value);

        await UniTask.DelayFrame(60);

        RefreshAllCharacterLimitRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RefreshAllCharacterLimitRpc()
    {
        NumInput.Instance.RefreshCharacterLimit();
    }

    public void LoadConnectScene()
    {
        NetworkManager.Singleton.Shutdown();
        LoadSceneAsync(Scenes.Connect).Forget();
    }

    private void GameEnd()
    {
        OnGameEnd?.Invoke();
    }

    [Rpc(SendTo.Everyone)]
    public void OnAllCorrectAnswerRpc()
    {
        GameEnd();
    }

    private async UniTask LoadSceneAsync(string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single)
    {
        D.Log("LoadSceneAsync 호출됨");
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            if (!IsServer) return;

            await SceneNetworkLoadAsync(this.GetCancellationTokenOnDestroy(), sceneName, sceneMode);
        }
        else
        {
            IProgress<float> progress = LoadingScreenShower.Instance.SettingProgressBarForSolo();
            await SceneManager.LoadSceneAsync(sceneName, sceneMode).ToUniTask(progress, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }

    private async UniTask<bool> SceneNetworkLoadAsync(CancellationToken cancellationToken, string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single)
    {
        bool isCompleted = false;

        void OnLoadEventCompleted(string loadSceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            bool isThisScene = sceneName == loadSceneName && sceneMode == loadSceneMode;

            if (!isThisScene) return;

            isCompleted = true;
        }

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        
        
        SceneEventProgressStatus progress = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, sceneMode);

        if (progress != SceneEventProgressStatus.Started)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
            return false;
        }

        await UniTask.WaitUntil(() => isCompleted, cancellationToken: cancellationToken);
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
    
        return true;        
    } 

    public void GameQuit()
    {
        NetworkManager.Singleton.Shutdown();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
