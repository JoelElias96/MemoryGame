using System;

namespace MemoryGame
{
    public class MemoryGame
    {
        private Board m_Board; // Making Board a nested class would likely have been a better choice, but
        // as this is a hw assignment and splitting up the files makes it easier to edit them in git, we'll
        // leave them separated (and slightly more readable) for now.
        private GameType m_GameType;
        private int m_P1Score = 0;
        private int m_P2Score = 0;
        private bool m_IsP1sTurn = true;
        private Guess? m_PreviousChoiceThisTurn;
        private Random rand = new Random(); 

        public MemoryGame(GameType i_GameType, int i_BoardWidth, int i_BoardHeight)
        {
            m_Board = new Board(i_BoardWidth, i_BoardHeight);
            m_GameType = i_GameType;
        }

        public char[,] ReturnVisibleBoard()
        {
            const char uncoveredTile = ' '; // This is how we'll represent uncovered tiles.

            char[,] revealedBoard = new char[m_Board.m_BoardWidth, m_Board.m_BoardHeight];

            for (int i = 0; i < m_Board.m_BoardWidth; i++)
            {
                for (int j = 0; j < m_Board.m_BoardHeight; j++)
                {
                    if (m_Board.IsTileRevealed(i, j))
                    {
                        revealedBoard[i, j] = m_Board.m_BoardContents[i, j];
                    }
                    else
                    {
                        revealedBoard[i, j] = uncoveredTile;
                    }
                }
            }

            return revealedBoard;
        }

        public bool IsPlayer1Turn()
        {
            return m_IsP1sTurn;  // If it's not, we assume it's p2's turn.
        }

        // Returns the state of the board after the player's choice, but before anything was hidden.
        public char[,] InputPlayerChoice(int i_GuessX, int i_GuessY)
        {
            if (!IsValidGuess(i_GuessX, i_GuessY))
            {
                throw new Exception("Given Invalid Guess");
            }

            // If this is a valid choice, let's reveal the given space and remember how the board looked like.
            char revealedItem = m_Board.RevealTile(i_GuessX, i_GuessY);
            char[,] boardState = ReturnVisibleBoard(); 

            // Was there a different choice this turn? If there was, then this is our second guess.
            if (!m_PreviousChoiceThisTurn.HasValue)
            {
                // Then this is our first guess, good! Remember the guess.
                m_PreviousChoiceThisTurn = new Guess(i_GuessX, i_GuessY, revealedItem);
            }
            else
            {
                Guess prevChoice = m_PreviousChoiceThisTurn.Value;

                // This is our second guess. Was it a correct one?
                if (prevChoice.m_GuessValue == revealedItem)
                {
                    givePointToCurrentPlayer();
                }
                else
                {
                    // If the guess was wrong, recover the tiles we've shown
                    m_Board.HideTile(i_GuessX, i_GuessY);
                    m_Board.HideTile(prevChoice.m_GuessX, prevChoice.m_GuessY);
                }

                // Either way, switch turns.
                changeCurrentPlayer();
            }

            return boardState;
        }

        // This function returns false if the input is invalid for InputPlayerChoice.
        public bool IsValidGuess(int i_GuessX, int i_GuessY)
        {
            return (i_GuessX < m_Board.m_BoardWidth && i_GuessX >= 0 &&
                i_GuessY < m_Board.m_BoardHeight && i_GuessY >= 0 &&
                !m_Board.IsTileRevealed(i_GuessX, i_GuessY));
        }

        private void changeCurrentPlayer()
        {
            m_IsP1sTurn = !m_IsP1sTurn;
            m_PreviousChoiceThisTurn = null;
        }

        //Makes the AI take a random turn. Not very intellegent.
        public (int, int) computerPlay()
        {
            rand = new Random(); // Reshuffle the rng.
            bool[,] currentlyRevealedInBoard = m_Board.GetRevealedInBoard();

            int randomX = rand.Next(currentlyRevealedInBoard.GetLength(0));
            int randomY = rand.Next(currentlyRevealedInBoard.GetLength(1));

            // brute-force search through the entire board if the random spot isn't valid.
            while (currentlyRevealedInBoard[randomX, randomY] == true) // \0 is the default value that char initializes to.
            {
                randomX++;

                if (randomX >= currentlyRevealedInBoard.GetLength(0))
                {
                    randomX = 0;
                    randomY++;

                    if (randomY >= currentlyRevealedInBoard.GetLength(1))
                    {
                        randomY = 0;
                    }
                }
            }

            return (randomX, randomY);
        }

        // Returns the previous choice made this turn, if such a choice was made at all.
        // This is mainly for AI usage.
        internal Guess? GetPreviousChoiceThisTurn()
        {
            return m_PreviousChoiceThisTurn;
        }

        private void givePointToCurrentPlayer()
        {
            if (m_IsP1sTurn)
            {
                m_P1Score++;
            }
            else
            {
                m_P2Score++;
            }
        }

        public int GetP1Score()
        {
            return m_P1Score;
        }

        public int GetP2Score()
        {
            return m_P2Score;
        }

        // The game is over if we've revealed all tiles.
        public bool IsGameOver()
        {
            return m_Board.GetUncoveredTileCount() == 0;
        }

        public GameType GetGameType()
        {
            return m_GameType;
        }

        public static void Main()
        {
            // TODO - Maybe put some generic tests here?
        }
    }
}
