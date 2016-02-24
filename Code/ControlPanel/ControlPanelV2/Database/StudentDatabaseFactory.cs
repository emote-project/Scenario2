using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ControlPanel.Database
{
    class StudentDatabaseFactory
    {
        public static IStudentsDatabase MakeDatabase()
        {
            return new ThalamusStudentDatabase();
        }
    }
}
