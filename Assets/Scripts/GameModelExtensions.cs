namespace Assets.Scripts
{
    public static class GameModelExtensions
    {
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