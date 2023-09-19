using TMPro;
using Unity.LEGO.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponHUB : MonoBehaviour
{
    [SerializeField] private GameObject canvas = default;
    [SerializeField] private GameObject parent = default;
    [SerializeField] private GameObject m_WeaponPrefab = default;
    [SerializeField] private GameObject infor = default;

    public delegate void SelectWeaponDelegate(int index);
    public SelectWeaponDelegate SelectWeapon;

    void Start()
    {
        canvas.SetActive(false);
        clean();
    }

    public void clean()
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Show()
    {
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.None;
#endif
        canvas.SetActive(true);

        OptionsMenuEvent evt = Events.OptionsMenuEvent;
        evt.Active = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventManager.Broadcast(evt);
    }

    public void Hide()
    {
#if !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
#endif
        canvas.SetActive(false);
        SelectWeapon = null;
        OptionsMenuEvent evt = Events.OptionsMenuEvent;
        evt.Active = false;
        EventSystem.current.SetSelectedGameObject(null);
        EventManager.Broadcast(evt);
    }

    private void SelectWeaponHandler(int index)
    {
        SelectWeapon?.Invoke(index);
    }

    public void AddWeapon(WeaponData data, int index)
    {
        var i = index;
        GameObject weapon = Instantiate(m_WeaponPrefab, parent.transform);
        weapon.GetComponent<Image>().sprite = data.m_Icon;
        EventTrigger.Entry eventtype_enter = new EventTrigger.Entry();
        eventtype_enter.eventID = EventTriggerType.PointerEnter;
        eventtype_enter.callback.AddListener((eventData) => {
            Debug.Log("Hover");
            infor.GetComponentInChildren<Image>().sprite = data.m_Icon;
            Text content = infor.GetComponentInChildren<Text>();
            content.text = string.Empty;
            content.text += "Name: " + data.m_name + "\n";
            content.text += "Price: " + data.m_Price.ToString() + "\n";
            content.text += "Sell price: " + data.m_SellPrice.ToString() + "\n";
            infor.SetActive(true);
        });

        EventTrigger.Entry eventtype_exit = new EventTrigger.Entry();
        eventtype_exit.eventID = EventTriggerType.PointerExit;
        eventtype_exit.callback.AddListener((eventData) => {
            infor.SetActive(false);
        });

        weapon.AddComponent<EventTrigger>();
        weapon.GetComponent<EventTrigger>().triggers.Add(eventtype_enter);
        weapon.GetComponent<EventTrigger>().triggers.Add(eventtype_exit);

        weapon.GetComponent<Button>().onClick.AddListener(() =>
        {
            SelectWeaponHandler(i);
        });
    }
}