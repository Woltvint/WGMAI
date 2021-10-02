using System;
using System.IO;
using System.Collections.Generic;
using WindowsInput;
using WindowsInput.Native;
using System.Threading;

namespace StructureGenerator
{
    class Program
    {
        public static string[,,] structure;

        public static int sizeX = 20;
        public static int sizeY = 20;
        public static int sizeZ = 20;

        public static int sizeXoff = 0;
        public static int sizeYoff = 18;
        public static int sizeZoff = 0;

        public static Dictionary<string, Dictionary<string, int>[,,]> blocks = new Dictionary<string, Dictionary<string, int>[,,]>();

        public static Random rnd = new Random();

        static void Main(string[] args)
        {
            if (!Directory.Exists("input"))
            {
                Console.WriteLine("input folder not found");
                return;
            }

            structure = new string[sizeX, sizeY, sizeZ];

            Console.WriteLine("loading..");

            string[] files = Directory.GetFiles("input", "*.txt");

            foreach (string file in files)
            {
                string[] name = file.Replace(".txt", "").Split('+');
                string id = name[0].Replace("input\\", "");

                if (!blocks.ContainsKey(id))
                {
                    blocks.Add(id, new Dictionary<string, int>[5, 5, 5]);
                }

                int px = Convert.ToInt32(name[1]) + 2;
                int py = Convert.ToInt32(name[2]) + 2;
                int pz = Convert.ToInt32(name[3]) + 2;

                string[] around = File.ReadAllText(file).Split(';');

                blocks[id][px, py, pz] = new Dictionary<string, int>();

                for (int i = 0; i < around.Length; i++)
                {
                    string blockname = around[i].Split(':')[0];
                    int blockcount = Convert.ToInt32(around[i].Split(':')[1]);

                    blocks[id][px, py, pz].Add(blockname, blockcount);
                }



            }

            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    structure[x, 0, z] = "stone";
                }
            }
            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    structure[x, 1, z] = "grass_block";
                }
            }

            for (int y = 2; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        structure[x, y, z] = "air";
                    }
                }
            }
            InputSimulator ins = new InputSimulator();
            Console.WriteLine("loading done");
            Thread.Sleep(2000);
            Console.WriteLine("Begin");
            ins.Keyboard.KeyPress(VirtualKeyCode.VK_W);
            Thread.Sleep(5000);


            ins.Keyboard.KeyPress(VirtualKeyCode.VK_T);
            Thread.Sleep(100);
            ins.Keyboard.TextEntry("/fill " + sizeXoff + " " + (sizeYoff + 2) + " " + sizeXoff + " " + (sizeX + sizeXoff) + " " + (sizeY + sizeYoff) + " " + (sizeZ + sizeZoff) + " " + "air");
            Thread.Sleep(100);
            ins.Keyboard.KeyPress(VirtualKeyCode.RETURN);

            bool first = true;

            

            while (true)
            {
                string[,,] newStrut = structure;

                for (int y = 2; y < sizeY - 2; y++)
                {
                    for (int x = 2; x < sizeX - 2; x++)
                    {
                        for (int z = 2; z < sizeZ - 2; z++)
                        {
                            Dictionary<string, int> possible = new Dictionary<string, int>();

                            for (int px = -2; px < 3; px++)
                            {
                                for (int py = -2; py < 3; py++)
                                {
                                    for (int pz = -2; pz < 3; pz++)
                                    {
                                        if (px != 0 || py != 0 || pz != 0)
                                        {
                                            int ppx = (px * -1) + 2;
                                            int ppy = (py * -1) + 2;
                                            int ppz = (pz * -1) + 2;

                                            if (blocks.ContainsKey(structure[x + px, y + py, z + pz]))
                                            {
                                                foreach (var item in blocks[structure[x + px, y + py, z + pz]][ppx, ppy, ppz])
                                                {
                                                    if (possible.ContainsKey(item.Key))
                                                    {
                                                        possible[item.Key] += item.Value;
                                                    }
                                                    else
                                                    {
                                                        possible.Add(item.Key, item.Value);
                                                    }
                                                }


                                            }


                                        }
                                    }
                                }
                            }

                            List<KeyValuePair<string, int>> pos = new List<KeyValuePair<string, int>>();

                            foreach (var item in possible)
                            {
                                pos.Add(KeyValuePair.Create(item.Key, item.Value));
                            }


                            pos.Sort(
                            delegate (KeyValuePair<string, int> pair1,
                            KeyValuePair<string, int> pair2)
                            {
                                if (pair1.Value == pair2.Value)
                                {
                                    return 0;
                                }
                                else if (pair1.Value > pair2.Value)
                                {
                                    return -1;
                                }
                                else
                                {
                                    return 1;
                                }
                            }
                            );




                            
                            List<string> choice = new List<string>();

                            for (int i = 0; i < 10; i++)
                            {
                                for (int j = 0; j < 50/Math.Pow(2,i); j++)
                                {
                                    choice.Add(pos[i].Key);
                                }
                            }

                            string chosen = choice[rnd.Next(0,choice.Count)];

                            

                            newStrut[x, y, z] = chosen;

                            if (newStrut[x, y, z] != "air" || !first)
                            {
                                ins.Keyboard.KeyPress(VirtualKeyCode.VK_T);
                                Thread.Sleep(25);
                                ins.Keyboard.TextEntry("/setblock " + (x + sizeXoff) + " " + (y + sizeYoff) + " " + (z + sizeZoff) + " " + newStrut[x, y, z]);
                                Thread.Sleep(25);
                                ins.Keyboard.KeyPress(VirtualKeyCode.RETURN);

                                Console.WriteLine(newStrut[x, y, z]);

                                Thread.Sleep(25);
                            }


                        }
                    }
                }


                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        for (int z = 0; z < sizeZ; z++)
                        {
                            structure[x,y,z] = newStrut[x,y,z];
                        }
                    }
                }
                

                first = false;
            }

        }
    }
}
