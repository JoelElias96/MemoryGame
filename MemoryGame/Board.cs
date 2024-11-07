using System;

namespace MemoryGame
{
    internal class Board
    {
        private Random rand = new Random(); 
        internal char[,] m_BoardContents;
        private bool[,] m_RevealedInBoard;
        internal int m_BoardWidth;
        internal int m_BoardHeight;
        private int m_NumOfUncoveredTiles;
        
        public Board(int i_BoardWidth, int i_BoardHeight) 
        {
            // Sanity testing. Test this before inputting!
            if (i_BoardHeight <= 0 || i_BoardWidth <= 0 || i_BoardHeight * i_BoardWidth % 2 == 1)
            {
                throw new Exception("Invalid Board Requirements.");
            }

            m_BoardWidth = i_BoardWidth;
            m_BoardHeight = i_BoardHeight; 
            constructBoard();
            m_RevealedInBoard = new bool[i_BoardWidth, i_BoardHeight]; // All initialized to false, convenient.
        }

        public int GetUncoveredTileCount()
        {
            return m_NumOfUncoveredTiles;
        }

        public bool[,] GetRevealedInBoard()
        {
            return m_RevealedInBoard;
        }

        private void constructBoard()
        {
            m_BoardContents = new char[m_BoardWidth, m_BoardHeight];
            m_NumOfUncoveredTiles = m_BoardWidth * m_BoardHeight;

            // We are going to assume that we do not allow duplicates in the memory game.
            // (As in, four or more cards with the same symbol.)
            // Therefore, we'll go alphabetically over a list of symbols and use them to populate the grid.
            for (int i = 0;  i < m_NumOfUncoveredTiles/2; i++)
            {
                // We'll choose our symbol. Currently it's a fairly simple implementation that would go A B C D E ...
                char selectedSymbol = (char)((int)'A' + i);

                // We'll select two random locations on the board and populate with the symbol.
                for (int j = 0; j < 2; j++)
                {
                    (int randomX, int randomY) = randomUnpopulatedMatrixLocation();
                    m_BoardContents[randomX, randomY] = selectedSymbol;
                }
            }
        }

        public bool IsTileRevealed(int i_TileX, int i_TileY)
        {
            return m_RevealedInBoard[i_TileX, i_TileY];
        }

        public char RevealTile(int i_RevealX, int i_RevealY)
        {
            m_RevealedInBoard[i_RevealX, i_RevealY] = true;
            m_NumOfUncoveredTiles--;
            return m_BoardContents[i_RevealX, i_RevealY];
        }

        public char HideTile(int i_HideX, int i_HideY)
        {
            m_RevealedInBoard[i_HideX, i_HideY] = false;
            m_NumOfUncoveredTiles++;
            return m_BoardContents[i_HideX, i_HideY]; 
        }

        // Returns a random spot on the board that's set to null. I am implicitly assuming that such a spot exists,
        // as I only use this function within constructBoard.
        private (int, int) randomUnpopulatedMatrixLocation()
        {
            int randomX = rand.Next(m_BoardContents.GetLength(0));
            int randomY = rand.Next(m_BoardContents.GetLength(1));

            // brute-force search through the entire board if the random spot isn't valid.
            while (m_BoardContents[randomX, randomY] != '\0') // \0 is the default value that char initializes to.
            {
                randomX++;

                if (randomX >= m_BoardContents.GetLength(0))
                {
                    randomX = 0;
                    randomY++;

                    if (randomY >= m_BoardContents.GetLength(1))
                    {
                        randomY = 0;
                    }
                }
            }

            return (randomX, randomY); 
        }
    }
}
