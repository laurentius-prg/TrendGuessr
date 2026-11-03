namespace TrendGuessr
{
    public class GameStateService
    {
        public int Streak { get; set; }
        public decimal Multiplier { get; set; } = 1;
        public bool ResultExists { get; set; } = false;
        public void ResetGameState()
        {
            Streak = 0;
            Multiplier = 1;
            ResultExists = false;
        }
        public void SetGameState(int streak, decimal multiplier, bool resultExists)
        {
            Streak = streak;
            Multiplier = multiplier;
            ResultExists = resultExists;
        }

    }
}
