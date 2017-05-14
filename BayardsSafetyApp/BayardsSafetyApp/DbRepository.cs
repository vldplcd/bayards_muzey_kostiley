using System;
using System.Collections.Generic;
using BayardsSafetyApp.Entities;
using SQLite;
using Xamarin.Forms;
using System.Linq;

namespace BayardsSafetyApp
{
    public class DbRepository : IDbRepository
    {
        static SQLiteConnection context;

        public DbRepository(string filename)
        {
            string databasePath = DependencyService.Get<ISQLite>().GetDatabasePath(filename);
            context = new SQLiteConnection(databasePath);
            switch (filename)
            {
                case "bayards_sections.db":
                    context.CreateTable<Section>();
                    break;
                case "bayards_risks.db":
                    context.CreateTable<Risk>();
                    break;
                case "bayards_media.db":
                    context.CreateTable<Media>();
                    break;
            }
        }
        private string GetCurrentDb(string type)
        {

            switch (type)
            {
                case "Section":
                    return DependencyService.Get<ISQLite>().GetDatabasePath("bayards_sections.db");
                case "Risk":
                    return DependencyService.Get<ISQLite>().GetDatabasePath("bayards_risks.db");
                case "Media":
                    return DependencyService.Get<ISQLite>().GetDatabasePath("bayards_media.db");
                default:
                    throw new ArgumentException("No db for such type");
            }
        }
        public bool IsEmpty<T>() where T : new()
        {
            context = new SQLiteConnection(GetCurrentDb(typeof(T).Name));
            
            return context.Table<T>().Count() == 0;
        }
        public IEnumerable<T> GetItems<T>() where T : new()
        {
            context = new SQLiteConnection(GetCurrentDb(typeof(T).Name));
            return (from i in context.Table<T>() select i).ToList();
        }
        public T GetItem<T>(int id) where T : new()
        {
            context = new SQLiteConnection(GetCurrentDb(typeof(T).Name));
            return context.Get<T>(id);
        }
        public int DeleteItem<T>(T item)
        {
            return context.Delete(item);
        }
        public int DeleteAll<T> ()
        {
            return context.DeleteAll<T>();
        }

        public int InsertItem<T>(T item)
        {
            if (context.Insert(item) != 0)
                return context.Update(item);
            else
                return 0;
        }

        public int InsertItems<T>(List<T> items)
        {
            if (context.InsertAll(items) != 0)
                return context.UpdateAll(items);
            else
                return 0;
        }

        #region DisposeRegion
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}