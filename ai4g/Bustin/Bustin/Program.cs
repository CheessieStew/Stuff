using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

//would be split into many files if CodinGame would allow it

#region Management
//mostly information management and IO
public partial class Game
{
    static void Main(string[] args)
    {
        int bustersPerPlayer = int.Parse(Console.ReadLine()); // the amount of busters you control
        int ghostCount = int.Parse(Console.ReadLine()); // the amount of ghosts on the map
        int myTeamId = int.Parse(Console.ReadLine()); // if this is 0, your base is on the top left of the map, if it is one, on the bottom right

        Game game = new Game(bustersPerPlayer, ghostCount, myTeamId);

        while (true)
        {
            game.AgeAllInfo();
            game.GetGameInfo();
            for (int i = 0; i < bustersPerPlayer; i++)
            {
                game.currentBuster = i;
                game.eachOnHisOwnTree();
            }
            game.FollowOrders();
        }
    }

    public static readonly int xSize = 16001;
    public static readonly int ySize = 9001;
    public static readonly int bigNumber = 3000;

    //game info
    int bustersPerPlayer;
    int ghostCount;
    int myTeamId;
    int myBaseX;
    int myBaseY;
    Entity[] myTeam;
    Entity[] enemyTeam;
    Entity[] ghosts;

    Game(int bpp, int gc, int mtid)
    {
        bustersPerPlayer = bpp;
        ghostCount = gc;
        myTeamId = mtid;
        myTeam = new Entity[bustersPerPlayer];
        orders = new string[bustersPerPlayer];

        helpers = new int[bustersPerPlayer];
        helping = new int[bustersPerPlayer];

        myBaseX = myTeamId == 0 ? 0 : xSize - 1;
        myBaseY = myTeamId == 0 ? 0 : ySize - 1;

        enemyTeam = new Entity[bustersPerPlayer];
        for (int i = 0; i < bustersPerPlayer; i++)
        {
            helpers[i] = 0;
            helping[i] = -1;
            enemyTeam[i].infoAge = bigNumber;
            enemyTeam[i].stunCooldown = 0;
            myTeam[i].stunCooldown = 0;
            myTeam[i].usedRadar = false;
        }

        ghosts = new Entity[ghostCount];
        for (int i = 0; i < ghostCount; i++)
        {
            ghosts[i].infoAge = bigNumber;
        }

        patrolInfo = new int[gridXSize, gridYSize];
        for (int i = 0; i < gridXSize; i++)
        {
            for (int j = 0; j < gridYSize; j++)
                patrolInfo[i, j] = bigNumber;
        }
        eachOnHisOwnTree = EachOnHisOwnTree();
    }

    void AgeAllInfo()
    {
        for (int i = 0; i < gridXSize; i++)
        {
            for (int j = 0; j < gridYSize; j++)
            {
                if (patrolInfo[i, j] < bigNumber)
                    patrolInfo[i, j]++;
            }
        }
        for (int i = 0; i < bustersPerPlayer; i++)
        {
            if (enemyTeam[i].infoAge < bigNumber)
                enemyTeam[i].infoAge++;
            if (myTeam[i].stunCooldown > 0)
                myTeam[i].stunCooldown--;
            if (enemyTeam[i].stunCooldown > 0)
                enemyTeam[i].stunCooldown--;
        }
        for (int i = 0; i < ghostCount; i++)
        {
            if (ghosts[i].infoAge < bigNumber)
                ghosts[i].infoAge++;
        }
    }

