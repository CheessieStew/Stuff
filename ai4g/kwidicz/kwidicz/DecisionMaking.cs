using System;
using System.Linq;
using System.Collections.Generic;

partial class Kwidicz
{

    #region AtomActions
    bool Throw(int who, Vector2 where, int power)
    {
        orders[who].target = where;
        orders[who].param = power;
        orders[who].type = Order.ActionType.THROW;
        orders[who].comment = Order.ThrowComment;
        return true;
    }

    bool Move(int who, Vector2 where, int thrust)
    {
        orders[who].target = where;
        orders[who].param = thrust;
        orders[who].type = Order.ActionType.MOVE;
        orders[who].comment = Order.FlyComment;
        return true;
    }

    bool Chase(int who, Entity what)
    {
        if (orders[1-who].chasing)
        {
            Vector2 mytarget = what.pos + what.vel;
            Entity hisSnaffle = snaffles.Find(s => s.id == orders[1 - who].chasedTarget);
            Vector2 histarget = hisSnaffle.pos + hisSnaffle.vel;
            Vector2 me = myWizards[who].pos + myWizards[who].vel;
            Vector2 buddy = myWizards[1 - who].pos + myWizards[1 - who].vel;
            if (me.DistanceTo(mytarget) + buddy.DistanceTo(histarget) > me.DistanceTo(histarget) + buddy.DistanceTo(mytarget))
            {
                Chase(1 - who, what);
                Chase(who, hisSnaffle);
                Console.Error.WriteLine("Swapped targets");
                return true;

            }
        }



        Vector2[] thrusts = Enumerable.Range(0, 20).Select(i => new Vector2(1,0).Rotate(i * (float)Math.PI / 10)).ToArray();
        float bestrating = float.MaxValue;
        Vector2 best = new Vector2();
        foreach (Vector2 t in thrusts)
        {
            float rating = (myWizards[who].pos + myWizards[who].vel + t * 150).DistanceTo(what.pos + what.vel);
            if (rating < bestrating)
            {
                bestrating = rating;
                best = t;
            }
        }
        if (bestrating == float.MaxValue)
            throw new Exception();
        Move(who, myWizards[who].pos + best * 30, 150);
        orders[who].comment = Order.ChaseComment;
        Console.Error.WriteLine($"{who} chasing {what.id}");
        orders[who].chasing = true;
        orders[who].chasedTarget = what.id;
        return true;
    }

    bool CastSpell(int who, Spell which, int target)
    {
        mana -= which.cost;
        orders[who].spellTarget = target;
        orders[who].spell = which;
        orders[who].type = Order.ActionType.SPELL;
        orders[who].comment = which.name + "!";
        if (which == Spell.Flipendo)
        {
            bullets.Add(new Tuple<int, int>(3, target));
        }
        if (which == Spell.Obliviate)
        {
            bludgerMemories[target % 2] = 3;
        }
        return true;
    }

    bool BeAGoalie(int who)
    {

        orders[who].type = Order.ActionType.MOVE;
        orders[who].comment = Order.DefComment;
        orders[who].param = (myWizards[who].pos.DistanceTo(myGoal) > 5000) ? 150 : 90;
        orders[who].target = myGoal;
        if (snaffles.Where(s => s.pos.DistanceTo(myGoal) < s.pos.DistanceTo(enemyGoal)).Count() < snaffles.Count/2 - 1)
        {
            orders[who].target = (myGoal + myGoal + enemyGoal) / 3;
        }
        else
        {
            if (myWizards[who].pos.DistanceTo(myGoal) < 3000)
                return false;
        }
        return true;
    }

    bool EvasiveManouvers(int who)
    {
        orders[who].comment = Order.DodgeComment;
        orders[who].type = Order.ActionType.MOVE;
        Vector2 dir1 = orders[who].target - myWizards[who].pos;
        Vector2 dir2 = dir1;
        Vector2 orgDir = dir2;
        for (int i = 1; i < 16; i++)
        {
            dir1 = dir1.Rotate((float)Math.PI / 8);
            dir2 = dir2.Rotate(-(float)Math.PI / 8);

            orders[who].target = myWizards[who].pos + dir1;
            orders[who].param = 150;
            if (!CollisionImminent(who))
            {
                return true;
            }

            orders[who].target = myWizards[who].pos + dir2;
            orders[who].param = 150;
            if (!CollisionImminent(who))
            {
                return true;
            }
        }
        orders[who].comment = Order.FailedDodgeComment;
        orders[who].target = myWizards[who].pos + orgDir;
        return true;
    }
    #endregion

