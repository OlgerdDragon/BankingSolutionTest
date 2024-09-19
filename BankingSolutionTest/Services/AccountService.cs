using BankingSolutionTest.Models;
using BankingSolutionTest.Services.Interface;

namespace BankingSolutionTest.Services
{
    public class AccountService : IAccountService
    {
        private static List<Account> accounts = new List<Account>();

        public Account CreateAccount(decimal initialBalance)
        {
            var newAccount = new Account
            {
                AccountNumber = accounts.Count + 1,
                Balance = initialBalance
            };
            accounts.Add(newAccount);
            return newAccount;
        }

        public Account GetAccountByNumber(int accountNumber)
        {
            return accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }

        public List<Account> GetAllAccounts()
        {
            return accounts;
        }

        public Account Deposit(int accountNumber, decimal amount)
        {
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account != null)
            {
                account.Balance += amount;
            }
            return account;
        }

        public Account Withdraw(int accountNumber, decimal amount)
        {
            var account = accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account != null && account.Balance >= amount)
            {
                account.Balance -= amount;
            }
            return account;
        }

        public bool Transfer(int fromAccountNumber, int toAccountNumber, decimal amount)
        {
            var fromAccount = accounts.FirstOrDefault(a => a.AccountNumber == fromAccountNumber);
            var toAccount = accounts.FirstOrDefault(a => a.AccountNumber == toAccountNumber);
            if (fromAccount != null && toAccount != null && fromAccount.Balance >= amount)
            {
                fromAccount.Balance -= amount;
                toAccount.Balance += amount;
                return true;
            }
            return false;
        }
    }
}
