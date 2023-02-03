namespace HtlDamage.Application.Model
{
    public class Student : User
    {
        public Student(string firstName, string lastName, string userName, string schoolClass) : base(firstName, lastName, userName)
        {
            SchoolClass = schoolClass;
        }

#pragma warning disable CS8618 
        protected Student() { }
#pragma warning restore CS8618

        public string SchoolClass { get; set; }
    }
}
