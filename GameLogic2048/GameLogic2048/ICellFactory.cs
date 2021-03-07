namespace GameLogic2048
{
    public interface ICellFactory<T> where T:ICell, new()
    {
        T CreateCell(int value, bool doAnimate);
    }
}