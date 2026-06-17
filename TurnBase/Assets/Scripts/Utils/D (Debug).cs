using UnityEngine;

public static class D
{
    // System.Diagnostics의 Conditional attribute 사용
    // Conditional은 소괄호 안 값이 정의되어 있지 않다면 해당 함수를 호출하는 코드를 컴파일하지 않음

    /// <summary>
    /// Debug.Log와 같은 코드 (빌드 시 최적화용)
    /// </summary>
    /// <param name="message">입력할 메세지</param>
    /// <param name="context">(선택적 매개 변수) 해당 로그가 발생한 오브젝트</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object message, Object context = null)
    {
        Debug.Log(message, context);
    }

    /// <summary>
    /// Debug.LogWarning와 같은 코드 (빌드 시 최적화용)
    /// </summary>
    /// <param name="message">입력할 메세지</param>
    /// <param name="context">(선택적 매개 변수) 해당 로그가 발생한 오브젝트</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(object message, Object context = null)
    {
        Debug.LogWarning(message, context);
    }

    /// <summary>
    /// Debug.LogError와 같은 코드 (빌드 시 최적화용)
    /// </summary>
    /// <param name="message">입력할 메세지</param>
    /// <param name="context">(선택적 매개 변수) 해당 로그가 발생한 오브젝트</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogError(object message, Object context = null)
    {
        Debug.LogError(message, context);
    }
}
