using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLINQAndTimers
{
    class Popularity
    {
        public int seenAsAFriend = 0;
        public int seenAsAnEnemy = 0;
    }

    class Government
    {
        public Dictionary<Character, Popularity> popularityStats = new Dictionary<Character, Popularity>();
        //register of the living people
        public List<Character> livingPeople;

        //the current president
        //easily accessible as a static variable
        public static Character president;

        //constructor
        public Government(List<Character> characters)
        {
            livingPeople = new List<Character>();
            livingPeople.AddRange(characters);
        }

        //when somebody die the governement produce a death certifcate
        public void DeathCertificate(Object sender, DeathEventArgs args)
        {
            livingPeople.Remove(args.Body);
            //if the dead is the president
            if (args.Body == president)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                //announce his death, death date and last words
                Console.WriteLine("Governemental announcement : {0} the president is Dead the {1}.\nHis last words were \"{2}\"", president.Name, args.DeathNote.ToLongDateString(), args.LastWords);
                Console.ForegroundColor = ConsoleColor.White;
                //elect a new president if there is more than 2 leaving people
                if (livingPeople.Count > 2)
                    Election();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                //announce his death
                Console.WriteLine("Governemental announcement : {0} is dead.", args.Body.Name);
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        //organize an election
        public void Election()
        {
            List<Character> population = livingPeople;

            //calculate the popularity for each living character
            foreach (Character character in livingPeople)
            {
                CalculatePopularity(character, livingPeople);
            }

            //sorting people by number of friends and enemies
            population.OrderBy(x => popularityStats[x].seenAsAFriend).ThenByDescending(x => popularityStats[x].seenAsAnEnemy);
            
            //select the first as president
            president = population[0];
            //announce the new president
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("{0} is the new president.", president.Name);
            Console.ForegroundColor = ConsoleColor.White;
            //unsuscribe everybody on president death (population and governement)
            president.BreakLinks();
            //remove president from the current list
            population.Remove(president);

            //make the governement aware of the president death again
            president.death += DeathCertificate;

            foreach (Character character in population)
            {
                //make everybody in the list honour the president when he dies
                president.death += character.Honour;
            }
        }

        public void CalculatePopularity(Character character, List<Character> people)
        {
            popularityStats[character] = new Popularity();
            //duplicate list to avoid modifying the orignal one
            List<Character> characters = new List<Character>(people);
            //remove the current characetr from the duplicated list
            characters.Remove(character);
            foreach (Character current in characters)
            {
                if (!current.isDead) 
                {
                    if (current.friends.Contains(character))
                        popularityStats[character].seenAsAFriend++;
                    else if (current.enemies.Contains(character))
                        popularityStats[character].seenAsAnEnemy++;
                }
            }
        }

    }
}
