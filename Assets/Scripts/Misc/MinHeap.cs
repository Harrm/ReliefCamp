using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class MinHeap<T> where T : IComparable<T> {
    private T[] array;

    public MinHeap() {
        Count=0;
        array = new T[10];
    }

    public int Count {get; private set;}
    public int Size { get { return array.Length; } }

    private void balance() {
        for(int i = Count-1;i>0;--i) {
            int parent = (i-1)/2;
            if(array[i].CompareTo(array[parent]) <= 0) {
                var tmp = array[i];
                array[i]=array[parent];
                array[parent] = tmp;
            }
        }
    }

    private void check() {
        if(Count < 0)
            Count = 0;
        if(Count >= array.Length)
            resize();
    }

    private void resize() {
        var newArr = new T[array.Length * 2];
        for(int i = 0; i<Count; ++i)
            newArr[i]=array[i];
        array = newArr;
    }

    public void Add(T elem) {
        check();
        array[Count++] = elem;
        balance();
    }

    public T Peek() {
        if(!IsEmpty())
            return array[0];
        return default(T);
    }

    public T Pop() {
        if(IsEmpty())
            return default(T);
        var ret = array[0];
        array[0]=array[--Count];
        balance();
        return ret;
    }

    public T Replace(T elem) {
        if(IsEmpty()) {
            array[Count++]=elem;
            return default(T);
        }

        var ret = array[0];
        array[0]=elem;
        balance();
        return ret;
        
    }

    public void AddRange(List<T> elems) {
        foreach(var elem in elems) {
            check();
            array[Count++] = elem;
        }
        balance();
    }

    public bool IsEmpty() {
        return Count <= 0;
    }

}
