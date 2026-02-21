using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Property;
using MelonLoader;

namespace SkillTree.Core
{
    public static class Cache
    {
        // Businesses
        public static readonly Dictionary<string, float> OriginalLaunderCapacity = [];

        // Customers
        //public static readonly List<OriginalCustomer> Customers = [];
        public static readonly Dictionary<string, float> OriginalMinSpend = [];
        public static readonly Dictionary<string, float> OriginalMaxSpend = [];
        //public static readonly Dictionary<string, float> OriginalMinOrder = [];
        //public static readonly Dictionary<string, float> OriginalMaxOrder = [];

        // Items
        public static readonly Dictionary<string, int> OriginalItemStackSize = [];

        // Economy
        public static readonly Dictionary<string, float> OriginalDealerCut = [];
        public static readonly Dictionary<string, float> OriginalDealerMoveSpeed = [];

        public static void FillCache(List<Business> businesses)
        {
            if (OriginalLaunderCapacity.Count > 0)
            {
                return; 
            }

            foreach (Business business in businesses)
            {
                if (!OriginalLaunderCapacity.ContainsKey(business.PropertyName))
                {
                    OriginalLaunderCapacity.Add(business.PropertyName, business.LaunderCapacity);
                }
            }
            MelonLogger.Msg("[Cache] Successfully cached original laundering capacity for each business!");
        }

        //public class OriginalCustomer
        //{
        //    public string Name { get; set; }
        //    public float MinWeeklySpend { get; set; }
        //    public float MaxWeeklySpend { get; set; }
        //    public int MinOrdersPerWeek { get; set; }
        //    public int MaxOrdersPerWeek { get; set; }
        //}

        public static void FillCache(List<Customer> customers)
        {
            foreach (Customer customer in customers)
            {
                //if (Customers.Find(x => x.Name.Equals(customer.CustomerData.name)) == null)
                //{
                //    Customers.Add(new OriginalCustomer
                //    {
                //        Name = customer.CustomerData.name,
                //        MinWeeklySpend = customer.CustomerData.MinWeeklySpend,
                //        MaxWeeklySpend = customer.CustomerData.MaxWeeklySpend,
                //        MinOrdersPerWeek = customer.CustomerData.MinOrdersPerWeek,
                //        MaxOrdersPerWeek = customer.CustomerData.MaxOrdersPerWeek
                //    });
                //}

                if (!OriginalMinSpend.ContainsKey(customer.CustomerData.name))
                {
                    OriginalMinSpend.Add(customer.CustomerData.name, customer.CustomerData.MinWeeklySpend);
                    OriginalMaxSpend.Add(customer.CustomerData.name, customer.CustomerData.MaxWeeklySpend);
                    //OriginalMaxSpend.Add(customer.CustomerData.name, customer.CustomerData.MinOrdersPerWeek);
                    //OriginalMaxSpend.Add(customer.CustomerData.name, customer.CustomerData.MaxOrdersPerWeek);
                }
            }
            MelonLogger.Msg("[Cache] Successfully cached orignal spending capacity for each customer!");
        }

        public static void FillCache(Il2CppSystem.Collections.Generic.List<ItemDefinition> items)
        {
            foreach (ItemDefinition item in items)
            {
                if (!OriginalItemStackSize.ContainsKey(item.name))
                {
                    OriginalItemStackSize.Add(item.name, item.StackLimit);
                }
            }
            MelonLogger.Msg("[Cache] Successfully cached stack limits for each item!");
        }

        public static void FillCache(Il2CppSystem.Collections.Generic.List<Dealer> dealers)
        {
            if (OriginalDealerCut.Count > 0)
            {
                return;
            }
            
            foreach (Dealer dealer in dealers)
            {
                if (!OriginalDealerCut.ContainsKey(dealer.name))
                {
                    OriginalDealerCut.Add(dealer.name, dealer.Cut);
                    OriginalDealerMoveSpeed.Add(dealer.name, dealer.Movement.MoveSpeedMultiplier);
                }
            }
            MelonLogger.Msg("[Cache] Successfully cached dealer data!");
        }

        public static void Reset()
        {
            OriginalLaunderCapacity.Clear();
            OriginalMinSpend.Clear();
            OriginalMaxSpend.Clear();
            OriginalItemStackSize.Clear();
            OriginalDealerCut.Clear();
            OriginalDealerMoveSpeed.Clear();
            //Customers.Clear()
        }
    }
}
