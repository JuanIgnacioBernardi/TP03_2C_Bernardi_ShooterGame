public static class GameEvents
{
    public static bool IsGameOver { get; set; }

    public static event System.Action<bool> onPauseChanged;
    public static void RaisePauseChanged(bool isPaused) => onPauseChanged?.Invoke(isPaused);

    public static event System.Action onWin;
    public static void RaiseWin()
    {
        IsGameOver = true;
        onWin?.Invoke();
    }
    public static event System.Action onDeath;
    public static void RaiseDeath()
    {
        IsGameOver = true;
        onDeath?.Invoke();
    }

    public static event System.Action<CampController> onCampCleared;
    public static void RaiseCampCleared(CampController camp) => onCampCleared?.Invoke(camp);
}