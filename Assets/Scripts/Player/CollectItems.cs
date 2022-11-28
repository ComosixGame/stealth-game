using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectItems : MonoBehaviour
{
    public LayerMask layer;
    private Transform itemTrans;
    private GameObject item;

    private void OnTriggerStay(Collider other) {
        item = other.gameObject;
        if((layer & (1 << item.layer)) != 0) {
            if(!item.GetComponent<CurrencyBonus>().useMagnet) return;
            itemTrans = item.transform;
            itemTrans.position = Vector3.Lerp(itemTrans.position, transform.position, 3f * Time.deltaTime);
        }
    }
}
