﻿/*
The MIT License (MIT)

Copyright (c) 2018 Twan Veldhuis, Ivar Troost

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;
using System.Collections;

public class ObjectPool : MonoBehaviour {

    [SerializeField]
    int capacity;

    [SerializeField]
    int active;

    public bool clone = true;

	// Use this for initialization
	protected virtual void Start () {
        if (clone)
        {
            GameObject obj = transform.GetChild(0).gameObject;

            for (int i = 0; i < capacity - 1; i++)
            {
                var newObj = Instantiate(obj);
                newObj.SetActive(false);
                newObj.transform.SetParent(transform);
            }
        }
        else
        {
            capacity = transform.childCount;
        }
	}
	
    public virtual GameObject Spawn(Vector2 position, bool setActive)
    {
        if(active < capacity)
        {
            GameObject obj = transform.GetChild(active).gameObject;
            obj.SetActive(setActive);
            obj.transform.position = position;
            obj.tag = "Pooled";
            active++;
            return obj;
        }
        else
        {
            return null;
        }
    }

    public static GameObject Spawn(string poolName, Vector2 position, bool setActive)
    {
        ObjectPool pool = GameObject.Find(poolName).GetComponent<ObjectPool>();
        return pool.Spawn(position, setActive);
    }

    public static void Despawn(GameObject obj, string poolName)
    {
        GameObject pool_obj = GameObject.Find(poolName);
        if (pool_obj == null) return;
        ObjectPool pool = pool_obj.GetComponent<ObjectPool>();
        if(!(pool is ObjectContainer))
        {
            if (obj.tag != "Pooled")
                return; //already destroyed
            if (pool_obj != null)
            {
                if (obj.transform.IsChildOf(pool.transform))
                {
                    obj.transform.SetAsLastSibling();
                    obj.SetActive(false);
                    obj.tag = "Untagged";
                    pool.active--;
                    return;
                }
            }
        }
        Destroy(obj);
    }
}
