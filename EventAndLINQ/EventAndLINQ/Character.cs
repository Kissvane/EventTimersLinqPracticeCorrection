using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EventLINQAndTimers
{
    public class DeathEventArgs : EventArgs
    {
        public Character Body { get; set; }
        public DateTime DeathNote { get; set; }
        public string LastWords { get; set; }
    }

    public delegate void DeathEventHandler(Object sender, DeathEventArgs e);

    public class Character
    {
        public string Name;
        public int LifeTime;
        public bool isDead;
        public HashSet<Character> friends =  new HashSet<Character>();
        public HashSet<Character> enemies = new HashSet<Character>();
        Random rand;

        #region Death event

        public event DeathEventHandler death;

        protected virtual void OnCharacterDead(DeathEventArgs e)
        {
            DeathEventHandler handler = death;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region constructors

        //defined lifetime constructor
        public Character(string name, int lifetime)
        {
            Name = name;
            //create a random customized for this character
            rand = new Random(NameToInt() + (int)DateTime.Now.Ticks);
            LifeTime = lifetime;
        }

        //random lifetime constructor
        public Character(string name)
        {
            Name = name;
            //create a random customized for this character
            rand = new Random(NameToInt() + (int)DateTime.Now.Ticks);
            LifeTime = rand.Next(500, 5000);
        }

        #endregion

        #region Life management

        //Lifetime management
        public void StartLife()
        {
            isDead = false;
            Console.WriteLine("{0} starts his life.", Name);
            //wait for lifetime duration
            Timer timer = new Timer(LifeTime);
            timer.Enabled = true;
            timer.AutoReset = false;
            timer.Elapsed += OnEndLifeEvent;
        }

        private void OnEndLifeEvent(Object source, ElapsedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("{0} is dead.", Name);
            Console.ForegroundColor = ConsoleColor.White;
            isDead = true;
            //I don't need to cry fro my friends anymore
            //because i'm dead
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (Character friend in friends)
            {
                friend.death -= Cry;
                Console.WriteLine("{0} unsuscribe from friend {1}", Name, friend.Name);
            }

            //I don't need to laugh at my enemies anymore
            //because i'm dead
            foreach (Character enemy in enemies)
            {
                enemy.death -= Enjoy;
                Console.WriteLine("{0} unsuscribe from enemy {1}", Name, enemy.Name);
            }

            //I don't need to honour the president
            //because i'm dead
            Government.president.death -= Honour;

            Console.ForegroundColor = ConsoleColor.White;
            DeathEventArgs args = new DeathEventArgs();
            args.Body = this;
            args.DeathNote = DateTime.Now;
            args.LastWords = GetLastWords();
            //send death event to all listeners  
            OnCharacterDead(args);
        }

        //breakings links between this character and the other
        //After that nobody will cry or laugh when he dies
        public void BreakLinks()
        {
            death = null;
        }

        #endregion

        #region Death reactions

        public void Enjoy(Object sender, DeathEventArgs args)
        {
            Console.WriteLine("{0} enjoy the death of {1}", Name, args.Body.Name);
            enemies.Remove(args.Body);
            args.Body.death -= Enjoy;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} unsuscribe from enjoying {1}'s death", Name, args.Body.Name);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Cry(Object sender, DeathEventArgs args)
        {
            Console.WriteLine("{0} cry for {1}", Name, args.Body.Name);
            friends.Remove(args.Body);
            args.Body.death -= Cry;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} unsuscribe from crying {1}", Name, args.Body.Name);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Honour(Object sender, DeathEventArgs args)
        {
            if (enemies.Contains(args.Body))
            {
                enemies.Remove(args.Body);
                Console.WriteLine("{0} stay at home during {1} death", Name, args.Body.Name);
            }
            else
            {
                if (friends.Contains(args.Body))
                    friends.Remove(args.Body);
                Console.WriteLine("{0} honour the name of {1}", Name, args.Body.Name);
            }
            args.Body.death -= Honour;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} unsuscribe from honouring {1}", Name, args.Body.Name);
            Console.ForegroundColor = ConsoleColor.White;
        }

        #endregion

        #region Enemies and friends attribution

        #region defined

        //start friend and enemies selection
        public void ChooseEnemiesAndFriends(List<Character> characters, int enemiesNumber, int friendsNumber)
        {
            //copy characters list to prevent the modification of the list in Program.cs
            List<Character> temp = new List<Character>();
            temp.AddRange(characters);
            //remove the current character from the list
            temp.Remove(this);
            //choose enemies
            ChooseNEnemies(temp, enemiesNumber);
            //choose friends
            ChooseNFriends(temp, friendsNumber);
        }

        public void ChooseNEnemies(List<Character> characters, int enemiesNumber)
        {
            //get characters who don't think I'm an friend
            IEnumerable<Character> query =
                from c in characters
                where !c.friends.Contains(this)
                select c;

            //transform query result in list
            List<Character> PossibleEnemies = query.ToList();

            //limit the number of enemies to available enemies
            enemiesNumber = Math.Min(enemiesNumber, PossibleEnemies.Count);

            for (int i = 0; i < enemiesNumber; i++)
            {
                //select a random enemy
                Character selected = PossibleEnemies[rand.Next(0, PossibleEnemies.Count)];
                //this character is an enemy now
                enemies.Add(selected);
                //I will enjoy his death
                selected.death += Enjoy;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} is enemy with {1}", Name, selected.Name);
                Console.ForegroundColor = ConsoleColor.White;
                //remove this character from the copy list so he won't be reselected in next loop
                PossibleEnemies.Remove(selected);
                //remove this character from the original list so he won't be processing when choosing enemies
                characters.Remove(selected);
            }
        }

        public void ChooseNFriends(List<Character> characters, int friendsNumber)
        {
            //get characters who don't think I'm an enemy 
            IEnumerable<Character> query =
                from c in characters
                where !c.enemies.Contains(this)
                select c;

            //transform query result in list
            List<Character> Friendable = query.ToList();

            //limit the number of friends to available friends
            friendsNumber = Math.Min(friendsNumber, Friendable.Count);

            for (int i = 0; i < friendsNumber; i++)
            {
                //select a random friend
                Character selected = Friendable[rand.Next(0, Friendable.Count)];
                //this character is an friend now
                friends.Add(selected);
                //I will cry for his death
                selected.death += Cry;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} is friend with {1}", Name, selected.Name);
                Console.ForegroundColor = ConsoleColor.White;
                //remove this character from the copy list so he won't be reselected in next loop
                Friendable.Remove(selected);
            }
        }

        #endregion

        #region random

        //start friend and enemies selection
        public void ChooseEnemiesAndFriends(List<Character> characters)
        {
            //copy characters list to prevent the modification of the list in Program.cs
            List<Character> temp = new List<Character>();
            temp.AddRange(characters);
            //remove the current character from the list
            temp.Remove(this);

            //choose enemies
            ChooseNEnemies(temp);
            //choose friends
            ChooseNFriends(temp);
        }

        public void ChooseNEnemies(List<Character> characters)
        {
            //get characters who don't think I'm an friend
            IEnumerable<Character> query =
                from c in characters
                where !c.friends.Contains(this)
                select c;

            //transform query result in list
            List<Character> PossibleEnemies = query.ToList();

            //limit the number of enemies to available enemies
            int enemiesNumber = rand.Next(0, PossibleEnemies.Count);

            for (int i = 0; i < enemiesNumber; i++)
            {
                //select a random enemy
                Character selected = PossibleEnemies[rand.Next(0, PossibleEnemies.Count)];
                //this character is an enemy now
                enemies.Add(selected);
                //I will enjoy his death
                selected.death += Enjoy;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} is enemy with {1}", Name, selected.Name);
                Console.ForegroundColor = ConsoleColor.White;
                //remove this character from the copy list so he won't be reselected in next loop
                PossibleEnemies.Remove(selected);
                //remove this character from the original list so he won't be processing when choosing enemies
                characters.Remove(selected);
            }
        }

        public void ChooseNFriends(List<Character> characters)
        {
            //get characters who don't think I'm an enemy
            IEnumerable<Character> query =
                from c in characters
                where !c.enemies.Contains(this)
                select c;

            //transform query result in list
            List<Character> Friendable = query.ToList();

            //limit the number of friends to available friends
            int friendsNumber = rand.Next(0, Friendable.Count);

            for (int i = 0; i < friendsNumber; i++)
            {
                //select a random friend
                Character selected = Friendable[rand.Next(0, Friendable.Count)];
                //this character is an friend now
                friends.Add(selected);
                //I will cry for his death
                selected.death += Cry;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} is friend with {1}", Name, selected.Name);
                Console.ForegroundColor = ConsoleColor.White;
                //remove this character from the copy list so he won't be reselected in next loop
                Friendable.Remove(selected);
            }
        }

        #endregion

        #endregion

        #region tools
        //transform Name in int
        int NameToInt()
        {
            int result = 0;
            foreach (char c in Name)
            {
                result += c;
            }
            return result;
        }

        public string GetLastWords()
        {
            int temp = rand.Next(0, 10000);
            string result = "";
            while (temp > 0)
            {
                int value = rand.Next(65, 90);
                result += (char)value;
                temp -= value;
            }

            return result;
        }

        #endregion
    }
}