    #region Predicates

    Entity defTarget;
    bool FastSnaffle(int who)
    {
        Vector2 myGoal;
        if (myTeamId == 0)
            myGoal = new Vector2(0, 3750);
        else
            myGoal = new Vector2(16000, 3750);
        foreach (Entity s in snaffles)
        {
            if (s.pos.DistanceTo(myGoal) < 3000 || s.pos.DistanceTo(myGoal) > 10000)
                continue;
            if (enemyWizards.OrderBy(ew => s.pos.DistanceTo(ew.pos)).First().pos.DistanceTo(s.pos) < 2000)
                continue;
            if (s.vel.Magnitude > 1000)
            {
                if ((s.pos + s.vel).DistanceTo(myGoal) < (s.pos.DistanceTo(myGoal)))
                {
                    defTarget = s;
                    return true;
                }
            }
        }
        return false;
    }

    Entity FlipendoTarget;
    
    bool CanAwesomeShot(int who)
    {
        Vector2 enemyPole1;
        Vector2 enemyPole2;
        if (myTeamId == 0)
        {
            enemyPole1 = new Vector2(16000, 1750);
            enemyPole2 = new Vector2(16000, 5750);
        }
        else
        {
            enemyPole1 = new Vector2(0, 1750);
            enemyPole2 = new Vector2(0, 5750);
        }
        foreach (Entity s in snaffles)
        {
            if (bullets.Any(b => b.Item2 == s.id))
                continue;
            if (s.vel.Magnitude > 1000 && (myTeamId == 0 && s.vel.x > 0 || myTeamId == 1 && s.vel.x < 0))
                continue;
            if (s.id == 8)
                Console.Error.WriteLine($"andrzej 0 vel {s.vel}");
            if (s.pos.DistanceTo(myWizards[who].pos) < 500)
                continue;
            if (s.id == 8)
                Console.Error.WriteLine("andrzej 1");

            float minEnemyDist = float.MaxValue;
            foreach (Entity ew in enemyWizards)
            {
                var enemydist = (ew.pos + ew.vel).DistanceTo(s.pos + s.vel);
                if (enemydist < minEnemyDist)
                    minEnemyDist = enemydist;
            }

            if (s.id == NearestSnaffle(1 - who).id && minEnemyDist > 2000)
                continue;
            if (s.id == 8)
                Console.Error.WriteLine("andrzej 2");
            if (enemyPole1.DistanceToLine(myWizards[who].pos + myWizards[who].vel, s.pos + s.vel) < 480)
                continue;

            if (enemyPole2.DistanceToLine(myWizards[who].pos + myWizards[who].vel, s.pos + s.vel) < 480)
                continue;
            if (s.id == 8)
                Console.Error.WriteLine("andrzej 3");
            var touch1 = enemyPole1.TouchPointOnLine(myWizards[who].pos + myWizards[who].vel, s.pos + s.vel);

            var touch2 = enemyPole2.TouchPointOnLine(myWizards[who].pos + myWizards[who].vel, s.pos + s.vel);

            var touchav = (touch1 + touch2) / 2;
            if (touchav.DistanceTo(s.pos) > touchav.DistanceTo(myWizards[who].pos))
                continue;
            if (s.id == 8)
                Console.Error.WriteLine("andrzej 4");
            if (touchav.DistanceTo(s.pos) + s.pos.DistanceTo(myWizards[who].pos) < 2300)
                continue;
            if (s.id == 8)
                Console.Error.WriteLine("andrzej 5");
            if (s.pos.DistanceTo(myWizards[who].pos) > 3000)
            {
                var forceNorm = s.pos - myWizards[who].pos;
                float power = Math.Min(3000 / Vector2.Square(forceNorm.Magnitude / 1000), 1000);
                forceNorm = forceNorm.Normalized;
                var nvel = s.vel + forceNorm * power;
                if (touchav.DistanceTo(s.pos + nvel * 2) > 1000)
                    continue;

            }
            if (s.id == 8)
                Console.Error.WriteLine("andrzej 6");
            Vector2 kindaSpeed = (touchav - s.pos - s.vel).Normalized * 300;

            Vector2 crosspoint = s.pos + s.vel;
            //Console.Error.WriteLine($"loop cs is {crosspoint}");
            int i = 0;
            while (i < 40 && crosspoint.x < 15500 && crosspoint.x > 500)
            {
                i++;
                //Console.Error.WriteLine($"loop cs is {crosspoint}");

                crosspoint += kindaSpeed;
            }
            if (i == 40)
                continue;
            if (s.id == 8)
                Console.Error.WriteLine("andrzej 7");
            if (!(crosspoint.y < 5750) || !(crosspoint.y > 1750))
                continue;
            bool obstacle = false;
            foreach (Entity b in bludgers)
            {
                if (b.id == s.id)
                    continue;
                if ((b.pos + b.vel).DistanceToLine(myWizards[who].pos + myWizards[who].vel, s.pos + s.vel) < 160)
                {
                    if (b.pos.DistanceTo(myWizards[who].pos) < b.pos.DistanceTo(s.pos))
                        continue;
                    obstacle = true;

                    break;
                }
            }
            foreach (Entity s2 in snaffles)
            {
                if (s2.id == s.id)
                    continue;
                if ((s2.pos + s2.vel).DistanceToLine(myWizards[who].pos + myWizards[who].vel, s.pos + s.vel) < 160)
                {
                    if (s2.pos.DistanceTo(myWizards[who].pos) < s2.pos.DistanceTo(s.pos))
                        continue;
                    obstacle = true;

                    break;
                }
            }
            foreach (Entity ew in enemyWizards)
            {
                if ((ew.pos + ew.vel).DistanceToLine(myWizards[who].pos + myWizards[who].vel, s.pos + s.vel) < 500)
                {
                    if (ew.pos.DistanceTo(myWizards[who].pos) < ew.pos.DistanceTo(s.pos))
                        continue;
                    obstacle = true;
                    if (s.id == 5)
                        Console.Error.WriteLine($"obstacle is {ew.id}");
                    break;
                }
            }
            if (!obstacle)
            {
                FlipendoTarget = s;
                return true;
            }

        }
        return false;
    }

