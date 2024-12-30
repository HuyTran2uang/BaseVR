using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    public string SkuName;
    public Button buyButton;

    private void Awake()
    {
        buyButton.onClick.AddListener(Buy);
    }

    private void Buy()
    {
        OculusIAP.Instance.Buy(SkuName);
    }
}
