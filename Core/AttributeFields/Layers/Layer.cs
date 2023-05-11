using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
[System.Serializable]
public struct Layer
{
    [SerializeField]
    int m_layer;
    
    public int Index { get { return m_layer; } set { m_layer = value; } }
    public int Mask { get { return 1 << m_layer; } }
    public string Name
    {
        get { return LayerMask.LayerToName(m_layer); }
        set { m_layer = LayerMask.NameToLayer(value); }
    }
    
    public static implicit operator int(Layer l)
    {
        return l.Index;
    }
    
    public static implicit operator Layer(int i)
    {
        return new Layer() { Index = i };
    }
    
    public Layer(int index)
    {
        m_layer = index;
    }
    public Layer(string name)
    {
        m_layer = LayerMask.NameToLayer(name);
    }
}
