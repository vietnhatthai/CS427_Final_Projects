using Unity.LEGO.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponHUB : MonoBehaviour
{
    [SerializeField] private GameObject canvas = default;
    [SerializeField] private GameObject parent = default;
    [SerializeField] private GameObject m_WeaponPrefab = default;

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

    public void AddWeapon(Sprite sprite, int index)
    {
        var i = index;
        GameObject weapon = Instantiate(m_WeaponPrefab, parent.transform);
        weapon.GetComponent<Image>().sprite = sprite;
        weapon.GetComponent<Button>().onClick.AddListener(() =>
        {
            SelectWeaponHandler(i);
        });
    }
}