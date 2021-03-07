namespace GameLogic2048
{
    public interface ICell
    {
        int id { get; }
        int value { get; set; }
        int offsetX{ get; set; }
        int offsetY{ get; set; }
        bool isMultiply{ get; set; }
        bool isReadyToDestroy{ get; set; }
        
        bool isNew { get; set; }
    }
}