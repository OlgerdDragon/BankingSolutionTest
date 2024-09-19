using BankingSolutionTest.Controllers;
using BankingSolutionTest.Models;
using BankingSolutionTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingSolutionTest_IntegrationTest
{
    public class AccountsIntegrationTests
    {
        private readonly AccountsController _controller;
        private readonly AccountService _accountService;

        public AccountsIntegrationTests()
        {
            _accountService = new AccountService();
            _controller = new AccountsController(_accountService);
        }

        [Fact]
        public async Task FullDataAnalytics_ReturnsCorrectAccountDetails()
        {
            // 1. Створити новий аккаунт
            // 1. Create a new account
            var newAccount = new Account { Balance = 100 };
            var createResponse = await _controller.CreateAccount(newAccount) as OkObjectResult;
            var createdAccount = createResponse.Value as Account;

            // 2. Отримати деталі аккаунта
            // 2. Get account details
            var getResponse = await _controller.GetAccount(createdAccount.AccountNumber) as OkObjectResult;
            var fetchedAccount = getResponse.Value as Account;
            Assert.Equal(createdAccount.AccountNumber, fetchedAccount.AccountNumber);
            Assert.Equal(100, fetchedAccount.Balance);

            // 3. Створити ще декілька аккаунтів
            // 3. Create additional accounts
            var secondAccount = new Account { Balance = 200 };
            var createSecondResponse = await _controller.CreateAccount(secondAccount) as OkObjectResult;
            var createdSecondAccount = createSecondResponse.Value as Account;

            var thirdAccount = new Account { Balance = 300 };
            var createThirdResponse = await _controller.CreateAccount(thirdAccount) as OkObjectResult;
            var createdThirdAccount = createThirdResponse.Value as Account;

            // 4. Переглянути всі аккаунти
            // 4. Get all accounts
            var accountsResponse = await _controller.GetAllAccounts() as OkObjectResult;
            var accountsList = accountsResponse.Value as List<Account>;
            Assert.Contains(accountsList, a => a.AccountNumber == createdAccount.AccountNumber);
            Assert.Contains(accountsList, a => a.AccountNumber == createdSecondAccount.AccountNumber);
            Assert.Contains(accountsList, a => a.AccountNumber == createdThirdAccount.AccountNumber);

            // 5. Внести кошти на аккаунт
            // 5. Deposit funds into the account
            var depositResponse = await _controller.Deposit(createdAccount.AccountNumber, 50) as OkObjectResult;
            var updatedAccountAfterDeposit = depositResponse.Value as Account;
            Assert.Equal(150, updatedAccountAfterDeposit.Balance);

            // 6. Переглянути аккаунт
            // 6. Get account details after deposit
            var accountAfterDepositResponse = await _controller.GetAccount(createdAccount.AccountNumber) as OkObjectResult;
            var fetchedAccountAfterDeposit = accountAfterDepositResponse.Value as Account;
            Assert.Equal(150, fetchedAccountAfterDeposit.Balance);

            // 7. Зняти кошти з аккаунта
            // 7. Withdraw funds from the account
            var withdrawResponse = await _controller.Withdraw(createdAccount.AccountNumber, 50) as OkObjectResult;
            var updatedAccountAfterWithdraw = withdrawResponse.Value as Account;
            Assert.Equal(100, updatedAccountAfterWithdraw.Balance);

            // 8. Переглянути аккаунт
            // 8. Get account details after withdrawal
            var accountAfterWithdrawResponse = await _controller.GetAccount(createdAccount.AccountNumber) as OkObjectResult;
            var fetchedAccountAfterWithdraw = accountAfterWithdrawResponse.Value as Account;
            Assert.Equal(100, fetchedAccountAfterWithdraw.Balance);

            // 9. Переказати кошти між двома рахунками
            // 9. Transfer funds between two accounts
            var transferRequest = new TransferRequest
            {
                FromAccountNumber = createdAccount.AccountNumber,
                ToAccountNumber = createdSecondAccount.AccountNumber,
                Amount = 50
            };
            var transferResponse = await _controller.Transfer(transferRequest) as OkObjectResult;
            Assert.Equal("Transfer successful.", transferResponse.Value);

            // 10. Переглянути всі рахунки
            // 10. Get all accounts after transfer
            accountsResponse = await _controller.GetAllAccounts() as OkObjectResult;
            accountsList = accountsResponse.Value as List<Account>;

            var finalFirstAccount = await _controller.GetAccount(createdAccount.AccountNumber) as OkObjectResult;
            var finalFetchedFirstAccount = finalFirstAccount.Value as Account;

            var finalSecondAccount = await _controller.GetAccount(createdSecondAccount.AccountNumber) as OkObjectResult;
            var finalFetchedSecondAccount = finalSecondAccount.Value as Account;

            Assert.Equal(50, finalFetchedFirstAccount.Balance); // 100 - 50
            Assert.Equal(250, finalFetchedSecondAccount.Balance); // 200 + 50
        }
    }
}
