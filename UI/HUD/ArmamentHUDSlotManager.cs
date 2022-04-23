using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmamentHUDSlotManager : MonoBehaviour
{
    [SerializeField]
    private PSO_CurrentLoadout currLoadout = null;

    [SerializeField]
    private List<RectTransform> orderedWeaponSlots = new List<RectTransform>();

    [SerializeField]
    private List<RectTransform> orderedEquipmentSlots = new List<RectTransform>();

    private void Start()
    {
        List<AArmament> weapons = currLoadout.Value.OrderedWeapons.ConvertAll(w => (AArmament)w);
        List<AArmament> equipments = currLoadout.Value.OrderedEquipments.ConvertAll(e => (AArmament)e);

        PopulateArmamentIconSlots(orderedWeaponSlots, weapons);
        PopulateArmamentIconSlots(orderedEquipmentSlots, equipments);
    }

    private void PopulateArmamentIconSlots(List<RectTransform> slotList, List<AArmament> armamentLoadout)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (i >= armamentLoadout.Count) { continue; }

            AArmament armament = armamentLoadout[i];

            if (armament == null) { continue; }

            Destroy(slotList[i].GetChild(0).gameObject);

            Instantiate(armament.ArmamentHUDIconPrefab, slotList[i]);
        }
    }
}
