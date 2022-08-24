using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colosseum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Colosseum colosseum = new Colosseum();
            colosseum.StartShow();
        }
    }

    class Colosseum
    {
        private Gladiator _gladiator1;
        private Gladiator _gladiator2;

        public void StartShow()
        {
            ChooseFighters();
            Fight();
            ShowWinner();
        }

        private void ChooseFighters()
        {
            _gladiator1 = ChooseGladiator();
            _gladiator2 = ChooseGladiator();
            ShowChosens();
        }

        private void ShowChosens()
        {
            Console.Clear();
            Console.WriteLine("Сегодня на арене Коллизея сойдутся в кровавом поединке:");
            ShowColorMessage(_gladiator1.ShowStats(), ConsoleColor.Green);
            ShowColorMessage(_gladiator2.ShowStats(), ConsoleColor.Yellow);
            Console.ReadKey(true);
        }

        private void Fight()
        {
            while (_gladiator1.Health > 0 && _gladiator2.Health > 0)
            {
                _gladiator2.TakeDamage(_gladiator1.GetDamage());
                _gladiator1.UseAbility(out bool isEntangled, out int damage);
                _gladiator2.TakeAbility(isEntangled, damage);
                _gladiator1.TakeDamage(_gladiator2.GetDamage());
                _gladiator2.UseAbility(out isEntangled, out damage);
                _gladiator1.TakeAbility(isEntangled, damage);
                ShowColorMessage(_gladiator1.ShowStats(), ConsoleColor.Green);
                ShowColorMessage(_gladiator2.ShowStats(), ConsoleColor.Yellow);
            }
        }

        private void ShowWinner()
        {
            if (_gladiator2.Health <= 0 && _gladiator1.Health <= 0)
            {
                ShowColorMessage($"Гладиаторы {_gladiator1.ClassName} и {_gladiator2.ClassName} убили друг друга!", ConsoleColor.Red);
            }
            else if (_gladiator1.Health <= 0)
            {
                ShowColorMessage($"Победитель: {_gladiator2.ClassName}!", ConsoleColor.Yellow);
            }
            else
            {
                ShowColorMessage($"Победитель: {_gladiator1.ClassName}!", ConsoleColor.Green);
            }
        }

        private Gladiator ChooseGladiator()
        {
            Gladiator[] gladiators = { new Thraex(), new Murmillo(), new Retiarius(), new Secutor(), new Dimachaerus(), new Veles() };
           
            Console.Clear();
            Console.WriteLine("Благородный патриций, добро пожаловать в Коллизей!");

            for (int i = 0; i < gladiators.Length; i++)
            {
                Console.WriteLine($"{i+1}) выбрать {gladiators[i].ClassName}");
            }
           
            int index = GetIndex() - 1;
            Console.Clear();
            Console.WriteLine("Вы выбрали: " + gladiators[index].ClassName);
            Console.ReadKey(true);
            return gladiators[index];
        }       

        private int GetIndex()
        {
            int index = 0;
            bool success = false;
            bool isCorrect = false;

            Console.SetCursorPosition(0, 9);

            while (isCorrect == false)
            {
                Console.WriteLine("Ваш выбор бойцов:");
                string userInput = Console.ReadLine();
                success = int.TryParse(userInput, out index);

                if (success && index > 0 && index <= 6)
                {
                    isCorrect = true;
                }
                else
                {
                    Console.WriteLine("Такого гладиатора нет");
                }
            }

            return index;
        }

        private void ShowColorMessage(string message, ConsoleColor сolor)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = сolor;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }
    }

    class Gladiator
    {
        protected int Damage;
        protected int Armor;
        protected int Block;
        protected int ChanceAbility;
        protected bool IsEntangled;
        private int _unentangleChance;
        private Random _random = new Random();

        public string ClassName { get; protected set; }
        public int Health { get; protected set; }

        public Gladiator(string className, int health, int damage, int armor, int block)
        {
            ClassName = className;
            Health = health;
            Damage = damage;
            Armor = armor;
            Block = block;
            IsEntangled = false;
            _unentangleChance = 50;
        }     


        public virtual int GetDamage()
        {
            if (IsInNet())
            {
                return 0;
            }

            ShowAttackInfo();
            return Damage;
        }


        public bool IsInNet()
        {
            if (IsEntangled)
            {
                Console.Write($"{ClassName} опутан сетью и пытается освободиться: ");
                Unentangle();
                return true;
            }

            return false;
        }

        public void TakeDamage(int damage)
        {
            if (WasItSuccessful(Block))
            {
                ShowBlockInfo();
            }
            else
            {
                int damageDone = damage - Armor;

                if (damageDone <= 0)
                {
                    Console.WriteLine($"{ClassName} не получил урона");
                }
                else
                {
                    Console.WriteLine($"Броня заблокировала {Armor} урона, {ClassName} получил {damageDone} урона");
                    Health -= (damageDone);
                }
            }
        }

        public string ShowStats()
        {
            return ($"{ClassName} |HP : {Health}| |DMG : {Damage}| |Armor : {Armor}| |Block : {Block}|");
        }


        public virtual void UseAbility(out bool isEntangled, out int damage)
        {
            isEntangled = false;
            damage = 0;
        }

        public virtual void TakeAbility(bool isEntangled, int damage)
        {
            if (damage != 0)
            {
                TakeDamage(damage);
            }

            if (isEntangled)
            {
                IsEntangled = true;
            }
        }
        public virtual void ShowAttackInfo()
        {
            Console.WriteLine($"{ClassName} ударил мечом на {Damage} урона");
        }

        protected virtual void ShowBlockInfo()
        {
            Console.WriteLine($"{ClassName} заблокировал атаку противника щитом");
        }        

        protected bool WasItSuccessful(int chance)
        {
            int random = _random.Next(101);    
            return (random <= chance);
        }

        private void Unentangle()
        {
            if (WasItSuccessful(_unentangleChance))
            {
                Console.WriteLine("Успешно");
                IsEntangled = false;
            }
            else
            {
                Console.WriteLine("барахтается в сети");
            }
        }
    }

    class Thraex : Gladiator
    {
        private int _damageModifieter;
        private int _maxDamage;

        public Thraex() : base("Фракиец", 100, 5, 7, 10)
        {
            _damageModifieter = 10;
            _maxDamage = 45;
            ChanceAbility = 45;
        }

        public override void UseAbility(out bool isEntangled, out int damage)
        {
            isEntangled = false;
            damage = 0;

            if (WasItSuccessful(ChanceAbility) && IsEntangled == false && Damage <= _maxDamage)
            {
                Console.WriteLine($"{ClassName} изучил слабые стороны противника и будет наносить повышенный урон");
                Damage += _damageModifieter;
            }
        }
    }

    class Murmillo : Gladiator
    {
        private int _blockModifieter;
        private int _maxBlock;

        public Murmillo() : base("Мирмиллон", 100, 15, 3, 20)
        {
            _blockModifieter = 10;
            _maxBlock = 70;
            ChanceAbility = 40;
        }

        public override void UseAbility(out bool isEntangled, out int damage)
        {
            isEntangled = false;
            damage = 0;

            if (WasItSuccessful(ChanceAbility) && IsEntangled == false && Block <= _maxBlock)
            {
                Block += _blockModifieter;
                Console.WriteLine($"{ClassName} изучил движения противника и будет теперь более сложной целью");
            }
        }
    }

    class Retiarius : Gladiator
    {
        public Retiarius() : base("Ретиарий", 100, 25, 0, 0)
        {
            ChanceAbility = 30;
        }

        public override void UseAbility(out bool isEntangled, out int damage)
        {
            isEntangled = false;
            damage = 0;

            if (WasItSuccessful(ChanceAbility))
            {
                Console.WriteLine($"{ClassName} опутал противника сетью");
                isEntangled = true;
                ChanceAbility = 0;
            }
        }

        public override void ShowAttackInfo()
        {
            Console.WriteLine($"{ClassName} ударил трезубцем на {Damage} урона");
        }

        protected override void ShowBlockInfo()
        {
            Console.WriteLine($"Противник опутан сетью и не может попать по {ClassName}");
        }
    }

    class Secutor : Gladiator
    {
        public Secutor() : base("Секутор", 110, 20, 5, 30)
        {

        }
    }

    class Dimachaerus : Gladiator
    {
        private int _stamina;

        public Dimachaerus() : base("Димахер", 100, 15, 7, 5)
        {
            _stamina = 150;
        }

        public override void UseAbility(out bool success, out int damage)
        {
            success = false;
            damage = 0;

            if (_stamina > 0)
            {
                Console.Write("Атака вторым мечом:");
                damage = GetDamage();
                _stamina -= 50;
            }
            else
            {
                Console.WriteLine("Димахер слишком устал для второй атаки");
                _stamina += 35;
            }
        }

        protected override void ShowBlockInfo()
        {
            Console.WriteLine($"{ClassName} парировал атаку мечом");
        }
    }

    class Veles : Gladiator
    {
        private int _chanceJavelinHit;
        private int _javelins;
        private int _javelinDamage;

        public Veles() : base("Велит", 100, 15, 0, 50)
        {
            _chanceJavelinHit = 50;
            _javelins = 5;
            _javelinDamage = 40;
        }

        public override int GetDamage()
        {
            if (IsInNet())
            {
                return 0;
            }

            if (_javelins > 0)
            {
                return ThrowJavelin();
            }

            ShowAttackInfo();
            return Damage;
        }

        public override void ShowAttackInfo()
        {
            Console.WriteLine($"{ClassName} ударил копьем на {Damage} урона");
        }

        protected override void ShowBlockInfo()
        {
            Console.WriteLine($"{ClassName} отбежал в сторону и увернулся от атаки противника");
        }

        private int ThrowJavelin()
        {
            _javelins--;

            if (WasItSuccessful(_chanceJavelinHit))
            {
                Console.WriteLine($"{ClassName} бросил дротик и нанес {_javelinDamage} урона");
                return _javelinDamage;
            }
            else
            {
                Console.WriteLine($"{ClassName} бросил дротик и промахнулся");
                return 0;
            }
        }
    }
}
