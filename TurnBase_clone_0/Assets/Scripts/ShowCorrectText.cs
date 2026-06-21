using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowCorrectText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private StringBuilder stringBuilder = new(512);

    public void OnDestroy()
    {
        CheckAnswerManager.Instance.AnswerNetworkList.OnListChanged -= OnAnswerChecked;
    }

    private void Start()
    {
        CheckAnswerManager.Instance.AnswerNetworkList.OnListChanged += OnAnswerChecked;
        stringBuilder.AppendLine("답란 : ");
        stringBuilder.AppendLine();

        text.SetText(stringBuilder);
    }


    private void OnAnswerChecked(NetworkListEvent<AnswerData> changeEvent)
    {
        if (changeEvent.Type != NetworkListEvent<AnswerData>.EventType.Add)
            return;

        AnswerData answer = changeEvent.Value;

        stringBuilder.Append("<color=white>");

        if (answer.AnsweredClientId == NetworkManager.Singleton.LocalClientId)
        {
            stringBuilder.AppendLine("나 : ");
        }
        else
        {
            stringBuilder.AppendLine("상대 : ");
        }

        for (int i = 0; i < CorrectNumber.digit; i++)
        {
            AddNumberAndCorrect(answer.GetInput(i), answer.IsCorrect(i), answer.IsInaccurate(i));
        }

        stringBuilder.AppendLine();

        text.SetText(stringBuilder);
    }

    private void AddNumberAndCorrect(byte number, bool correct, bool inaccurate)
    {
        if (correct)
        {
            stringBuilder.Append("<color=green>");
        }
        else
        {
            if (inaccurate)
            {
                stringBuilder.Append("<color=yellow>");
            }
            else
            {
                stringBuilder.Append("<color=red>");
            }
        }

        stringBuilder.Append(number);
    }
}