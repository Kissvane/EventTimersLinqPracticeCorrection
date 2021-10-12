using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLINQAndTimers
{
    class Program
    {
        public static List<string> deads = new List<string>();

        static void Main(string[] args)
        {
            //character list initialisation
            List<Character> characters = new List<Character>();
            //name list initialisation
            List<string> names = new List<string> { "Diego", "Adrien", "Simon", "Pierre", "Paul", "Jacques", "Michel", "Uriel", "Achille", "Tom", "Shérine", "Athena", "Jeanne", "Laura" };


            //characters creation
            foreach (string name in names)
            {
                characters.Add(new Character(name));
            }

            //government creation
            Government government = new Government(characters);

            foreach (Character character in characters)
            {
                //Life start and setting life in life list
                character.StartLife();

                //choosing enemies and friends
                character.ChooseEnemiesAndFriends(characters);

                //governement subscribe on character death event to produce death certificate
                character.death += government.DeathCertificate;
            }

            //organisation of the first election
            government.Election();

            int deadsNumber = 0;
            //compare deads number to registered characters number
            while (deadsNumber < names.Count)
            {
                deadsNumber = CountTheDead(characters);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Everybody is dead");
        }

        public static int CountTheDead(List<Character> characters)
        {
            int result = 0;
            foreach (Character character in characters)
            {
                if (character.isDead) result++;
            }
            return result;
        }
    }
}
