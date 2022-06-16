using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmamentHUDSlotManager : MonoBehaviour
{
    [SerializeField]
    private SO_DeveloperToolsSettings devTools = null;

    [SerializeField]
    private PSO_CurrentLoadout currLoadout = null;

    [SerializeField]
    private List<RectTransform> orderedWeaponSlots = new List<RectTransform>();

    [SerializeField]
    private List<RectTransform> orderedEquipmentSlots = new List<RectTransform>();

    private void Start()
    {
        PopulateArmamentIconSlots(orderedWeaponSlots, currLoadout.Value.OrderedWeapons);
        PopulateArmamentIconSlots(orderedEquipmentSlots, currLoadout.Value.OrderedEquipments);
    }

    private void PopulateArmamentIconSlots<T>(List<RectTransform> slotList, List<LoadoutWrapper<T>> armamentLoadout) where T : AArmament
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (i >= armamentLoadout.Count)
            {
                continue;
            }

            bool armamentLocked = !devTools.AllArmamentsAvailable
            && (armamentLoadout[i].LockedPSO?.Value ?? false);

            if (armamentLocked)
            {
                continue;
            }

            AArmament armament = armamentLoadout[i].Armament;

            Destroy(slotList[i].GetChild(0).gameObject);

            Instantiate(armament.ArmamentHUDIconPrefab, slotList[i]);
        }
    }
}
