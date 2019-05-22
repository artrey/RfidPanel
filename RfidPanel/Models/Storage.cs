using System;
using System.Collections.Generic;
using SQLite;

namespace RfidPanel.Models
{
    public class Storage
    {
        private readonly SQLiteConnection _sql;

        public Storage(string path)
        {
            _sql = new SQLiteConnection(path);
            _sql.CreateTable<Person>();
            _sql.CreateTable<Check>();
        }

        public IEnumerable<Check> Checks(Person p)
        {
            return _sql.Table<Check>().Where(v => v.PersonUid == p.Uid);
        }

        public T Add<T>(T item)
        {
            _sql.Insert(item);
            return item;
        }

        public Person FindPerson(string uid)
        {
            return _sql.Find<Person>(uid);
        }

        public Check AddMark(Person p, DateTime time)
        {
            return Add(new Check {PersonUid = p.Uid, Time = time});
        }

        public Check AddMark(string uid, DateTime time)
        {
            var p = FindPerson(uid);
            return p == null ? null : AddMark(p, time);
        }
    }
}