    bool WorthPetrifying(Entity what)
    {
        IEnumerable<Entity> targets = myWizards.Concat(enemyWizards).OrderBy(wiz => wiz.pos.DistanceTo(what.pos));
        if (targets.First().id < 2 && myTeamId == 0 || targets.First().id > 1 && myTeamId == 1)
        {
            if ((what.pos + what.vel).DistanceTo(targets.First().pos + targets.First().vel) < 800)
                return true;
        }
        return false;
    }
    bool WorthObliviating(Entity what)
    {
        IEnumerable<Entity> targets = myWizards.Concat(enemyWizards).OrderBy(wiz => wiz.pos.DistanceTo(what.pos));
        if (targets.First().id < 2 && myTeamId == 0 || targets.First().id > 1 && myTeamId == 1)
        {
            if ((what.pos + what.vel).DistanceTo(targets.First().pos + targets.First().vel) < 2400)
                return false;
            if (myTeamId == 0)
            {
                if (targets.First(wiz => wiz.id > 1).pos.DistanceTo(what.pos) - targets.First().pos.DistanceTo(what.pos) < 1500)
                    return true;
            }
            else
            {
                if (targets.First(wiz => wiz.id < 2).pos.DistanceTo(what.pos) - targets.First().pos.DistanceTo(what.pos) < 1500)
                    return true;
            }
        }
        return false;
    }

    bool CanThrow(int who)
    {
        return myWizards[who].state == 1;
    }