    void GetGameInfo()
    {
        int entities = int.Parse(Console.ReadLine()); // the number of busters and ghosts visible to you
        for (int i = 0; i < entities; i++)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int entityId = int.Parse(inputs[0]); // buster id or ghost id
            int x = int.Parse(inputs[1]);
            int y = int.Parse(inputs[2]); // position of this buster / ghost
            int entityType = int.Parse(inputs[3]); // the team id if it is a buster, -1 if it is a ghost.
            int state = int.Parse(inputs[4]); // For busters: 0=idle, 1=carrying a ghost.
            int value = int.Parse(inputs[5]); // For busters: Ghost id being carried. For ghosts: number of busters attempting to trap this ghost.

            if (entityType >= 0 && entityId >= bustersPerPlayer)
            {
                entityId -= bustersPerPlayer;
            }

            if (entityType == myTeamId)
            {
                myTeam[entityId].xPos = x;
                myTeam[entityId].yPos = y;
                myTeam[entityId].value = value;
                patrolInfo[x / gridCellSize, y / gridCellSize] = 0;
                if (myTeam[entityId].state != 2 && state == 2)
                {
                    currentBuster = entityId;
                    EnemyInRange();
                    enemyTeam[closestEnemy].stunCooldown = 20;
                }
                myTeam[entityId].state = state;
            }
            else if (entityType == 1 - myTeamId)
            {
                enemyTeam[entityId].xPos = x;
                enemyTeam[entityId].yPos = y;
                enemyTeam[entityId].state = state;
                enemyTeam[entityId].value = value;
                enemyTeam[entityId].infoAge = 0;
            }
            else
            {
                ghosts[entityId].xPos = x;
                ghosts[entityId].yPos = y;
                ghosts[entityId].state = state;
                ghosts[entityId].infoAge = 0;
            }
        }
    }

    public void FollowOrders()
    {
        for (int i = 0; i < bustersPerPlayer; i++)
        {
            Console.WriteLine(orders[i]);
        }
    }
}
#endregion

#region DecisionMaking

public partial class Game
{
    public static readonly int startingInterestRange = 7000;
    public static readonly int maxInterestIncrease = 12000;
    public static readonly int gridCellSize = 1000;
    public static readonly int gridXSize = xSize / gridCellSize + 1;
    public static readonly int gridYSize = ySize / gridCellSize + 1;
    public static readonly int minValueForRadar = 10000;
    public static readonly int escortChange = 8000;
    public int interestRange = startingInterestRange;
    public int maxAggroThiefChange = 8000;
    public int aggresiveness = 3000;
    public int thievery = 4500;
    public int cowardiness = 2000;
    public int shortestEscortDistance = 10000;

    int myScore = 0;
    int currentBuster;
    int closestGhost;
    int closestEnemy;
    int allyInNeed;
    BTree.Node eachOnHisOwnTree;
    String[] orders;
    int[,] patrolInfo;
    int[] helpers;
    int[] helping;

    bool CarriesGhost()
    {
        return (myTeam[currentBuster].carrying);
    }

    bool GoHome()
    {
        orders[currentBuster] = $"MOVE {myBaseX} {myBaseY} Going home...";
        return true;
    }

    bool Score()
    {
        if (myTeam[currentBuster].DistanceTo(myBaseX, myBaseY) <= 1600)
        {
            interestRange += maxInterestIncrease / ghostCount;
            aggresiveness -= maxAggroThiefChange / ghostCount;
            if (aggresiveness <= 500)
                aggresiveness = 500;
            shortestEscortDistance -= escortChange / ghostCount;
            thievery += maxAggroThiefChange / ghostCount;
            myScore++;
            orders[currentBuster] = "RELEASE Fly, little one!";
            return true;
        }

        return false;
    }

    bool GhostInRange()
    {
        int minDist = int.MaxValue;
        int staminaOnMinDist = 1;
        int trueMinDist = int.MaxValue;
        for (int i = 0; i < ghostCount; i++)
        {
            if (ghosts[i].infoAge > 50)
                continue;

            int localDist = ghosts[i].DistanceTo(myTeam[currentBuster]);

            if (localDist <= 2200 && ghosts[i].infoAge > 0)
            {
                ghosts[i].infoAge = bigNumber;
                continue;
            }

            int trueDist = localDist;
            if (ghosts[i].value > 0)
                localDist /= 2;
            if (localDist < int.MaxValue)
                localDist += ghosts[i].infoAge * localDist / 50;
            if (minDist == int.MaxValue || localDist * (ghosts[i].state + 1) < minDist * (staminaOnMinDist + 1))
            {
                minDist = localDist;
                trueMinDist = trueDist;
                staminaOnMinDist = ghosts[i].state;
                closestGhost = i;
            }
        }
        return (trueMinDist <= interestRange);
    }

    bool BustClosestGhost()
    {
        int dist = ghosts[closestGhost].DistanceTo(myTeam[currentBuster]);
        if (dist <= 1760 && dist >= 900)
        {
            if (ghosts[closestGhost].infoAge > 0)
            {
                ghosts[closestGhost].infoAge = bigNumber;
                return false;
            }
            orders[currentBuster] = $"BUST {closestGhost} Busting... {ghosts[closestGhost].DistanceTo(myTeam[currentBuster])}";
            ghosts[closestGhost].state--;
            ghosts[closestGhost].value++;
            if (ghosts[closestGhost].state <= 0)
            {
                ghosts[closestGhost].infoAge = bigNumber;
            }
            return true;
        }
        return false;
    }

