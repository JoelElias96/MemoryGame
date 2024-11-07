namespace MemoryGame
{
    internal struct Guess
    {
        internal int m_GuessX;
        internal int m_GuessY;
        internal char m_GuessValue;

        internal Guess(int i_GuessX, int i_GuessY, char i_GuessValue)
        { 
            m_GuessX = i_GuessX; 
            m_GuessY = i_GuessY; 
            m_GuessValue = i_GuessValue;
        }
    }
    public enum GameType
    {
        VsPlayer,
        VsComputer
    }
}
