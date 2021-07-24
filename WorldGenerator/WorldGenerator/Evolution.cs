using System;
using System.Collections.Generic;
using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.SpeciationStrategies;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.Phenomes;

namespace WorldGenerator
{
    class Evolution
    {
        private IGenomeFactory<NeatGenome> neatGenomeFactory;
        private List<NeatGenome> genomeList;
        private NeatEvolutionAlgorithmParameters neatParameters;
        private NeatEvolutionAlgorithm<NeatGenome> network;

        public Evolution(int inputCount, int outputCount, int creatures)
        {
            neatGenomeFactory = new NeatGenomeFactory(inputCount, outputCount);
            genomeList = neatGenomeFactory.CreateGenomeList(creatures, 0);
            neatParameters = new NeatEvolutionAlgorithmParameters
            {
                SpecieCount = creatures
            };

            EuclideanDistanceMetric dist = new EuclideanDistanceMetric();

            ParallelKMeansClusteringStrategy<NeatGenome> speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(dist);

            NullComplexityRegulationStrategy complexityRegulationStrategy = new NullComplexityRegulationStrategy();

            network = new NeatEvolutionAlgorithm<NeatGenome>(neatParameters, speciationStrategy, complexityRegulationStrategy);

            NetworkActivationScheme activationScheme = NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(1);

            NeatGenomeDecoder genomeDecoder = new NeatGenomeDecoder(activationScheme);

            Evaulator phenomeEvaluator = new Evaulator();

            ParallelGenomeListEvaluator<NeatGenome, IBlackBox> genomeListEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, phenomeEvaluator);

            network.Initialize(genomeListEvaluator, neatGenomeFactory, genomeList);

            network.UpdateScheme = new UpdateScheme(1);
            network.UpdateEvent += ea_UpdateEvent;

            
        }

        public void train()
        {
            network.StartContinue();
        }

        public void stop()
        {
            network.Stop();
        }

        static void ea_UpdateEvent(object sender, EventArgs e)
        {
            var network = (NeatEvolutionAlgorithm<NeatGenome>)sender;
            Console.WriteLine($"Generation={network.CurrentGeneration} bestFitness ={ Math.Round(network.Statistics._maxFitness,3)}% meanFitness ={ Math.Round(network.Statistics._meanFitness,3)}%");
        }
    }
}
