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
            // auto generating database and tables if not exists
            _sql = new SQLiteConnection(path);
            _sql.CreateTable<Person>();
            _sql.CreateTable<Check>();
        }

        public IEnumerable<Check> Checks(Person p)
        {
            // find all visits by person
            return _sql.Table<Check>().Where(v => v.PersonUid == p.Uid);
        }

        public T Add<T>(T item)
        {
            // insert any (Person or Check) object to database
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

        public IEnumerable<Person> Persons()
        {
            // return all persons ordered by department (for pretty output)
            return _sql.Table<Person>().OrderBy(p => p.Department);
        }

        public void RemovePerson(Person p)
        {
            // first, remove all visits of person
            foreach (var c in Checks(p))
            {
                _sql.Delete<Check>(c.Id);
            }
            // then remove person
            _sql.Delete<Person>(p.Uid);
        }
    }
}
