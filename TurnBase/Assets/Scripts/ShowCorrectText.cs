using UnityEngine;
using TMPro;
using System.Text;

public class ShowCorrectText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private StringBuilder stringBuilder = new(512);

    private void OnDestroy()
    {
        CheckAnswerManager.Instance.OnAnswerChecked -= OnAnswerChecked;
    }

    private void Start()
    {
        CheckAnswerManager.Instance.OnAnswerChecked += OnAnswerChecked;
        stringBuilder.AppendLine("답란 : ");
        stringBuilder.AppendLine();

        text.SetText(stringBuilder);
    }


    private void OnAnswerChecked(byte[] numbers, bool[] answerCorrects, bool[] answerInaccurates)
    {
        for (int i = 0; i < CorrectNumber.digit; i++)
        {
            AddNumberAndCorrect(numbers[i], answerCorrects[i], answerInaccurates[i]);
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