/// <summary>
/// Interface to be implemented by objects that should be protected by the shield power up
/// </summary>
public interface IShieldable
{
    void OnRecievedShield();

    void OnLostShield();

    bool IsShielded();
}