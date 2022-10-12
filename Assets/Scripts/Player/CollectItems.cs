using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectItems : MonoBehaviour
{
    public LayerMask layer;
    private void Start() {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other) {
        GameObject item = other.gameObject;
        if((layer & (1 << item.layer)) != 0) {
            Transform itemTrans = item.transform;
            itemTrans.position = Vector3.MoveTowards(itemTrans.position, transform.position, 10f * Time.deltaTime);
        }
    }
}
