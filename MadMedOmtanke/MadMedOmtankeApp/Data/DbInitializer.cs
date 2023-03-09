using EmployeeLibary.Models;

namespace MadMedOmtankeApp.Data
{
    public class DbInitializer
    {
        public static void Initialze(MadMedOmtankeContext context)
        {
            if (context.Position.Any())
            {
                return;
            }

            var positionContent = new Position[]
            {
                new Position {PositionName="Manager"},
                new Position {PositionName="Cashier"},
                new Position {PositionName="Restocker"},
                new Position {PositionName="Service Worker"}
            };

            context.Position.AddRange(positionContent);
            context.SaveChanges();

            if (context.Department.Any())
            {
                return;
            }

            var departmentContent = new Department[]
            {
                new Department {Location="Odense"},
                new Department {Location="Copenhagen"},
                new Department {Location="Kolding"},
                new Department {Location="Esbjerg"}
            };

            context.Department.AddRange(departmentContent);
            context.SaveChanges();
        }
    }
}
