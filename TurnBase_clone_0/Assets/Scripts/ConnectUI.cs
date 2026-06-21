using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Cysharp.Threading.Tasks;

public class ConnectUI : NetworkBehaviour
{
    private ushort port = 12345;
    private string ipAddress = "127.0.0.1";
    private int inputDigit = 4;

    private UnityTransport Transport => // 프로퍼티의 getter (식 본문 멤버)
        NetworkManager.Singleton.GetComponent<UnityTransport>();

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnNetworkDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    public void StartHosting()
    {
        Transport.SetConnectionData(ipAddress, port);

        bool isSucceedToParse = int.TryParse(InputFields.Instance.DigitInput.text, out inputDigit);

        if (!isSucceedToParse) return;
        if (inputDigit <= 0) return;

        bool isSucceed = NetworkManager.Singleton.StartHost();

        if (!isSucceed)
        {
            D.LogError($"[Network] Host 실패! / Port {port}가 이미 사용 중이거나 Transport 설정에 문제가 있을 수 있습니다.");
            return;
        }

        D.Log($"[Network] Host 성공! / Port {port}");
    }

    public void StartClient()
    {
        Transport.SetConnectionData(ipAddress, port);
        bool isSucceed = NetworkManager.Singleton.StartClient();

        if (!isSucceed)
        {
            D.LogError("[Network] 참가 실패! / 접속하려는 IP 주소가 잘못되었을 수도 있습니다!");
            return;
        }

        D.Log($"[Network] 참가 성공! / 접속한 IP : {ipAddress}, Port : {port}");
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;
        if (clientId == NetworkManager.ServerClientId) return;

        D.Log($"[Network] 클라이언트 {clientId}가 게임에 참가함!");
        
        GameManager.Instance.GameStart(inputDigit).Forget();
    }
}
