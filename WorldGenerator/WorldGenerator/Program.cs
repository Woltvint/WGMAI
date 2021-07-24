using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

namespace WorldGenerator
{
    class Program
    {
        public static int[][,,] structures;
        public static string[] palette;
        static void Main(string[] args)
        {
            palette = File.ReadAllLines("input/blockPalette.txt");
            string blocksJson = File.ReadAllText("input/blocks.json");

            List<Structure> processedStructures = JsonSerializer.Deserialize<List<Structure>>(blocksJson);

            structures = new int[processedStructures.Count][,,];

            for (int i = 0; i < processedStructures.Count; i++)
            {
                structures[i] = new int[processedStructures[i].sizeX, processedStructures[i].sizeY, processedStructures[i].sizeZ];

                for (int x = 0; x < structures[i].GetLength(0); x++)
                {
                    for (int y = 0; y < structures[i].GetLength(1); y++)
                    {
                        for (int z = 0; z < structures[i].GetLength(2); z++)
                        {
                            structures[i][x, y, z] = -1;
                        }
                    }
                }

                foreach (Block block in processedStructures[i].blocks)
                {
                    structures[i][block.posX, block.posY, block.posZ] = block.ID;
                }
            }

            Evolution evo = new Evolution(palette.Length * 26,palette.Length,200);

            evo.train();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    break;
                }
            }

            evo.stop();

            


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
        public int ID { get; set; }
    }


}
