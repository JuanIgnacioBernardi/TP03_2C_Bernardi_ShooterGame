public interface IPooleable
{
    bool IsActive { get; set; }
    void Activate ();
    void DeActivate ();
}