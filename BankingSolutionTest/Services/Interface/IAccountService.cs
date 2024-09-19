using BankingSolutionTest.Models;
using System.Collections.Generic;

namespace BankingSolutionTest.Services.Interface
{
    public interface IAccountService
    {
        Account CreateAccount(decimal initialBalance);

        Account GetAccountByNumber(int accountNumber);

        List<Account> GetAllAccounts();

        Account Deposit(int accountNumber, decimal amount);

        Account Withdraw(int accountNumber, decimal amount);

        bool Transfer(int fromAccountNumber, int toAccountNumber, decimal amount);
    }
}
