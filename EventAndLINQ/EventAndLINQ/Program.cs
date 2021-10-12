using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLINQAndTimers
{
    class Program
    {
        static int deadcount = 0;

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
                character.IsDead += government.DeathCertificate;
                character.IsDead += countTheDead;
            }

            //organisation of the first election
            government.Election();

            while (deadcount < characters.Count)
            {

            }

            Console.WriteLine("Everybody is dead");
        }

        public static void countTheDead(Object sender, DeathEventArgs args)
        {
            deadcount++;
        }
    }
}
