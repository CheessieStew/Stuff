using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        z3_v1.Tree root = new z3_v1.TreeNode()
        {
            Left = new z3_v1.TreeNode()
            {
                Left = new z3_v1.TreeNode()
                {
                    Left = new z3_v1.TreeLeaf() { Value = 1 },
                    Right = new z3_v1.TreeLeaf() { Value = 2 },
                },
                Right = new z3_v1.TreeLeaf() { Value = 2 },
            },
            Right = new z3_v1.TreeLeaf() { Value = 3 }
        };
        Console.WriteLine(new z3_v1.DepthTreeVisitor().MeasureTree(root));

        z3_v2.Tree root2 = new z3_v2.TreeNode()
        {
            Left = new z3_v2.TreeNode()
            {
                Left = new z3_v2.TreeNode()
                {
                    Left = new z3_v2.TreeLeaf() { Value = 1 },
                    Right = new z3_v2.TreeLeaf() { Value = 2 },
                },
                Right = new z3_v2.TreeLeaf() { Value = 2 },
            },
            Right = new z3_v2.TreeLeaf() { Value = 3 }
        };
        Console.WriteLine(new z3_v2.DepthTreeVisitor().MeasureTree(root2));
        Console.ReadLine();
    }
}

namespace z3_v1
{
    public abstract class Tree
    {
        public virtual void Accept(TreeVisitor visitor) { }
    }
    public class TreeNode : Tree
    {
        public Tree Left;
        public Tree Right;
        public override void Accept(TreeVisitor visitor)
        {
            if (visitor.Order == TreeVisitor.VisitOrder.Prefix)
                visitor.VisitNode(this);
            if (Left != null)
                Left.Accept(visitor);
            if (visitor.Order == TreeVisitor.VisitOrder.Infix)
                visitor.VisitNode(this);
            if (Right != null)
                Right.Accept(visitor);
            if (visitor.Order == TreeVisitor.VisitOrder.Postfix)
                visitor.VisitNode(this);
        }
    }
    public class TreeLeaf : Tree
    {
        public int Value;

        public override void Accept(TreeVisitor visitor)
        {
            visitor.VisitLeaf(this);
        }
    }

    public abstract class TreeVisitor
    {
        public enum VisitOrder { Prefix, Infix, Postfix }
        public abstract VisitOrder Order { get; }
        public abstract void VisitNode(TreeNode node);
        public abstract void VisitLeaf(TreeLeaf leaf);
    }

    public class DepthTreeVisitor : TreeVisitor
    {
        public override VisitOrder Order
        {
            get
            {
                return VisitOrder.Prefix;
            }
        }
        Dictionary<Tree, int> levels = new Dictionary<Tree, int>();

        public int MeasureTree(Tree tree)
        {
            tree.Accept(this);
            return levels.Values.Max();
        }

        public override void VisitLeaf(TreeLeaf leaf)
        {
        }

        public override void VisitNode(TreeNode node)
        {
            var childrenLevel = 1;
            if (levels.Count == 0)
                levels.Add(node, 0);
            else
                childrenLevel = levels[node] + 1;
            if (node.Left != null)
                levels.Add(node.Left, childrenLevel);
            if (node.Right != null)
                levels.Add(node.Right, childrenLevel);
        }

    }


}


namespace z3_v2
{
    public abstract class Tree
    {
    }
    public class TreeNode : Tree
    {
        public Tree Left;
        public Tree Right;
    }
    public class TreeLeaf : Tree
    {
        public int Value;
    }

    public abstract class TreeVisitor
    {
        public enum VisitOrder { Prefix, Infix, Postfix }
        public abstract VisitOrder Order { get; }

        public void Visit(Tree tree)
        {
            if (tree is TreeNode)
                VisitNode((TreeNode)tree);
            if (tree is TreeLeaf)
                VisitLeaf((TreeLeaf)tree);
        }

        private void VisitNode(TreeNode node)
        {
            if (node != null)
            {
                if (Order == VisitOrder.Prefix)
                    ProcessNode(node);
                this.Visit(node.Left);
                if (Order == VisitOrder.Infix)
                    ProcessNode(node);
                this.Visit(node.Right);
                if (Order == VisitOrder.Postfix)
                    ProcessNode(node);
            }
        }

        private void VisitLeaf(TreeLeaf leaf)
        {
            ProcessLeaf(leaf);
        }


        protected abstract void ProcessLeaf(TreeLeaf leaf);

        protected abstract void ProcessNode(TreeNode node);
    }

    public class DepthTreeVisitor : TreeVisitor
    {
        public override VisitOrder Order
        {
            get
            {
                return VisitOrder.Prefix;
            }
        }
        Dictionary<Tree, int> levels = new Dictionary<Tree, int>();

        public int MeasureTree(Tree tree)
        {
            Visit(tree);
            return levels.Values.Max();
        }

        protected override void ProcessLeaf(TreeLeaf leaf)
        {

        }

        protected override void ProcessNode(TreeNode node)
        {
            var childrenLevel = 1;
            if (levels.Count == 0)
                levels.Add(node, 0);
            else
                childrenLevel = levels[node] + 1;
            if (node.Left != null)
                levels.Add(node.Left, childrenLevel);
            if (node.Right != null)
                levels.Add(node.Right, childrenLevel);
        }

    }


}