    Entity pullTarget;
    bool WorthPulling(int who)
    {
        Vector2 myGoal;
        if (myTeamId == 0)
            myGoal = new Vector2(0, 3750);
        else
            myGoal = new Vector2(16000, 3750);
        Vector2 enemyGoal = new Vector2(16000 - myGoal.x, myGoal.y);
        foreach (Entity s in snaffles)
        {
            if (s.pos.DistanceTo(myWizards[who].pos) > 10000)
                continue;
            if (NearestSnaffle(1 - who).id == s.id && (s.pos + s.vel).DistanceTo(myWizards[1-who].pos + myWizards[1-who].vel) < 2600)
                continue;
            if (s.pos.DistanceTo(myGoal) < 1800 && Math.Abs(s.pos.x - myGoal.x) < 800 && Math.Abs((s.pos + s.vel).x - myGoal.x) < Math.Abs(s.pos.x - myGoal.x))
                continue;
            if (Math.Abs(s.pos.x - myGoal.x) < 200)
                continue;
            if (s.pos.DistanceTo(myGoal) < myWizards[who].pos.DistanceTo(myGoal))
            {
                bool nearest = (s.id == NearestSnaffle(who).id);
                bool enemyClose = false;

                foreach (Entity ew in enemyWizards)
                    if ((ew.pos + ew.vel).DistanceTo(s.pos + s.vel) < (myWizards[who].pos + myWizards[who].vel).DistanceTo(s.pos + s.vel))
                        enemyClose = true;
                if (enemyClose && nearest || nearest && enemyGoal.DistanceTo(myWizards[who].pos) < 5000 || enemyClose && snaffles.Count < 4)
                {
                    pullTarget = s;
                    return true;
                }
            }
        }
        return false;
    }

    bool InMyWay(Entity e, int who)
    {
        return (e.pos + (e.id == myWizards[1 - who].id ? FutureVel(1 - who) : e.vel)).InCone
            (
                myWizards[who].pos,
                myWizards[who].pos + FutureVel(who).Normalized * (e.id == bludgers[0].id || e.id == bludgers[1].id ? 3000 : 1400),
                e.id == myWizards[1 - who].id ? 900 : 600,
                1200
            );
    }

    bool CollisionImminent(int who)
    {
        if (InMyWay(myWizards[1 - who], who))
        {
            return true;
        }
        foreach (Entity b in bludgers)
            if (InMyWay(b, who))
            {
                Console.Error.WriteLine("bludger ;-;");
                return true;
            }
        foreach (Entity ew in enemyWizards)
            if (InMyWay(ew, who))
            {
                return true;
            }
        return false;
    }

    bool CanAfford(Spell s, int howmany = 1)
    {
        if (mana < s.cost * howmany)
            Console.Error.WriteLine($"can't afford {s.name}");
        return (mana >= s.cost * howmany);
    }

    bool BludgerDanger(int who)
    {
        foreach (Entity b in bludgers)
        {
            // Console.Error.WriteLine($"memory of {b.id} = {bludgerMemories[b.id % 2]}");
            // Console.Error.WriteLine($"cone {b.pos}, {b.pos + b.vel.Normalized * 2400}, 800, 3000 ({myWizards[who].pos.InCone(b.pos, b.pos + b.vel.Normalized * 4000, 800, 3000)}, vel {b.vel})");
            if (bludgerMemories[b.id % 2] == 0 &&
                myWizards[who].pos.InCone(b.pos, b.pos + b.vel.Normalized * 2400, 900, 2000))
                return true;
        }
        return false;
    }
    #endregion

    #region ValueSellers
    Entity NearestSnaffle(int who)
    {

        Vector2 myGoal;
        if (myTeamId == 0)
            myGoal = new Vector2(0, 3750);
        else
            myGoal = new Vector2(16000, 3750);


        float defVal = 1.7f;
        if (snaffles.Where(s => (s.pos + s.vel).DistanceTo(myGoal) < (s.pos + s.vel).DistanceTo(new Vector2(myGoal.x == 0 ? 16000 : 0, myGoal.y))).Count() > snaffles.Count / 2.0 + myScore)
        {
            defVal = 5;
        }
        if (myWizards[who].pos.DistanceTo(myGoal) < myWizards[1 - who].pos.DistanceTo(myGoal))
            defVal += 3;
        Entity res = new Entity();
        res.id = -1;
        float minDist = float.MaxValue;
        Console.Error.WriteLine();
        foreach (Entity s in snaffles)
        {
            if (s.id == orders[1 - who].chasedTarget && snaffles.Count > 1)
                continue;
            float dist = (s.pos + s.vel).DistanceTo(myWizards[who].pos + myWizards[who].vel);
            dist *= (1 + defVal * s.pos.DistanceTo(myGoal) / 20000);
            if (dist < minDist)
            {
                bool nvm = false;
                foreach (Entity w in enemyWizards)
                {
                    if (s.pos.DistanceTo(w.pos) < 400)
                    {
                        nvm = true;
                    }
                }
                if (!nvm)
                {
                    minDist = dist;
                    res = s;
                }
            }
        }
        return res;
    }

