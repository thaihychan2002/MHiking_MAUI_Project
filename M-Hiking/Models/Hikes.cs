using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Hiking.Models
{
    public class Hikes
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public bool HasParking { get; set; }
        public double Length { get; set; }
        public string Difficulty { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Hikes Clone()=> MemberwiseClone() as Hikes;
        public (bool IsValid, string? ErrorMessage) Validate()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Location) ||
                Length== 0||
                string.IsNullOrWhiteSpace(Difficulty) ||
                string.IsNullOrWhiteSpace(Description))
            {
               return(false,"Please fill in all fields before creating a hike.");
            }
            return (true, null);
        }
    }
}
