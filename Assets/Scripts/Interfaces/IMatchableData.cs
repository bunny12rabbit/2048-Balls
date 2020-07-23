namespace Interfaces
{
    public interface IMatchableData<out T> where T : struct
    {
        T Criteria { get;}
        bool IsMatched { get; set; }

        void UpdateData();
    }
}