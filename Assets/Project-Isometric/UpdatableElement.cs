using System.Collections.Generic;

public class UpdatableElement
{
    private LinkedListNode<UpdatableElement> _linkedNode;
    
    public LinkedList<UpdatableElement> list
    {
        get
        { return _linkedNode != null ? _linkedNode.List : null; }
    }

    public bool activated
    {
        get
        { return _linkedNode != null ? _linkedNode.List != null : false; }
    }

    public UpdatableElement()
    {

    }

    public void Update(float deltaTime)
    {

    }

    public void OnAddByList()
    {

    }

    public void Remove()
    {
        if (activated)
            _linkedNode.List.Remove(_linkedNode);
    }
}