using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class Sellers
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Insira um email válido")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Email { get; set; }

        [Display(Name = "Salário")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public double Salary { get; set; }

        [Display(Name = "Vendas")]
        public ICollection<Sales> Sales { get; set; } = new List<Sales>();

        public Sellers() { }

        public Sellers(int id, string name, string email, double salary)
        {
            Id = id;
            Name = name;
            Email = email;
            Salary = salary;
        }

        public double CalculateTotalSalesAmount()
        {
            return Sales.Sum(sale => sale.Amount);
        }

    }
}
