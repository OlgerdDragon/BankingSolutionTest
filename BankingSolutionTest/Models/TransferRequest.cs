namespace BankingSolutionTest.Models
{
    public class TransferRequest
    {
        public int FromAccountNumber { get; set; }

        public int ToAccountNumber { get; set; }

        public decimal Amount { get; set; }
    }
}
