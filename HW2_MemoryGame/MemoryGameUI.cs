using MemoryGame;
using System;
using System.Linq;

namespace HW2_MemoryGame
{
    public class MemoryGameUI
    {
        const int k_TimeOfPauseInMiliseconds = 750;
        public MemoryGame.MemoryGame m_Game;
        public string[] m_PlayerNames = new string[2];

        public static void Main()
        {
            MemoryGameUI UI = new MemoryGameUI();
            UI.RunGame();
        }

        public void RunGame()
        {
            Console.WriteLine("Hello, Welcome to the memory game!");

            GameType gameType = getGameTypeFromUser();

            m_PlayerNames[0] = getValidPlayerName();

            if (gameType == GameType.VsPlayer)
            {
                m_PlayerNames[1] = getValidPlayerName();
            }
            else
            {
                m_PlayerNames[1] = "the Computer";
            }

            createGameBoard(gameType);

            while (!m_Game.IsGameOver())
            {
                printBoardToScreen();

                if (m_Game.GetGameType() == GameType.VsComputer && !m_Game.IsPlayer1Turn())
                {
                    letComputerPlay();
                }
                else
                {
                    letCurrentUserPlay();
                } 
            }

            printVictoryScreen();
            Console.Read();
        }

        private static GameType getGameTypeFromUser()
        {
            bool currentInputIsValid = false;
            GameType gameType = new GameType();
            Console.WriteLine("Please enter the game mode you want to play:\n[P]layer, [C]omputer");
            String gameModeInput = Console.ReadLine();

            while (!currentInputIsValid)
            {
                gameModeInput = gameModeInput.ToLower();

                if (gameModeInput == "p" || gameModeInput == "player")
                {
                    gameType = GameType.VsPlayer;
                    currentInputIsValid = true;
                }
                else if (gameModeInput == "c" || gameModeInput == "computer")
                {
                    gameType = GameType.VsComputer;
                    currentInputIsValid = true;
                }
                else
                {
                    Console.WriteLine("Invalid Input!");
                    gameModeInput = Console.ReadLine();
                }
            }

            return gameType;
        }

        private static int getPositiveIntFromUser(string i_RequestName)
        {
            Console.WriteLine("Please input {0}:", i_RequestName);
            string inputNum = Console.ReadLine();

            //keeps trying until we get a non-zero positive integer
            while (!int.TryParse(inputNum, out int value) || int.Parse(inputNum) < 0)
            {
                Console.WriteLine("Please insert a positive integer");
                inputNum = Console.ReadLine();
            }

            return int.Parse(inputNum);
        }

        private string getGuessFromUser()
        {
            bool currentInputIsValid = false;
            string userGuess = "";
            Console.WriteLine("{0}, please input your guess:", m_Game.IsPlayer1Turn() ? m_PlayerNames[0] : m_PlayerNames[1]);

            while (!currentInputIsValid)
            {
                String gameModeInput = Console.ReadLine().ToLower();
                if (gameModeInput.Length == 2 &&
                    Char.ToLower(gameModeInput[0]) >= 'a' && Char.ToLower(gameModeInput[1]) <= 'z' &&
                    gameModeInput[1] >= '0' && gameModeInput[1] <= '9')
                {
                    int convertGuessX, convertGuessY;
                    (convertGuessX, convertGuessY) = convertGuess(gameModeInput);

                    if (m_Game.IsValidGuess(convertGuessX, convertGuessY))
                    {
                        userGuess = gameModeInput;
                        currentInputIsValid = true;
                    }

                    else
                    {
                        Console.WriteLine("Invalid input. The given guess is already revealed, please try again:");
                    }
                }

                else
                {
                    Console.WriteLine("Invalid input. The guess should be an uppercase letter followed by a digit (e.g., A1), please try again:");
                }
            }

            return userGuess;
        }

        // Converts guess from ASCII to the appropriate numbers for MemoryGame to handle.
        private (int, int) convertGuess(string i_Guess)
        {
            int char_to_int = i_Guess[0] - 'a';
            int num_to_int = i_Guess[1] - '1'; 
            return (char_to_int, num_to_int);
        }

        private void printBoardToScreen()
        {
            char[,] visibleBoard = m_Game.ReturnVisibleBoard();
            printBoardToScreen(visibleBoard);
        }

