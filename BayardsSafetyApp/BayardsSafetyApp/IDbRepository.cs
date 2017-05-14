using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BayardsSafetyApp
{
    public interface IDbRepository : IDisposable
    {
        bool IsEmpty<T>() where T : new();
        int InsertItem<T>(T item);
        int InsertItems<T>(List<T> items);
        int DeleteItem<T>(T model);
        int DeleteAll<T>();
        T GetItem<T>(int id) where T : new();
        IEnumerable<T> GetItems<T>() where T : new();
    }
}