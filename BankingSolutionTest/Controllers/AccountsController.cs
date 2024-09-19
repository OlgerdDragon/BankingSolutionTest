using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using BankingSolutionTest.Models;

namespace BankingSolutionTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private static List<Account> accounts = new List<Account>();

               [HttpPost]
        public IActionResult CreateAccount([FromBody] Account newAccount)
        {
            newAccount.AccountNumber = accounts.Count + 1;
            accounts.Add(newAccount);
            return Ok(newAccount);
        }

        [HttpGet("{accountNumber}")]
        public IActionResult GetAccount(int accountNumber)
        {
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                return NotFound("Account not found.");
            }
            return Ok(account);
        }

        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            return Ok(accounts);
        }

        [HttpPost("{accountNumber}/deposit")]
        public IActionResult Deposit(int accountNumber, [FromBody] decimal amount)
        {
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                return NotFound("Account not found.");
            }
            account.Balance += amount;
            return Ok(account);
        }

        [HttpPost("{accountNumber}/withdraw")]
        public IActionResult Withdraw(int accountNumber, [FromBody] decimal amount)
        {
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                return NotFound("Account not found.");
            }
            if (account.Balance < amount)
            {
                return BadRequest("Insufficient funds.");
            }
            account.Balance -= amount;
            return Ok(account);
        }

        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferRequest transfer)
        {
            var fromAccount = accounts.FirstOrDefault(a => a.AccountNumber == transfer.FromAccountNumber);
            var toAccount = accounts.FirstOrDefault(a => a.AccountNumber == transfer.ToAccountNumber);

            if (fromAccount == null || toAccount == null)
            {
                return NotFound("One or both accounts not found.");
            }

            if (fromAccount.Balance < transfer.Amount)
            {
                return BadRequest("Insufficient funds in the source account.");
            }

            fromAccount.Balance -= transfer.Amount;
            toAccount.Balance += transfer.Amount;

            return Ok(new { From = fromAccount, To = toAccount });
        }
    }
}