        private void printBoardToScreen(char[,] i_Board)
        {
            // These two are flexible incase we want to change it later, get fancy and all.
            // Also we assume the font used is monospaced.
            const char horizontalSeparator = '=';
            const char verticalSeparator = '|';

            Ex02.ConsoleUtils.Screen.Clear();

            int boardWidth = i_Board.GetLength(0);
            int boardHeight = i_Board.GetLength(1);

            // Print headers
            Console.Write("  {0}", verticalSeparator);
            for (int i = 0; i < boardWidth; i++)
            {
                Console.Write(" {0} {1}", (char)('A' + i), verticalSeparator);
            }
            Console.WriteLine();

            // Print board
            for (int i = 0; i < boardHeight; i++)
            {
                printHorizontalSeparator(horizontalSeparator, boardWidth);
                Console.Write("{0} ", i + 1);

                for (int j = 0; j < boardWidth; j++)
                {
                    Console.Write("{0} {1} ", verticalSeparator, i_Board[j, i]);
                }
                Console.WriteLine(verticalSeparator);
            }
            printHorizontalSeparator(horizontalSeparator, boardWidth);
        }

        private void printHorizontalSeparator(char i_horizontalSeparator, int i_Width) 
        {
            for (int i = 0; i < (i_Width * 4 + 3); i++)
            {
                Console.Write(i_horizontalSeparator);
            }

            Console.WriteLine();
        }

        //Gets from the user a valid user name without spaces
        private static String getValidPlayerName()
        {
            String nameInput = " ";

            while (nameInput.Contains(' '))
            {
                Console.WriteLine("Please enter your user-name with no spaces:");
                nameInput = Console.ReadLine();
            }

            return nameInput;
        }

        private void createGameBoard(GameType gameType)
        {
            bool boardSizeValid = false; 

            while (!boardSizeValid)
            {
                int boardWidth = getPositiveIntFromUser("the board's width");
                int boardHeight = getPositiveIntFromUser("the board's height");

                try
                {
                    m_Game = new MemoryGame.MemoryGame(gameType, boardWidth, boardHeight);
                    boardSizeValid = true; // Will only run if the previous line did not run an exception.
                }
                catch (Exception e)
                {
                    // It is assumed that we caught the "Invalid Board Size" exception.
                    Console.WriteLine("The board cannot have both odd width and height! One must be even.");
                }
            }
        }

        private void printVictoryScreen()
        {
            Ex02.ConsoleUtils.Screen.Clear();

            string endingText = "Ending text undefined! Please call one of the programmers!";
            int p1Score = m_Game.GetP1Score();
            int p2Score = m_Game.GetP2Score();

            if (p1Score > p2Score)
            {
                endingText = String.Format("The winner is {0}, with {1} points!", m_PlayerNames[0], p1Score);
            }
            else if (p1Score < p2Score)
            {
                endingText = String.Format("The winner is {0}, with {1} points!", m_PlayerNames[1], p2Score);
            }
            else
            {
                endingText = "It's a tie! Good job to you both!";
            }

            Console.WriteLine("Game Over!\n{0}\nPress enter to close the game.", endingText);
        }

        private void letCurrentUserPlay()
        {
            int userGuessX = -1;
            int userGuessY = -1;

            while (!m_Game.IsValidGuess(userGuessX, userGuessY))
            {
                (userGuessX, userGuessY) = convertGuess(getGuessFromUser());
            }

            System.Threading.Thread.Sleep(k_TimeOfPauseInMiliseconds);
            printBoardToScreen(m_Game.InputPlayerChoice(userGuessX, userGuessY));
            System.Threading.Thread.Sleep(k_TimeOfPauseInMiliseconds * 2);
        }

        private void letComputerPlay()
        {
            int computerGuessX, computerGuessY;
            string computerMessage = "Computer thinking...";

            Console.WriteLine(computerMessage);
            System.Threading.Thread.Sleep(k_TimeOfPauseInMiliseconds);

            // The computer plays two turns and returns control.
            for (int i = 0; i < 2; i++)
            {
                (computerGuessX, computerGuessY) = m_Game.computerPlay();
                printBoardToScreen(m_Game.InputPlayerChoice(computerGuessX, computerGuessY));
                Console.WriteLine(computerMessage);
                System.Threading.Thread.Sleep(k_TimeOfPauseInMiliseconds * 2);
            }
        }
    }
}
