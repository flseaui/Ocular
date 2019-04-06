using System;
using System.Collections.Generic;
using Level;
using Level.Objects;
using Priority_Queue;

namespace Player {
    public class Node : FastPriorityQueueNode, IEquatable<Node>
    {
        public bool Enabled;
        public readonly List<Node> Neighbors;
        public readonly Walkable Walkable;

        public Node(Walkable walkable)
        {
            Walkable = walkable;
            Neighbors = new List<Node>();
            Enabled = true;
        }

        public bool Equals(Node other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Equals(Walkable, other.Walkable);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Node) obj);
        }

        public override int GetHashCode() => Walkable != null ? Walkable.GetHashCode() : 0;

        public static bool operator == (Node left, Node right) => Equals(left, right);

        public static bool operator != (Node left, Node right) => !Equals(left, right);
    }
}