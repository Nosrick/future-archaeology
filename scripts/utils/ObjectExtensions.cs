using System;
using System.Collections.Generic;
using System.Linq;

namespace ATimeGoneBy.scripts.utils
{
    public static class ObjectExtensions
    {
        private static readonly Random ROLLER = new Random();
        
        public static T GetRandom<T>(this ICollection<T> collection)
        {
            return collection.Count == 0 
                ? default 
                : collection.ElementAt(ROLLER.Next(collection.Count));
        }
        
    }
}