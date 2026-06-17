public static class GameEvents
{
    public static event System.Action<bool> onPauseChanged;
    public static void RaisePauseChanged(bool isPaused) => onPauseChanged?.Invoke(isPaused);
}