using System;
using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace WorldGenerator
{
    class Evaulator : IPhenomeEvaluator<IBlackBox>
    {
        public ulong EvaluationCount => 0;

        public bool StopConditionSatisfied => shouldEnd;
        bool shouldEnd = false;

        Random rnd = new Random();

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            //phenome.InputSignalArray[index] - input neuron array
            //phenome.OutputSignalArray[index] - output neuron array

            double fitness = 1000;

            for (int i = 0; i < 1000; i++)
            {
                int structure = rnd.Next(0,Program.structures.Length);

                int posX = rnd.Next(0, Program.structures[structure].GetLength(0));
                int posY = rnd.Next(0, Program.structures[structure].GetLength(1));
                int posZ = rnd.Next(0, Program.structures[structure].GetLength(2));

                int b = 0;
                int realBlock = -1;

                

                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        for (int z = -1; z < 2; z++)
                        {
                            if (x == y && y == z)
                            {
                                realBlock = getBlock(structure, posX, posY, posZ);
                                continue;
                            }

                            int block = getBlock(structure, posX + x, posY + y, posZ + z);

                            if (block >= 0)
                            {
                                phenome.InputSignalArray[(b * Program.palette.Length) + block] = 1.0;
                            }
                            
                            
                            b++;
                        }
                    }
                }

                if (realBlock < 0)
                {
                    i--;

                }
                else
                {
                    phenome.Activate();

                    double cost = 0;

                    for (int o = 0; o < phenome.OutputCount; o++)
                    {
                        if (o == realBlock)
                        {
                            cost += Math.Abs(1 - phenome.OutputSignalArray[o]);
                        }
                        else
                        {
                            cost += Math.Abs(phenome.OutputSignalArray[o]);
                        }
                    }

                    cost /= phenome.OutputCount;

                    fitness -= cost;
                }

            }

            fitness /= 1000;
            fitness *= 100;

            return new FitnessInfo(fitness, fitness);
        }

        private int getBlock(int structure, int posX, int posY, int posZ)
        {
            if (posX < 0 || posX >= Program.structures[structure].GetLength(0))
                return -1;
            if (posY < 0 || posY >= Program.structures[structure].GetLength(1))
                return -1;
            if (posZ < 0 || posZ >= Program.structures[structure].GetLength(2))
                return -1;
            

            return Program.structures[structure][posX, posY, posZ];
        }

        public void Reset() { }

    }
}
