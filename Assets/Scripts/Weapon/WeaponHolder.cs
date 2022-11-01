using UnityEngine;

public abstract class WeaponHolder : MonoBehaviour
{
    public Transform weaponHolder;

    public GameObject AddWeapon( GameObject weapon) {
        GameObject w = Instantiate(weapon, weaponHolder.position, weaponHolder.rotation);
        w.transform.SetParent(weaponHolder);
        return w;
    }
}

