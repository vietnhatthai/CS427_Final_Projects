using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        void clean()
        {
            foreach (Transform child in parent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void Show()
        {
            canvas.SetActive(true);
            clean();
        }

        public void Hide()
        {
            canvas.SetActive(false);
            SelectWeapon = null;
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