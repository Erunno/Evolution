﻿using NUnit.Framework;
using Utils;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using Evolution;
using UtilsTests;


namespace EvolutionTests
{
    class DeafultSelectrorTests
    {
        
        [Test]
        [TestCase(50, 42)]
        [TestCase(500, 420)]
        [TestCase(1000, 402)]
        [TestCase(1500, 4200)]
        [TestCase(2000, 4020)]
        [TestCase(5000, 4002)]
        public void AddCreature_GetBestCreatures(int size,int seed)
        {
            //arrange
            NewBestCretureFoundEventDelegate<FakeCreature> newBestFound = (_, __) => { };
            RatedCreature<FakeCreature> foreFather = new RatedCreature<FakeCreature>(new FakeCreature(-1), -1);
            DisposedCreaturesStore<FakeCreature> disposedCreatures = new DisposedCreaturesStore<FakeCreature>();

            DefaultSelector<FakeCreature> selector 
                = new DefaultSelector<FakeCreature>(newBestFound, foreFather,size,disposedCreatures);
            RandomEnum rndE = new RandomEnum(seed);
            List<RatedCreature<FakeCreature>> insertedCreatures = new List<RatedCreature<FakeCreature>>();

            int limit = size * 10;
            insertedCreatures.Add(foreFather);

            foreach (var num in rndE)
            {
                if (--limit < 0) break;

                insertedCreatures.Add(new RatedCreature<FakeCreature>(new FakeCreature(num), num));
            }

            //act
            foreach (var creature in insertedCreatures)
                selector.AddCreature(creature);

            //Assert
            insertedCreatures.Sort((x,y) => y.CompareTo(x));
            var expected = insertedCreatures.Take(size).ToList();
            var actual =
                (from cr in selector.GetSurvivingCreatures()
                 orderby cr.FitnessValue descending
                 select cr).ToList();

            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
                Assert.AreEqual(expected[i].FitnessValue, actual[i].FitnessValue);
        }
    }


    class FakeCreature
    {
        public int ID { get; }

        public FakeCreature(int id)
        {
            ID = id;
        }
    }

    static class Functions
    {
        public static double FakeFitnesFunction(FakeCreature creature) => creature.ID;
    }
}
