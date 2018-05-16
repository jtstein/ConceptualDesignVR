using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DCGElement
{
    public static int currentID = 0;
    public int elementID;   //Unique identifier for this element
    public int lastMoveID;  
    public bool isSelected; 
    public bool isLocked;
    public int playerLocked;
    public DCGElement lastCopyMade;

    public virtual void Render(Material mat) { }    //Called from OnRenderObject in DCGBase
    public virtual void Update() { }    //Called when an element associated with this one is updated
    public virtual void Remove() { }    //Called when this element is removed, should remove other elements that rely on this one
    public virtual bool ChildrenSelected() { return false; }    //Returns true iff all children of this element are selected
    public virtual float Distance(Vector3 position) { return Mathf.Infinity; } //Returns the distance of a dcg element to a given position
    public virtual DCGConstraint NearestConstraint(Vector3 position) { return new DCGConstraint(); }//Deprecated
    public virtual Vector3 ConstraintPosition(float[] constraintData) { return Vector3.zero; }//Deprecated
    public virtual List<Point> GetPoints() { return new List<Point>(); }//Returns all points that make up a given DCG object
    public virtual List<DCGElement> Extrude() { return new List<DCGElement>(); } //Extrude a DCGElement
    public virtual void Lock() { } //Is used to lock all elements below this element in the hierarchy.
    public virtual void Unlock() { } //Is used to unlock all elements below this element in the hierarchy.
    public virtual DCGElement Copy(int moveId = -1) { return new Point(new Vector3(0, 0, 0));  } //Returns a copy of a chosen DCGElement
    public virtual bool ParentSelected() { return false; }//Returns whether or not the parent of the element has been selected
    public virtual void RemoveChildren() { }
    public virtual List<DCGElement> GetParents() { return new List<DCGElement>(); }//Returns all parents of a given DCG
    public virtual List<DCGElement> GetChildren() { return new List<DCGElement>(); }

    public int nextElementID()
    {
        currentID += 32;
        return currentID;
    }
}
