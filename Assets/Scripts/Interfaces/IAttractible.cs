using UnityEngine;
/// <summary>
/// Interface to be implemented by any object that can be affected by an attraction field. 
/// </summary>
public interface IAttractible
{
    bool isInAttractionField { get; set; }

    void OnEnterAttractionField();

    void RecieveAttractionForce(Vector2 sourcePoint, float forceMagnitude);

    void OnExitAttractionField();
}
