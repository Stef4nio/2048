namespace Assets.Scripts
{
    public static class GameModelExtensions
    {
        /// <summary>
        /// An extension method for modelCell matrices, to check their equality
        /// </summary>
        /// <param name="array1">First matrix</param>
        /// <param name="array2">Second matrix</param>
        /// <returns>True if matrices are equal, false otherwise</returns>
        public static bool IsEqual(this ModelCell[,] array1, ModelCell[,] array2)
        {
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    if (array1[i, j] == null && array2[i, j] == null)
                    {
                        continue;
                    }

                    if (array1[i, j] == null ^ array2[i, j] == null)
                    {
                        return false;
                    }

                    if (array1[i, j].id != array2[i, j].id)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Converts this project's Direction to a gameLogic .dll Direction
        /// </summary>
        /// <param name="direction">This project direction</param>
        /// <returns>GameLogic .dll direction </returns>
        public static GameLogic2048.Direction ToGameLogicDirection(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return GameLogic2048.Direction.Up;
                case Direction.Down:
                    return GameLogic2048.Direction.Down;
                case Direction.Left:
                    return GameLogic2048.Direction.Left;
                case Direction.Right:
                    return GameLogic2048.Direction.Right;
            }
            return GameLogic2048.Direction.None;
        }
    }
}