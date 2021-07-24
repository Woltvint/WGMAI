using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using fNbt;

namespace StructurePreprocessor
{
    class Program
    {
        static void Main(string[] args)
        {
            List<NbtFile> structures = new List<NbtFile>();
            List<string> blockPalette = new List<string>();
            List<Structure> processedStructures = new List<Structure>();

            if (!Directory.Exists("input"))
            {
                Console.WriteLine("input folder not found");
                return;
            }

            string[] files = Directory.GetFiles("input", "*.nbt");

            if (files.Length == 0)
            {
                return;
            }

            foreach (string f in files)
            {
                structures.Add(new NbtFile(f));
            }

            foreach (NbtFile structure in structures)
            {
                NbtList palette = structure.RootTag.Get<NbtList>("palette");

                foreach (NbtCompound b in palette)
                {
                    blockPalette.Add(b.Get<NbtString>("Name").Value);
                }
            }

            if (!Directory.Exists("output"))
            {
                Directory.CreateDirectory("output");
            }

            if (File.Exists("output/blockPalette.txt"))
            {
                File.Delete("output/blockPalette.txt");
            }

            blockPalette = blockPalette.Distinct().ToList();

            File.WriteAllLines("output/blockPalette.txt", blockPalette);


            foreach (NbtFile structure in structures)
            {
                NbtList size = structure.RootTag.Get<NbtList>("size");

                NbtList palette = structure.RootTag.Get<NbtList>("palette");

                NbtList blocks = structure.RootTag.Get<NbtList>("blocks");

                Structure struc = new Structure(size.Get<NbtInt>(0).Value, size.Get<NbtInt>(1).Value, size.Get<NbtInt>(2).Value);

                foreach (NbtCompound b in blocks)
                {
                    int state = b.Get<NbtInt>("state").Value;
                    NbtList pos = b.Get<NbtList>("pos");

                    for (int a = 0; a < blockPalette.Count; a++)
                    {
                        string name = palette.Get<NbtCompound>(state).Get<NbtString>("Name").Value;
                        if (blockPalette[a] == name)
                        {
                            struc.blocks.Add(new Block(a, pos.Get<NbtInt>(0).Value, pos.Get<NbtInt>(1).Value, pos.Get<NbtInt>(2).Value));
                        }
                    }
                }

                processedStructures.Add(struc);

            }

            JsonSerializerOptions o = new JsonSerializerOptions();

            o.WriteIndented = true;

            string json = JsonSerializer.Serialize(processedStructures, o);

            if (File.Exists("output/blocks.json"))
            {
                File.Delete("output/blocks.json");
            }

            File.WriteAllText("output/blocks.json", json);
        }
    }

    public class Structure
    {
        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public int sizeZ { get; set; }

        public List<Block> blocks { get; set; }

        public Structure(int _sizeX, int _sizeY, int _sizeZ)
        {
            sizeX = _sizeX;
            sizeY = _sizeY;
            sizeZ = _sizeZ;
            blocks = new List<Block>();
        }
    }

    public class Block
    {
        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }
        public int ID { get; set; }

        public Block(int _id, int px, int py, int pz)
        {
            posX = px;
            posY = py;
            posZ = pz;
            ID = _id;
        }
    }
}
