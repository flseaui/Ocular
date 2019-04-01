using System;
using System.Collections.Generic;
using Priority_Queue;
using Sirenix.OdinInspector;

public class Node : FastPriorityQueueNode, IEquatable<Node>
{
    public Walkable Walkable;
    public List<Node> Neighbors;
    public bool Enabled;

    public Node(Walkable walkable)
    {
        Walkable = walkable;
        Neighbors = new List<Node>();
        Enabled = true;
    }

    public bool Equals(Node other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Walkable, other.Walkable);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Node) obj);
    }

    public override int GetHashCode() => (Walkable != null ? Walkable.GetHashCode() : 0);

    public static bool operator ==(Node left, Node right) => Equals(left, right);

    public static bool operator !=(Node left, Node right) => !Equals(left, right);
}
