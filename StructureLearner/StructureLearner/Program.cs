using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace StructureLearner
{
    class Program
    {
        public static string[][,,] structures;
        public static string[] palette;

        static void Main(string[] args)
        {
            string blocksJson = File.ReadAllText("input/blocks.json");

            List<Structure> processedStructures = JsonSerializer.Deserialize<List<Structure>>(blocksJson);

            structures = new string[processedStructures.Count][,,];

            for (int i = 0; i < processedStructures.Count; i++)
            {
                structures[i] = new string[processedStructures[i].sizeX, processedStructures[i].sizeY, processedStructures[i].sizeZ];

                for (int x = 0; x < structures[i].GetLength(0); x++)
                {
                    for (int y = 0; y < structures[i].GetLength(1); y++)
                    {
                        for (int z = 0; z < structures[i].GetLength(2); z++)
                        {
                            structures[i][x, y, z] = "air";
                        }
                    }
                }

                foreach (Block block in processedStructures[i].blocks)
                {
                    structures[i][block.posX, block.posY, block.posZ] = block.ID;
                }
            }


            if (!Directory.Exists("output"))
            {
                Directory.CreateDirectory("output");
            }

            Dictionary<string, Dictionary<string,int>> stuff = new Dictionary<string, Dictionary<string,int>>();

            for (int s = 0; s < structures.GetLength(0); s++)
            {
                Console.WriteLine("structure: " + s);

                for (int x = 2; x < structures[s].GetLength(0)-2; x++)
                {
                    for (int y = 2; y < structures[s].GetLength(1)-2; y++)
                    {
                        for (int z = 2; z < structures[s].GetLength(2)-2; z++)
                        {
                            string id = structures[s][x, y, z];

                            for (int px = -2; px < 3; px++)
                            {
                                for (int py = -2; py < 3; py++)
                                {
                                    for (int pz = -2; pz < 3; pz++)
                                    {
                                        if (px != 0 || py != 0 || pz != 0)
                                        {
                                            string idOfNext = structures[s][x + px, y + py, z + pz];

                                            string nn = "output/" + id + "+" + px + "+" + py + "+" + pz + ".txt";

                                            
                                            if (stuff.ContainsKey(nn))
                                            {

                                                if (stuff[nn].ContainsKey(idOfNext))
                                                {
                                                    stuff[nn][idOfNext]++;
                                                }
                                                else
                                                {
                                                    stuff[nn].Add(idOfNext, 1);
                                                }

                                            }
                                            else
                                            {
                                                stuff.Add(nn, new Dictionary<string, int>());
                                                stuff[nn].Add(idOfNext, 1);
                                            }

                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }

            foreach (var item in stuff)
            {
                string res = "";
                foreach (var c in item.Value)
                {
                    if (res == "")
                    {
                        res += c.Key + ":" + c.Value;
                    }
                    else
                    {
                        res += ";" + c.Key + ":" + c.Value;
                    }
                }
                File.WriteAllText(item.Key, res);
            }


        }
    }

    public class Structure
    {
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public int sizeZ { get; set; }

        public List<Block> blocks { get; set; }
    }

    public class Block
    {
        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }
        public string ID { get; set; }
    }
}
