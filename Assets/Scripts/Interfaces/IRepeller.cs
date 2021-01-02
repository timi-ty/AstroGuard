using UnityEngine;
using System.Collections;

public interface IRepeller
{
    bool isRepelling { get; set; }

    void OnStartRepelling();

    void OnStopRepelling();
}
