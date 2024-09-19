using Microsoft.AspNetCore.Mvc;
using BankingSolutionTest.Models;
using BankingSolutionTest.Services.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingSolutionTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] Account newAccount)
        {
            var createdAccount = _accountService.CreateAccount(newAccount.Balance);
            return Ok(createdAccount);
        }

        [HttpGet("{accountNumber}")]
        public async Task<IActionResult> GetAccount(int accountNumber)
        {
            var account = _accountService.GetAccountByNumber(accountNumber);
            if (account == null)
            {
                return NotFound("Account not found.");
            }
            return Ok(account);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = _accountService.GetAllAccounts();
            return Ok(accounts);
        }

        [HttpPost("{accountNumber}/deposit")]
        public async Task<IActionResult> Deposit(int accountNumber, [FromBody] decimal amount)
        {
            var account = _accountService.Deposit(accountNumber, amount);
            if (account == null)
            {
                return NotFound("Account not found.");
            }
            return Ok(account);
        }

        [HttpPost("{accountNumber}/withdraw")]
        public async Task<IActionResult> Withdraw(int accountNumber, [FromBody] decimal amount)
        {
            var account = _accountService.Withdraw(accountNumber, amount);
            if (account == null)
            {
                return NotFound("Account not found.");
            }
            if (account.Balance < amount)
            {
                return BadRequest("Insufficient funds.");
            }
            return Ok(account);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest transfer)
        {
            var success = _accountService.Transfer(transfer.FromAccountNumber, transfer.ToAccountNumber, transfer.Amount);
            if (!success)
            {
                return BadRequest("Transfer failed. Check account details and balance.");
            }

            return Ok("Transfer successful.");
        }
    }
}
