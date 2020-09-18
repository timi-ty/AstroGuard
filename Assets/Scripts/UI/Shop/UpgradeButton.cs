using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class UpgradeButton : Button
{
    private event System.Action downEvent;
    private event System.Action<UpgradableListing> presssedEvent;
    private event System.Action upEvent;

    private UpgradableListing upgradableListing;

    public void Initialize(System.Action onDown, System.Action<UpgradableListing> onPresssed, System.Action onUp, 
        UpgradableListing upgradableListing, bool hasAstroGold)
    {
        downEvent = onDown;
        presssedEvent = onPresssed;
        upEvent = onUp;
        this.upgradableListing = upgradableListing;

        Animator pulseAnim = GetComponent<Animator>();
        pulseAnim.enabled = hasAstroGold;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (IsInteractable())
        {
            StartCoroutine(PressedCoroutine());
        }
    }

    private IEnumerator PressedCoroutine()
    {
        downEvent?.Invoke();

        while (Input.GetMouseButton(0))
        {
            presssedEvent?.Invoke(upgradableListing);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        upEvent?.Invoke();
    }
}
