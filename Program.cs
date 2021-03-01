using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EfDateCompareBug
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EfDateCompareBug;Trusted_Connection=True;");
        }

        public DbSet<Person> People { get; set; }
    }

    public class Person
    {
        public int PersonId { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }

        public DateTime DateTime { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var db = new Context();
            db.Database.EnsureCreated();

            var dateTimeOffset = new DateTimeOffset(2021, 3, 1, 12, 43, 28, TimeSpan.FromHours(-8));

            var person = new Person 
            { 
                DateTime = dateTimeOffset.DateTime,
                DateTimeOffset = dateTimeOffset,
            };
            db.People.Add(person);
            db.SaveChanges();

            Console.WriteLine("         LHS/Db Column     RHS/Parameter");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("SqlQuery DateTime       == DateTime:       {0}", db.People.Any(p => p.DateTime == dateTimeOffset.DateTime));
            Console.WriteLine("SqlQuery DateTime       == DateTimeOffset: {0}", db.People.Any(p => p.DateTime == dateTimeOffset));
            Console.WriteLine("SqlQuery DateTimeOffset == DateTime:       {0}", db.People.Any(p => p.DateTimeOffset == dateTimeOffset.DateTime));
            Console.WriteLine("SqlQuery DateTimeOffset == DateTimeOffset: {0}", db.People.Any(p => p.DateTimeOffset == dateTimeOffset));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(".NET     DateTime       == DateTime:       {0}", person.DateTime == dateTimeOffset.DateTime);
            Console.WriteLine(".NET     DateTime       == DateTimeOffset: {0}", person.DateTime == dateTimeOffset);
            Console.WriteLine(".NET     DateTimeOffset == DateTime:       {0}", person.DateTimeOffset == dateTimeOffset.DateTime);
            Console.WriteLine(".NET     DateTimeOffset == DateTimeOffset: {0}", person.DateTimeOffset == dateTimeOffset);
        }
    }
}
