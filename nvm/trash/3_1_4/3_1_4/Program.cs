using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace _3_1_4
{
    class GabinetGolibrody
    {
        public int poczekalniaMax = 5;
        public int poczekalniaCurrent = 0;
        int numerek = 0;
        public Queue<Thread> poczekalnia;
        public Thread golibrodaT;
        public bool czyZajete;

        public GabinetGolibrody()
        {
            poczekalnia = new Queue<Thread>();
        }

        void Live()
        {
            golibrodaT.Start();
            Random rand = new Random();
            while (true)
            {
                Thread.Sleep(rand.Next(2000,4000));
                Klient k = new Klient(numerek,this);
                new Thread(k.Live).Start();
                numerek++;
            }
        }

        static void Main(string[] args)
        {
            GabinetGolibrody gabinet = new GabinetGolibrody();
            Golibroda golibroda = new Golibroda(gabinet);
            gabinet.golibrodaT = new Thread(golibroda.Live);
            gabinet.Live();
        }

    }

    class Golibroda
    {
        GabinetGolibrody mojGabinet;

        public Golibroda(GabinetGolibrody gabinet)
        {
            mojGabinet = gabinet;
        }

        public void Live()
        {
            bool work = true;
            Thread klient = null;

            while (true)
            {
                while(work)
                {
                    lock(mojGabinet.poczekalnia)
                    {
                        if (mojGabinet.poczekalnia.Any())
                        {
                            mojGabinet.poczekalniaCurrent--;
                            klient = mojGabinet.poczekalnia.Dequeue();
                            mojGabinet.czyZajete = true;
                        }
                        else
                        {
                            work = false;
                            mojGabinet.czyZajete = false;
                        }
                    }
                    if (work)
                    {
                        Console.WriteLine("\t[Golibroda Amadeusz] Golę klienta.");
                        Thread.Sleep(2400);
                        Console.WriteLine("\t[Golibroda Amadeusz] Ogolono, idź pan se.");
                        if (klient != null) klient.Interrupt();
                    }
                       
                }

                work = true;
                Console.WriteLine("\t[Golibroda Amadeusz] Idę spać.");
                try
                {
                    Thread.Sleep(Timeout.Infinite);
                }
                catch(ThreadInterruptedException ex)
                {
                    Console.WriteLine("\t[Golibroda Amadeusz] Ktoś mnie obudził.");
                }
            }

        }
    }

    class Klient
    {
        GabinetGolibrody tamIde;
        int numerek;

        public Klient(int num, GabinetGolibrody gabinet)
        {
            numerek = num;
            tamIde = gabinet;
        }

        public void Live()
        {
            bool czywychodzimy = false;
            Console.WriteLine($"[Klient {numerek}]: Dzien dobry.");
            lock(tamIde.poczekalnia)
            {
                if (tamIde.poczekalniaCurrent < tamIde.poczekalniaMax)
                {
                    tamIde.poczekalniaCurrent++;
                    tamIde.poczekalnia.Enqueue(Thread.CurrentThread);
                    if (!tamIde.czyZajete)
                    {
                        Console.WriteLine($"[Klient {numerek}]: Budzę golibrodę.");
                        tamIde.golibrodaT.Interrupt();
                    }
                    else Console.WriteLine($"[Klient {numerek}]: Siadam w poczekalni.");
                }
                else czywychodzimy = true;
            }

            if (czywychodzimy)
            {
                Console.WriteLine($"[Klient {numerek}]: Ojojoj jak tu tłoczno, do widzenia.");
                Thread.EndCriticalRegion();
                return;
            }
            else
            {
                try
                {
                    Thread.Sleep(Timeout.Infinite);
                }
                catch(ThreadInterruptedException ex)
                {
                    Console.WriteLine($"[Klient {numerek}]: Dziękuję bardzo, do widzenia.");
                }
                return;
            }
        }
    }

}