    Entity NearestBludger(int who)
    {
        Entity res = new Entity();
        res.id = -1;
        float minDist = float.MaxValue;
        foreach (Entity b in bludgers)
        {
            float dist = b.pos.DistanceTo(myWizards[who].pos);
            if (dist < minDist)
            {
                minDist = dist;
                res = b;
            }
        }
        return res;
    }

    Vector2 FutureVel(int who)
    {
        return myWizards[who].vel + (orders[who].target - myWizards[who].pos).Normalized * orders[who].param;
    }

    Vector2 ThrowTarget(int who)
    {
        Vector2 target;
        Entity mySnaffle = NearestSnaffle(who);

        if (myTeamId == 0)
        {
            target = new Vector2(16000, 3750);
        }
        else
        {
            target = new Vector2(0, 3750);
        }
        Vector2 whereItLands = mySnaffle.pos + (myWizards[who].vel + (target - mySnaffle.pos).Normalized * 1000) * 10;

        if (myWizards[1 - who].pos.DistanceTo(myWizards[who].pos) < 600 && myWizards[1 - who].pos.InCone(mySnaffle.pos, whereItLands, 800, 3000))
        {
            target = myWizards[1 - who].pos;
        }
        whereItLands = mySnaffle.pos + (myWizards[who].vel + (target - mySnaffle.pos).Normalized * 1000) * 10;
        float[] mods = Enumerable.Range(-10, 21).Select(m => m * (float)Math.PI / 30).ToArray();

        float bestminobstacledistance = 0;
        Vector2 bestTarget = new Vector2();
        float bestPenalty = float.MaxValue;
        foreach (var m in mods)
        {
            var tempTarget = mySnaffle.pos + (target - mySnaffle.pos).Rotate(m);
            var tempWhereItLands = mySnaffle.pos + (myWizards[who].vel + (tempTarget - mySnaffle.pos).Normalized * 1000) * 10;
            int worstId = -1;
            float minObstacleDistance = float.MaxValue;
            foreach (Entity ew in enemyWizards)
            {
                float obstacleDistance = (float)ew.pos.DistanceToLine(mySnaffle.pos, tempWhereItLands);
                if (ew.pos.DistanceTo(mySnaffle.pos) > 6000)
                    continue;
                if (ew.pos.DistanceTo(mySnaffle.pos) > 4000)
                    obstacleDistance *= (1 - ew.pos.TouchPointOnLine(mySnaffle.pos, tempWhereItLands).DistanceTo(mySnaffle.pos) / whereItLands.DistanceTo(mySnaffle.pos));
                if (obstacleDistance < minObstacleDistance && ew.pos.TouchPointOnLine(mySnaffle.pos, tempWhereItLands).Between(myWizards[who].pos, tempWhereItLands))
                {
                    minObstacleDistance = obstacleDistance;
                    worstId = ew.id;
                }
            }
            foreach (Entity b in bludgers)
            {
                float obstacleDistance = (float)(b.pos + b.vel).DistanceToLine(mySnaffle.pos, tempWhereItLands);
                if (b.id == 11)
                    if (b.pos.DistanceTo(mySnaffle.pos) > 6000)
                        continue;
                if (b.pos.DistanceTo(mySnaffle.pos) > 4000)
                    obstacleDistance *= (1 - b.pos.TouchPointOnLine(mySnaffle.pos, tempWhereItLands).DistanceTo(mySnaffle.pos) / whereItLands.DistanceTo(mySnaffle.pos));

                if (obstacleDistance < minObstacleDistance && b.pos.TouchPointOnLine(mySnaffle.pos, tempWhereItLands).Between(myWizards[who].pos, tempWhereItLands))
                {
                    minObstacleDistance = obstacleDistance;
                    worstId = b.id;
                }
            }

            var tempPenalty = (target - tempTarget).Magnitude;

            Console.Error.WriteLine($"Target {tempTarget} of angle {m * 360 / (2 * Math.PI)} has mindist {minObstacleDistance}, worst obstacle {worstId}");

            if (bestminobstacledistance > 1000 && minObstacleDistance > 1000 )
            {
                if (tempPenalty < bestPenalty)
                {
                    bestminobstacledistance = minObstacleDistance;
                    bestTarget = tempTarget;
                    bestPenalty = tempPenalty;
                }
            }
            else if (minObstacleDistance > bestminobstacledistance)
            {
                bestminobstacledistance = minObstacleDistance;
                bestTarget = tempTarget;
                bestPenalty = tempPenalty;
            }
        }

        Console.Error.WriteLine($"Picked target {bestTarget}");
        return bestTarget;
    }
    #endregion

