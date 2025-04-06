using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField]
    private Button btnHost;
    [SerializeField]
    private Button btnJoin;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private TextMeshProUGUI textLoading;
    [SerializeField]
    private TMP_InputField inputName;
    [SerializeField]
    private TextMeshProUGUI textAnnounce;

    void Awake()
    {
        ServiceLocator.Register(this);
        ResetMenu();
        btnHost.onClick.AddListener(() =>
        {
            ShowLoadingLayer();
            ServiceLocator.Get<NetworkManager>().StartGame(GameMode.Host);
        });

        btnJoin.onClick.AddListener(() =>
        {
            ShowLoadingLayer();
            ServiceLocator.Get<NetworkManager>().StartGame(GameMode.Client);
        });
        inputName.onEndEdit.AddListener(UpdateName);
        inputName.onSubmit.AddListener(UpdateName);
        inputName.text = StaticString.DefaultName;
        textAnnounce.text = "";
    }

    public void UpdateName(string name)
    {
        PlayerPrefs.SetString(PlayerPrefsKeys.PlayerName, name);
    }

    public void BackToMenu(string message = "")
    {
        ResetMenu();
        textAnnounce.text = message;
    }

    public void HideLoadingLayer()
    {
        menuPanel.SetActive(false);
    }

    public void ShowLoadingLayer(string message = "")
    {
        btnHost.gameObject.SetActive(false);
        btnJoin.gameObject.SetActive(false);
        inputName.gameObject.SetActive(false);
        textLoading.gameObject.SetActive(true);
        textLoading.text = message == "" ? StaticString.Loading : message;
        PlayerPrefs.SetString(PlayerPrefsKeys.PlayerName, inputName.text);
    }

    public void ResetMenu()
    {
        btnHost.gameObject.SetActive(true);
        btnJoin.gameObject.SetActive(true);
        inputName.gameObject.SetActive(true);
        textLoading.gameObject.SetActive(false);
        menuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
