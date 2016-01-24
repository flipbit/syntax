using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Syntax.Data
{
    [TestFixture]
    public class DataTests
    {
        protected IDbConnection Connection;
        protected string Filename;

        [SetUp]
        public void Setup()
        {
            Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sqlite-test.db");

            var connectionString = string.Format("Data Source={0};Version=3;", Filename);

            SQLiteConnection.CreateFile(Filename);

            Connection = new SQLiteConnection(connectionString);

            Connection.Open();

            Connection.ExecuteNonQuery("CREATE TABLE [Person] ([Name] TEXT, [Age] INT);");
        }

        [TearDown]
        public void TearDown()
        {
            Connection.Close();

            Connection.Dispose();

            File.Delete(Filename);
        }

        [Test]
        public void TestInsert()
        {
            var person = new Person { Name = "Bob", Age = 50 };

            Connection
                .Insert(person)
                .Column(c => c.Name)
                .Column(c => c.Age)
                .ExecuteNonQuery();

            var saved = Connection
                .Select<Person>()
                .Column(c => c.Name)
                .Column(c => c.Age)
                .Query()
                .FirstOrDefault();

            Assert.AreEqual("Bob", saved.Name);
        }

        [Test]
        public void TestUpdate()
        {
            var person = new Person { Name = "Bob", Age = 50 };

            Connection
                .Insert(person)
                .Column(c => c.Name)
                .Column(c => c.Age)
                .ExecuteNonQuery();

            var saved = Connection
                .Select<Person>()
                .Column(c => c.Name)
                .Column(c => c.Age)
                .Query()
                .FirstOrDefault();

            saved.Name = "John";

            Connection
                .Update(saved)
                .Column(c => c.Name)
                .Where(p => p.Name == "Bob")
                .ExecuteNonQuery();

            var updated = Connection
                .Select<Person>()
                .Column(c => c.Name)
                .Column(c => c.Age)
                .Query()
                .FirstOrDefault();

            Assert.AreEqual("John", updated.Name);
        }

        private class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}