    void Think()
    {
        orders[0].target = new Vector2(-10000, -10000);
        orders[0].type = Order.ActionType.NOOP;
        orders[0].chasing = false;
        orders[1].target = new Vector2(-10000, -10000);
        orders[1].type = Order.ActionType.NOOP;
        orders[1].chasing = false;

        WizardBrain(0);
        WizardBrain(1);

        //EvasiveManouvers();
    }

    BTree.Node _btree;
    BTree.Node WizardBrain
    {
        get
        {
            if (_btree != null)
                return _btree;
            BTree.Node Attack = BTree.Selector(new BTree.Node[]
            {

                BTree.Sequencer(new BTree.Node[]
                {
                    CanThrow,
                    (who => Throw(who, ThrowTarget(who), 500))
                }),
                BTree.Sequencer(new BTree.Node[]
                {
                    (_ => CanAfford(Spell.Flipendo)),
                    CanAwesomeShot,
                    (who => orders[1-who].type != Order.ActionType.SPELL || orders[1-who].spellTarget != FlipendoTarget.id),
                    (who => CastSpell(who,Spell.Flipendo,FlipendoTarget.id))
                }),
            });

            BTree.Node BludgersSuck = BTree.Sequencer(new BTree.Node[]
            {
                BludgerDanger,
                BTree.Selector(new BTree.Node[]
                {
                    BTree.Sequencer(new BTree.Node[]
                    {
                        (_ => CanAfford(Spell.Obliviate)),
                        (who => WorthObliviating(NearestBludger(who))),
                        (who => orders[1-who].type != Order.ActionType.SPELL || orders[1-who].spellTarget != NearestBludger(who).id),
                        (who => CastSpell(who, Spell.Obliviate, NearestBludger(who).id))
                    }),
                    BTree.Sequencer(new BTree.Node[]
                    {
                        (_ => CanAfford(Spell.Flipendo, 3)),
                        (who => orders[1-who].type != Order.ActionType.SPELL || orders[1-who].spellTarget != NearestBludger(who).id),
                        (who => CastSpell(who, Spell.Flipendo, NearestBludger(who).id))
                    }),
                })
            });

            BTree.Node FlyingSkillz = BTree.Sequencer(new BTree.Node[]
            {
                (who => orders[who].type == Order.ActionType.MOVE),
                CollisionImminent,
                EvasiveManouvers
            });

            BTree.Node Defend = BTree.Selector(new BTree.Node[]
            {
                BTree.Sequencer(new BTree.Node[]
                {
                    (_ => CanAfford(Spell.Petrificus)),
                    FastSnaffle,
                    (who => orders[1-who].type != Order.ActionType.SPELL || orders[1-who].spellTarget != defTarget.id),
                    (who => CastSpell(who, Spell.Petrificus, defTarget.id))
                }),
                BTree.Sequencer(new BTree.Node[]
                {
                    (_ => CanAfford(Spell.Accio)),
                    (who => !CanAwesomeShot(1-who)),
                    WorthPulling,
                    (who => orders[1-who].type != Order.ActionType.SPELL || orders[1-who].spellTarget != pullTarget.id),
                    (who => CastSpell(who,Spell.Accio,pullTarget.id))
                }),
                BTree.Sequencer(new BTree.Node[]
                {
                    (_ => snaffles.Count < 3),
                    (who => myWizards[who].pos.DistanceTo(myGoal) < myWizards[1-who].pos.DistanceTo(myGoal)),
                    (who => NearestSnaffle(who).pos.DistanceTo(myWizards[who].pos) > 2400),
                    BeAGoalie
                }),
            });

            _btree = BTree.Sequencer(new BTree.Node[]
            {
                BTree.Selector(new BTree.Node[]
                {
                    Attack,
                    Defend,
                    BludgersSuck,
                    BTree.Sequencer(new BTree.Node[]
                    {
                        (who => NearestSnaffle(who).id > 0),
                        (who => Chase(who, NearestSnaffle(who)))
                    }),
                    BTree.Sequencer(new BTree.Node[]
                    {
                        (_ => snaffles.Count < 3 && snaffles.Where(s => s.pos.DistanceTo(myGoal) < s.pos.DistanceTo(enemyGoal)).Count() > 0
                              || snaffles.Count < 5 && snaffles.Where(s => s.pos.DistanceTo(myGoal) < s.pos.DistanceTo(enemyGoal)).Count() > 1),
                        BeAGoalie
                    }),
                    (who => Move(who, myWizards[1-who].pos, 150)),
                }),
            });
            return _btree;
        }
    }

