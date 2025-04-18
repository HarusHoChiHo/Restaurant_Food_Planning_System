using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Menu
{
    public int      Id   { get; set; }
    
    public DateTime Date { get; set; }
    
    public int      MenuItem_Id   { get; set; }
    public MenuItem MenuItem { get; set; }
}