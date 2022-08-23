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
            colosseum.StartTheShow();
        }
    }

    class Colosseum
    {
        private Gladiator _gladiator1;
        private Gladiator _gladiator2;

        public Colosseum()
        {
            _gladiator1 = null;
            _gladiator2 = null;
        }

        public void StartTheShow()
        {
            ChooseFighters();
            Fight();
            ShowWinner();
        }

        private void ChooseFighters()
        {
            _gladiator1 = ChooseGladiator(_gladiator1);
            _gladiator2 = ChooseGladiator(_gladiator2);
            ShowChosens();
        }

        private void ShowChosens()
        {
            Console.Clear();
            Console.WriteLine("Сегодня на арене Коллизея сойдутся в кровавом поединке:");
            СhangeMessageСolor(_gladiator1.ShowStats(), ConsoleColor.Green);
            СhangeMessageСolor(_gladiator2.ShowStats(), ConsoleColor.Yellow);
            Console.ReadKey(true);
        }

        private void Fight()
        {
            while (_gladiator1.GetHealth() > 0 && _gladiator2.GetHealth() > 0)
            {
                _gladiator2.TakeDamage(_gladiator1.Attack());
                _gladiator1.UseAbility(out bool isEntangled, out int damage);
                _gladiator2.TakeAbility(isEntangled, damage);
                _gladiator1.TakeDamage(_gladiator2.Attack());
                _gladiator2.UseAbility(out isEntangled, out damage);
                _gladiator1.TakeAbility(isEntangled, damage);
                СhangeMessageСolor(_gladiator1.ShowStats(), ConsoleColor.Green);
                СhangeMessageСolor(_gladiator2.ShowStats(), ConsoleColor.Yellow);
            }
        }

        private void ShowWinner()
        {
            if (_gladiator2.GetHealth() <= 0 && _gladiator1.GetHealth() <= 0)
            {
                СhangeMessageСolor($"Гладиаторы {_gladiator1.GetClassName()} и {_gladiator2.GetClassName()} убили друг друга!", ConsoleColor.Red);
            }
            else if (_gladiator1.GetHealth() <= 0)
            {
                СhangeMessageСolor($"Победитель: {_gladiator2.GetClassName()}!", ConsoleColor.Yellow);
            }
            else
            {
                СhangeMessageСolor($"Победитель: {_gladiator1.GetClassName()}!", ConsoleColor.Green);
            }
        }

        private Gladiator ChooseGladiator(Gladiator gladiator)
        {
            while (gladiator == null)
            {
                Console.Clear();
                Console.WriteLine("Благородный патриций, добро пожаловать в Коллизей! Ваш выбор бойцов: \n1) выбрать Фракийца \n2) выбрать Мирмиллона " +
                    " \n3) выбрать Ретиария \n4) выбрать Секутора \n5) выбрать Димахера \n6) выбрать Велита");
                ConsoleKeyInfo key = Console.ReadKey(true);
                Console.Clear();

                switch (key.KeyChar)
                {
                    case '1':
                        gladiator = new Thraex();
                        break;
                    case '2':
                        gladiator = new Murmillo();
                        break;
                    case '3':
                        gladiator = new Retiarius();
                        break;
                    case '4':
                        gladiator = new Secutor();
                        break;
                    case '5':
                        gladiator = new Dimachaerus();
                        break;
                    case '6':
                        gladiator = new Veles();
                        break;
                    default:                       
                        break;
                }
            }

            Console.WriteLine("Вы выбрали: " + gladiator.GetClassName());
            Console.ReadKey(true);
            return gladiator;
        }       

        private void СhangeMessageСolor(string message, ConsoleColor сolor)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = сolor;
            Console.WriteLine(message);
            Console.ForegroundColor = defaultColor;
        }
    }

    class Gladiator
    {
        protected string ClassName;
        protected int Health;
        protected int Damage;
        protected int Armor;
        protected int Block;
        protected int ChanceAbility;
        protected bool IsEntangled;
        private int _unentangleChance;
        private Random _random = new Random();

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

        public string GetClassName()
        {
            return ClassName;
        }

        public int GetHealth()
        {
            return Health;
        }

        public virtual int Attack()
        {
            if (CheckToEntangle())
            {
                return 0;
            }

            ShowAttackInfo();
            return Damage;
        }


        public bool CheckToEntangle()
        {
            if (IsEntangled)
            {
                Console.Write($"{GetClassName()} опутан сетью и пытается освободиться: ");
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
                    Console.WriteLine($"{GetClassName()} не получил урона");
                }
                else
                {
                    Console.WriteLine($"Броня заблокировала {Armor} урона, {GetClassName()} получил {damageDone} урона");
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
            Console.WriteLine($"{GetClassName()} ударил мечом на {Damage} урона");
        }

        protected virtual void ShowBlockInfo()
        {
            Console.WriteLine($"{GetClassName()} заблокировал атаку противника щитом");
        }        

        protected bool WasItSuccessful(int chance)
        {
            int random = _random.Next(101);

            if (random <= chance)
            {
                return true;
            }

            return false;
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
                Console.WriteLine($"{GetClassName()} изучил слабые стороны противника и будет наносить повышенный урон");
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
                Console.WriteLine($"{GetClassName()} изучил движения противника и будет теперь более сложной целью");
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
                Console.WriteLine($"{GetClassName()} опутал противника сетью");
                isEntangled = true;
                ChanceAbility = 0;
            }
        }

        public override void ShowAttackInfo()
        {
            Console.WriteLine($"{GetClassName()} ударил трезубцем на {Damage} урона");
        }

        protected override void ShowBlockInfo()
        {
            Console.WriteLine($"Противник опутан сетью и не может попать по {GetClassName()}");
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
                damage = Attack();
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
            Console.WriteLine($"{GetClassName()} парировал атаку мечом");
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

        public override int Attack()
        {
            if (CheckToEntangle())
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
            Console.WriteLine($"{GetClassName()} ударил копьем на {Damage} урона");
        }

        protected override void ShowBlockInfo()
        {
            Console.WriteLine($"{GetClassName()} отбежал в сторону и увернулся от атаки противника");
        }

        private int ThrowJavelin()
        {
            _javelins--;

            if (WasItSuccessful(_chanceJavelinHit))
            {
                Console.WriteLine($"{GetClassName()} бросил дротик и нанес {_javelinDamage} урона");
                return _javelinDamage;
            }
            else
            {
                Console.WriteLine($"{GetClassName()} бросил дротик и промахнулся");
                return 0;
            }
        }
    }
}
