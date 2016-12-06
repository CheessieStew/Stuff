using System;

//aa
public static class BTree
{
    public delegate Boolean Node(int who);

    public static Node Selector(Node[] actions)
    {
        return
            (who =>
            {
                Boolean res = false;
                foreach (Node action in actions)
                {
                    res = action(who);
                    if (res)
                        break;
                }
                return res;
            }
            );
    }

    public static Node Sequencer(Node[] actions)
    {
        return
            (who =>
            {
                Boolean res = false;
                foreach (Node action in actions)
                {
                    res = action(who);
                    if (!res)
                        break;
                }
                return res;
            }
            );
    }

    public static Node Not(Node action)
    {
        return (who => !(action(who)));
    }
}