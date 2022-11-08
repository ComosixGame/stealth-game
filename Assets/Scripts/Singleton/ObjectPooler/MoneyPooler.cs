using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using MyCustomAttribute;
#endif

public class MoneyPooler : Singleton<MoneyPooler>
{
    public int size;
    public GameObject prefab;
    private Queue<GameObject> objectPool = new Queue<GameObject>();
    #if UNITY_EDITOR
    [ReadOnly]
    #endif
    public int active, inactive;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start() {
        for(int i = 0; i< size; i++) {
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            inactive ++;
            objectPool.Enqueue(obj);
        }
    }

    public void SpawnMoney(Vector3 position, Quaternion rotation) {
        if(inactive <= 0) {
            GameObject obj = Instantiate(prefab, position, rotation);
            obj.transform.SetParent(transform);
            obj.SetActive(true);
            active ++;
            objectPool.Enqueue(obj);
            size ++;
        } else {
            GameObject obj = objectPool.Dequeue();
            Transform objTrans = obj.transform;
            objTrans.position = position;
            objTrans.rotation = rotation;
            obj.SetActive(true);
            active ++;
            inactive --;
            objectPool.Enqueue(obj);
        }
    }

    public void InactiveMoney(GameObject obj) {
        obj.SetActive(false);
        inactive ++;
        active --;
    }

}