    bool MoveToClosestGhost()
    {
        if (ghosts[closestGhost].infoAge == bigNumber)
            return false;

        double xmod = xSize - myBaseX - ghosts[closestGhost].xPos;
        double ymod = ySize - myBaseY - ghosts[closestGhost].yPos;
        double norm = Math.Sqrt(Math.Pow(xmod, 2) + Math.Pow(ymod, 2));
        xmod *= 900d / norm;
        ymod *= 900d / norm;
        orders[currentBuster] = $"MOVE {ghosts[closestGhost].xPos + (int)xmod} {ghosts[closestGhost].yPos + (int)ymod} Cm'ere, damnit!";

        return true;
    }

    bool CanStun()
    {
        return (myTeam[currentBuster].stunCooldown == 0);
    }

    bool EnemyInRange()
    {
        int dist = int.MaxValue;
        int range = aggresiveness;
        bool thief = false;
        for (int i = 0; i < bustersPerPlayer; i++)
        {
            if (enemyTeam[i].infoAge > 25 || enemyTeam[i].stunned)
                continue;

            int localDist = enemyTeam[i].DistanceTo(myTeam[currentBuster]);
            if (localDist <= 2200 && enemyTeam[i].infoAge > 0)
            {
                enemyTeam[i].infoAge = bigNumber;
                continue;
            }
            if (enemyTeam[i].state == 1 && thief == false)
            {
                    dist = localDist;
                    closestEnemy = i;
                    thief = true;
                    range = thievery;

            }
            if (enemyTeam[i].state != 1 && thief == true)
                continue;


            if (localDist < int.MaxValue)
                localDist += enemyTeam[i].infoAge * localDist / 50;
            if (localDist < dist)
            {
                dist = localDist;
                closestEnemy = i;
            }
        }
        bool shouldIgnore = true;
        if (enemyTeam[closestEnemy].carrying)
            shouldIgnore = false;
        else
        {
            foreach (Entity ghost in ghosts)
                if (ghost.state < 10 && ghost.DistanceTo(enemyTeam[closestEnemy]) <= 1800)
                    shouldIgnore = false;
        }
        if (shouldIgnore)
            return false;
        int distance = myTeam[currentBuster].DistanceTo(xSize-myBaseX,ySize-myBaseY) - enemyTeam[closestEnemy].DistanceTo(xSize - myBaseX, ySize - myBaseY);
        if (distance > 1200)
            return false;
        return (dist <= range / (myTeam[currentBuster].carrying ? 2 : 1));

    }

    bool StunClosestEnemy()
    {

        int dist = enemyTeam[closestEnemy].DistanceTo(myTeam[currentBuster]);
        if (enemyTeam[closestEnemy].infoAge > 0 || enemyTeam[closestEnemy].stunned)
            return false;
        if (dist < 1760)
        {
            if (enemyTeam[closestEnemy].carrying)
                orders[currentBuster] = $"STUN {closestEnemy + bustersPerPlayer * (1- myTeamId)} Gimme that!";
            else
                orders[currentBuster] = $"STUN {closestEnemy + bustersPerPlayer * (1 - myTeamId)} Nighty night.";
            enemyTeam[closestEnemy].state = 2;
            myTeam[currentBuster].stunCooldown = 20;
            return true;
        }
        return false;
    }

    bool MoveToClosestEnemy()
    {
        orders[currentBuster] = $"MOVE {enemyTeam[closestEnemy].xPos} {enemyTeam[closestEnemy].yPos} I WILL BE YOUR DOOM";
        return true;
    }

