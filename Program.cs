using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ACW
{

    class Program //redesign selection function in whole different way (maybe access population directly not solutions through arraylist?)
    {
        public static Random _rand = new Random(); 
        public static double[,] population;
        public static double[,] newPopulation;
        public static double[] solutions = new double[500]; //we will be working with 100 solutions (will be used to represent fitness of members) 
        public static int generation = 1; //used to count the generation we are currently on
        public static int countCreation; //to count amount of times we've created a new member

        static void Main(string[] args) //intialises population, and used to store population of future generations
        {
            File.WriteAllText("output.csv", String.Empty); //empties file for new results of GA
            population = new double[solutions.Length, 60];
            newPopulation = new double[solutions.Length, 60];
            //Create a list to store some random doubles 
            List<double> randomWeights = new List<double>();
            Random _rand = new Random();

            for (int i = 0; i < solutions.Length; i++) //used to store values in every element of population array
            {

                for (int x = 0; x < 60; x++)
                {
                    //Add 60 random doubles to the lists (between the ranges [0,1]
                    randomWeights.Add((_rand.NextDouble() * 2) - 1);


                    //These represent random weights 
                    population[i, x] = ((_rand.NextDouble() * 2) - 1);

                }


                solutions[i] = GetResults(randomWeights); //finds score from each 60 weights
                randomWeights.Clear();
                
         
            }


            do
            {
                Print(); //prints current generation 
                generation++;
                do
                {
                    Creation(); //starts processes of: selection, crossover and mutation 
                    countCreation++; //correct function
                }
                while (countCreation != solutions.Length); //checks if we've created enough children store create a new population

                countCreation = 0; //sets count of children members created to 0
                population = new double[solutions.Length, 60]; //clear old population to get ready to store current population of this generation 
                for (int i = 0; i < solutions.Length; i++) //used to store values in every element of population array
                {
                    for (int x = 0; x < 60; x++)
                    {

                        //Copy over newPopulation arrays to population 
                        population[i, x] = newPopulation[i, x];
                        randomWeights.Add(population[i, x]);

                    }


                    solutions[i] = GetResults(randomWeights); //finds score from each 60 weights
                    randomWeights.Clear();


                    //Console.WriteLine("GEN 2: " + solutions[i].ToString());
                }
                Console.WriteLine($"GEN {generation}: {solutions.Max()}");
                newPopulation = new double[solutions.Length, 60]; //makes a new newPopulation array for future storage
            }
            while (generation != 13); //10 generations made
            Console.ReadLine();
        }
        public static double GetRandomNumber(double min, double max) //creates new random generated values 
        {
            return (_rand.NextDouble() * (max - min)) + min;
            
        }
        public static void Print()//will mainly print doubles if not string is ok
        {

            using (StreamWriter sw = new StreamWriter("output.csv", true))//writes to CSV and appends data
            {
                //if(generation == 1)
                //{
                //    sw.WriteLine("{0},{1},{2}", "Member", "MemberGen", "MemberFitness");
                //}
                for(int i = 0; i < population.GetLength(0); i++)
                {
                    //sw.WriteLine("{0},{1},{2}", i.ToString(), generation.ToString(), solutions[i].ToString());
                    sw.Write(solutions[i].ToString() + ',');
                }
                sw.WriteLine();

            }

        }


        public static void Creation()//selects 4 members
        {
            Random numGen = new Random();

            double[] tournament = new double[100]; //sets our tournament size 
            int[] indexSelect = new int[tournament.Length]; //will be used to store unique integers equal to size of our tournament 
            int temp = 0;
            double[,] tourneyInfo = new double[tournament.Length, 2]; //will store the member index and their fitness score



            for (int i = 0; i < tournament.Length; i++)
            {
                temp = numGen.Next(0, solutions.Length - 1); //stores a random number

                if (i == 0 || indexSelect.Contains(temp) == false) //i==0 essentially is to intialise process of checking duplicate indexes
                {
                    indexSelect[i] = temp; //stores the random numebr which is unique 
                    tournament[i] = solutions[indexSelect[i]]; //stores the fitness of the member at the index
                    for (int x = 0; x < 1; x++)
                    {
                        tourneyInfo[i, x] = indexSelect[i]; //stores in correct position 
                        tourneyInfo[i, 1] = tournament[i]; //does not store in 2nd dimension     
                    }
                }
                else//if the random number has already been picked then reiterate by doing i--, do till we find a non-duplicate number
                {

                    i--;


                }
            }
            //compare the solution scores, remove the 2 lowest
            Array.Sort(tournament); //arranges values from lowest to highest
            Array.Reverse(tournament); //rearrange to make values go from highest to lowest

            double[,] parents = new double[2, 2]; //pick the indexes from the 2 better solutions
            int[] parentsIndex = new int[2];

            for (int i = 0; i < tournament.Length; i++)
            {
                if (tourneyInfo[i, 1] == tournament[0])//overwrites current best stored
                {
                    parents[0, 0] = tourneyInfo[i, 0];
                    parents[0, 1] = tourneyInfo[i, 1];
                    //remove highest value so we don't look at anymore

                }
                if (tourneyInfo[i, 1] == tournament[1])
                {
                    parents[1, 0] = tourneyInfo[i, 0];
                    parents[1, 1] = tourneyInfo[i, 1];
                }
            }



            parentsIndex[0] = (int)parents[0, 0];
            parentsIndex[1] = (int)parents[1, 0];
            //Creation(parentsIndex, parents);

            List<double> tempChild = new List<double>();
            double randToBreed = GetRandomNumber(0, 1); //to see if crossover is applied

            if (randToBreed >= 0.50)//Crossover the two parents
            {
                //take first half of parent A, second half from parent B
                for (int i = 0; i < 60; i++)
                {
                    if (i <= 29)
                    {
                        tempChild.Add(population[parentsIndex[0], i]);
                    }
                    else if (i > 29)
                    {
                        tempChild.Add(population[parentsIndex[1], i]);
                    }
                }
                //add weights correctly
            }
            else//Do not breed but rahter copy from Parent A
            {
                for (int i = 0; i < 60; i++)
                {
                    tempChild.Add(population[parentsIndex[0], i]); //make child = first parent from selection method
                }
            }


            double mutationRate = 0.055; //sets our mutation ration
            double toMutate;
            double[] newChild = new double[tempChild.Count];
            tempChild.CopyTo(newChild); 



            for (int i = 0; i < newChild.Length; i++)
            {
                toMutate = GetRandomNumber(0, 1);
                if (toMutate < mutationRate)//if random gen less than our mutation rate then mutate (generate new weight at gene/weight)
                {
                    newChild[i] = _rand.NextDouble() * 2 - 1;
                }
            }

            //child ready to be a member in new population - copy over 
            //when all created and added make population = newpopulation
            //clear new population 


            Console.WriteLine(GetResults(tempChild));

            for (int i = 0; i < newChild.Length; i++)
            {
                newPopulation[countCreation, i] = newChild[i]; //copy over each offspring to newPopulation, do this X amount of times till we have a full population to copy over to the next generation  
            }
           

        }

        public static double GetResults(List<double> weights)//Function to get fitness for each member in our population
        {
            Network net = new Network(); //calls Network class in which a neural network will be created intialised

            net.SetWeights(weights);

            PendulumMaths p = new PendulumMaths();
            p.initialise(1);

            Network v = new Network();
            v.SetWeights(net.GetWeights());

            double[][] motor_vals = new double[p.getcrabnum()][];

            for (int i = 0; i < motor_vals.Length; i++)
            {
                motor_vals[i] = new double[2];
            }

            do
            {
                double[][] sval = (p.getSensorValues());

                double[] inputs = new double[10];

                for (int i = 0; i < p.getcrabnum(); i++)
                {

                    for (int x = 0; x < sval[0].Length; x++)
                    {
                        inputs[x] = ((sval[i][x]) / (127) * (1 - 0)) + 1;
                    }

                    v.SetInputs(inputs);

                    v.Execute();

                    double[] outputs = v.GetOutputs();

                    motor_vals[i][0] = ((outputs[0])) * 127;
                    motor_vals[i][1] = ((outputs[1])) * 127;

                }

            }
            while (p.performOneStep(motor_vals) == 1);

            return p.getFitness();
        }
    }
}