    void EvasiveManouvers()
    {

        float[] angles = Enumerable.Range(-6, 13).Select(a => a * (float)Math.PI / 20).ToArray();
        Vector2 p0 = myWizards[0].pos;
        Vector2 p1 = myWizards[1].pos;
        Vector2 t0 = orders[0].type == Order.ActionType.MOVE ? (orders[0].target - myWizards[0].pos).Normalized * orders[0].param : new Vector2(0, 0);
        Vector2 t1 = orders[1].type == Order.ActionType.MOVE ? (orders[1].target - myWizards[1].pos).Normalized * orders[1].param : new Vector2(0, 0);
        Vector2 v0 = myWizards[0].vel;
        Vector2 v1 = myWizards[1].vel;
        Tuple<int, int> bestAngles = new Tuple<int, int>(10, 10);
        float leastDanger = float.MaxValue;
        for (int i0 = 0; i0 < angles.Length; i0++)
        {
            for (int i1 = 0; i1 < angles.Length; i1++)
            {
                Vector2 p0temp = p0 + v0 + t0.Rotate(angles[i0]);
                Vector2 p1temp = p1 + v1 + t1.Rotate(angles[i1]);
                float score = Math.Abs(i0 - 6) + Math.Abs(i1 - 6); //moves curving too far are sad
                foreach (Entity b in bludgers)
                {
                    if (b.vel.Magnitude == 0)
                        continue;
                    float danger0 = 1500 - (float)p0temp.DistanceToLine(b.pos, b.pos + b.vel);
                    if (i0 == 4)
                        Console.Error.WriteLine($"Danger0 {danger0}");
                    if (p0temp.InCone(b.pos, b.pos + b.vel * 6, 400, 1300))
                        score += danger0;
                    float danger1 = 1500 - (float)p1temp.DistanceToLine(b.pos, b.pos + b.vel);
                    if (p1temp.InCone(b.pos, b.pos + b.vel * 6, 400, 1300))
                        score += danger1;
                }
                if (p0temp.DistanceTo(p1temp) < 2000)
                    score += p0temp.DistanceTo(p1temp)/100;
                if (i1 == i0)
                    Console.Error.WriteLine($"curve {i0 - 6}, {i1 - 6} has score {score}");
                if (score < leastDanger)
                {
                    leastDanger = score;
                    bestAngles = new Tuple<int, int>(i0, i1);
                }
            }
        }
        Console.Error.WriteLine($"Chosen curve {bestAngles}");
        Console.Error.WriteLine($"Wizard 0 curved {bestAngles.Item1 - 6}");
        Console.Error.WriteLine($"Wizard 1 curved {bestAngles.Item2 - 6}");
        if (orders[0].type == Order.ActionType.MOVE)
            orders[0].target = p0 + t0.Rotate(angles[bestAngles.Item1]) * 10;
        if (orders[1].type == Order.ActionType.MOVE)
            orders[1].target = p1 + t1.Rotate(angles[bestAngles.Item2]) * 10;


    }
}