    int PatrolRating(int x, int y, int dirX, int dirY, int depth)
    {
        double rating = 0;
        if (!(x + dirX < gridXSize && y+dirY < gridYSize && x + dirX >= 0 && y + dirY >= 0))
            return 0;
        if (dirX == 0 || dirY == 0)
        {
            for (int layer = 1; layer <= depth; layer++)
            {
                int fields = 0;
                int sum = 0;
                for (int field = -layer; field <= layer; field++)
                {
                    int xcd = x + dirX * layer + dirY * field;
                    int ycd = y + dirY * layer + dirX * field;
                    if (xcd < gridXSize && xcd >= 0 && ycd < gridYSize && ycd >= 0)
                    {
                        sum += patrolInfo[xcd, ycd];
                        fields++;
                    }
                }
                if (fields>0)
                    rating += sum * (depth- layer + 1.0d)/depth / fields;
            }
        }
        else
        {
            for (int layer = 0; layer <= depth; layer++)
            {
                int fields = 0;
                int sum = 0;
                for (int field = -layer; field <= layer; field++)
                {
                    int xcd = x + dirX * layer;
                    int ycd = y + dirY * layer;
                    if (field < 0)
                        xcd += dirX * field;
                    else
                        ycd -= dirX * field;
                    if (xcd < gridXSize && xcd >= 0 && ycd < gridYSize && ycd >= 0)
                    {
                        sum += patrolInfo[xcd, ycd];
                        fields++;
                    }
                }
                if (fields > 0)
                    rating += sum * (depth - layer + 1.0d) / depth / fields;
            }
        }
        return (int)rating;
    }

    bool Patrol()
    {

        int xchunk = (myTeam[currentBuster].xPos) / gridCellSize;
        int ychunk = (myTeam[currentBuster].yPos) / gridCellSize;
        int bestScore = 0;
        int targetX = 0;
        int targetY = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {

                int score = PatrolRating(xchunk, ychunk, j, i, 6);
                if (i == 0 && j == 0)
                {
                    continue;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    targetX = j;
                    targetY = i;
                }
            }
        }

        patrolInfo[xchunk + targetX, ychunk + targetY] = 0;
        orders[currentBuster] = $"MOVE {(xchunk + targetX) * gridCellSize + gridCellSize / 2} {(ychunk + targetY) * gridCellSize + gridCellSize / 2} *whistles*";
        return true;
    }

    bool EnemyCanStun()
    {
        foreach (Entity enemy in enemyTeam)
        {
            if (enemy.DistanceTo(myTeam[currentBuster]) <= aggresiveness && enemy.stunCooldown <= 0)
                return true;
        }
        return false;
    }

    bool Camp()
    {
        if (myScore > ghostCount * 4 / 10)
        {
            int xtarget = xSize - myBaseX;
            int ytarget = ySize - myBaseY;
            double lowerAngle = Math.PI / 9;
            double upperAngle = Math.PI / 2;
            double myAngle = lowerAngle + (upperAngle - lowerAngle) / bustersPerPlayer * currentBuster;

            double xmod = Math.Cos(myAngle) * (myTeamId == 0 ? -2500 : 2500);
            double ymod = Math.Sin(myAngle) * (myTeamId == 0 ? -2500 : 2500);

            xtarget += (int)xmod;
            ytarget += (int)ymod;
            String fin = myTeam[currentBuster].stunCooldown == 0 ? "!" : "";
            orders[currentBuster] = $"MOVE {xtarget} {ytarget} hihihi{fin} ";
            return true;
        }
        return false;
    }

    bool RadarWorthIt()
    {
        int score = 0;
        int xchunk = (myTeam[currentBuster].xPos) / gridCellSize;
        int ychunk = (myTeam[currentBuster].yPos) / gridCellSize;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                score += PatrolRating(xchunk, ychunk, j, i, 3);
                if (i == 0 && j == 0)
                {
                    continue;
                }
            }
        }
        Console.Error.WriteLine($"Radarscore = {score}");
        return (myScore >= ghostCount * 3 / 10 && score >= minValueForRadar);
    }

    bool Radar()
    {
        if (myTeam[currentBuster].usedRadar)
            return false;
        orders[currentBuster] = $"RADAR I SEE EVERYTHING";
        myTeam[currentBuster].usedRadar = true;
        return true;
    }

    int OppressionRate(Entity who)
    {
        int rate = 0;
        foreach(Entity enemy in enemyTeam)
        {
            if (enemy.infoAge < 4 && enemy.DistanceTo(who) <= cowardiness)
            {
                Console.Error.WriteLine($"threat on distance {enemy.DistanceTo(who)}");
                rate++;
            }
        }
        return rate;
    }
    
    bool AllyNeedsHelp()
    {
        int bestOppression = 0;
        int bestDistance = 0;
        for (int i = 0; i< bustersPerPlayer; i++)
        {
            if (helping[currentBuster] == i)
            {
                helping[currentBuster] = -1;
                helpers[i]--;
            }
            if (myTeam[i].carrying)
            {
                int oppression = OppressionRate(myTeam[i]);
                if (oppression < bestOppression)
                    continue;
                int distance = myTeam[i].DistanceTo(myBaseX, myBaseY);
                if (oppression == bestOppression && bestDistance > distance)
                    continue;
                bestDistance = distance;
                bestOppression = oppression;
                allyInNeed = i;
            }
        }

        Console.Error.WriteLine($"escorting distance ({shortestEscortDistance})");
        if ((bestDistance > shortestEscortDistance && helpers[allyInNeed] == 0))
            Console.Error.WriteLine($"escorting cause distance ({shortestEscortDistance})");
        if (bestOppression > helpers[allyInNeed])
            Console.Error.WriteLine($"escorting cause oppr ({bestOppression} > {helpers[allyInNeed]})");
        if ((bestDistance > shortestEscortDistance && helpers[allyInNeed] == 0)|| bestOppression > helpers[allyInNeed])
        {
            helping[currentBuster] = allyInNeed;
            helpers[allyInNeed]++;
            return true;
        }
        return false;
    }

    bool MoveToAlly()
    {

        orders[currentBuster] = $"MOVE {myTeam[allyInNeed].xPos} {myTeam[allyInNeed].yPos} Got your back!";
        return true;
    }

    bool Cry()
    {
        orders[currentBuster] = $"MOVE {myTeam[currentBuster].xPos} {myTeam[currentBuster].yPos} ;-;";
        return true;
    }

    BTree.Node EachOnHisOwnTree()
    {
        BTree.Node EnemyInteraction = BTree.Sequencer(new BTree.Node[]
        {
            EnemyInRange,
            BTree.Selector(new BTree.Node[]
            {
                BTree.Sequencer(new BTree.Node[]
                {
                    CanStun,
                    BTree.Selector(new BTree.Node[]
                    {
                        StunClosestEnemy,
                        MoveToClosestEnemy
                    })
                }),
            })
        });

        BTree.Node GhostCarrying = BTree.Sequencer(new BTree.Node[]
        {
            CarriesGhost,
            BTree.Selector(new BTree.Node[]
            {
                Score,
                GoHome
            })
        });

        BTree.Node GhostBusting = BTree.Sequencer(new BTree.Node[]
        {
            GhostInRange,
            BTree.Selector(new BTree.Node[]
            {
                BustClosestGhost,
                MoveToClosestGhost
            })
        });

        BTree.Node MaybeRadar = BTree.Sequencer(new BTree.Node[]
        {
            RadarWorthIt,
            Radar
        });

        BTree.Node Helping = BTree.Sequencer(new BTree.Node[]
        {
            AllyNeedsHelp,
            MoveToAlly
        });

        BTree.Node Crying = BTree.Sequencer(new BTree.Node[]
        {
            (()=>myTeam[currentBuster].stunned),
            Cry
        });

        return BTree.Selector(new BTree.Node[]
        {
            Crying,
            EnemyInteraction,
            GhostCarrying,
            GhostBusting,
            Helping,
            MaybeRadar,
            Camp,
            Patrol
        });
    }
}
#endregion

#region Utilities
public static class BTree
{
    public delegate Boolean Node();

    public static Node Selector(Node[] actions)
    {
        return
            (() =>
            {
                Boolean res = false;
                foreach (Node action in actions)
                {
                    res = action();
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
            (() =>
            {
                Boolean res = false;
                foreach (Node action in actions)
                {
                    res = action();
                    if (!res)
                        break;
                }
                return res;
            }
            );
    }

    public static Node Not(Node action)
    {
        return (() => !(action()));
    }
}


public struct Entity
{
    public int xPos;
    public int yPos;
    public int state;
    public int value;
    public int infoAge;
    public int stunCooldown;
    public bool stunned
    {
        get
        {
            return (state == 2);
        }
    }
    public bool carrying
    {
        get
        {
            return (state == 1);
        }
    }
    public bool usedRadar;

    public int DistanceTo(Entity e)
    {
        return (int)Math.Round((Math.Sqrt(Math.Pow(e.xPos - xPos, 2) + Math.Pow(e.yPos - yPos, 2))));
    }

    public int DistanceTo(int x, int y)
    {
        return (int)Math.Round((Math.Sqrt(Math.Pow(x - xPos, 2) + Math.Pow(y - yPos, 2))));
    }
}
#endregion