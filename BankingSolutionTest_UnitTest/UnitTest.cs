using BankingSolutionTest.Controllers;
using BankingSolutionTest.Models;
using BankingSolutionTest.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BankingSolutionTest_UnitTest
{
    public class AccountsControllerTests
    {
        private readonly AccountsController _controller;
        private readonly Mock<IAccountService> _mockService;

        public AccountsControllerTests()
        {
            _mockService = new Mock<IAccountService>();
            _controller = new AccountsController(_mockService.Object);
        }

        // Тест для створення акаунта
        // Test for creating an account
        [Fact]
        public async Task CreateAccount_ReturnsCreatedAccount()
        {
            var newAccount = new Account { Balance = 100 };
            _mockService.Setup(s => s.CreateAccount(It.IsAny<decimal>())).Returns(newAccount);

            var result = await _controller.CreateAccount(newAccount) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(newAccount, result.Value);
        }

        // Тест для отримання акаунта за номером
        // Test for getting an account by number
        [Fact]
        public async Task GetAccount_ReturnsAccount_WhenExists()
        {
            var account = new Account { AccountNumber = 1, Balance = 100 };
            _mockService.Setup(s => s.GetAccountByNumber(1)).Returns(account);

            var result = await _controller.GetAccount(1) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(account, result.Value);
        }

        // Тест для отримання акаунта за номером, коли не існує
        // Test for getting an account by number when it does not exist
        [Fact]
        public async Task GetAccount_ReturnsNotFound_WhenDoesNotExist()
        {
            _mockService.Setup(s => s.GetAccountByNumber(1)).Returns((Account)null);

            var result = await _controller.GetAccount(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // Тест для отримання всіх акаунтів
        // Test for getting all accounts
        [Fact]
        public async Task GetAllAccounts_ReturnsListOfAccounts()
        {
            var accounts = new List<Account>
            {
                new Account { AccountNumber = 1, Balance = 100 },
                new Account { AccountNumber = 2, Balance = 200 }
            };
            _mockService.Setup(s => s.GetAllAccounts()).Returns(accounts);

            var result = await _controller.GetAllAccounts() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(accounts, result.Value);
        }

        // Тест для поповнення балансу акаунта
        // Test for depositing into an account
        [Fact]
        public async Task Deposit_ReturnsUpdatedAccount_WhenSuccessful()
        {
            var account = new Account { AccountNumber = 1, Balance = 100 };
            _mockService.Setup(s => s.Deposit(1, 50)).Returns(account);

            var result = await _controller.Deposit(1, 50) as OkObjectResult;

            Assert.NotNull(result);
            var resultAccount = result.Value as Account;
            Assert.NotNull(resultAccount);
            Assert.Equal(100, resultAccount.Balance);
        }

        // Тест для зняття грошей з акаунта
        // Test for withdrawing from an account
        [Fact]
        public async Task Withdraw_ReturnsUpdatedAccount_WhenSuccessful()
        {
            var account = new Account { AccountNumber = 1, Balance = 100 };
            _mockService.Setup(s => s.Withdraw(1, 50)).Returns(account);

            var result = await _controller.Withdraw(1, 50) as OkObjectResult;
            Assert.NotNull(result);
            var resultAccount = result.Value as Account;
            Assert.NotNull(resultAccount);
            Assert.Equal(100, resultAccount.Balance);
        }

        // Тест для зняття грошей з акаунта, коли недостатньо коштів
        // Test for withdrawing from an account when there are insufficient funds
        [Fact]
        public async Task Withdraw_ReturnsBadRequest_WhenInsufficientFunds()
        {
            var account = new Account { AccountNumber = 1, Balance = 50 };
            _mockService.Setup(s => s.Withdraw(1, 100)).Returns(account);

            var result = await _controller.Withdraw(1, 100);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // Тест для переказу між акаунтами
        // Test for transferring between accounts
        [Fact]
        public async Task Transfer_ReturnsOk_WhenSuccessful()
        {
            var transferRequest = new TransferRequest { FromAccountNumber = 1, ToAccountNumber = 2, Amount = 100 };
            _mockService.Setup(s => s.Transfer(1, 2, 100)).Returns(true);

            var result = await _controller.Transfer(transferRequest);

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Transfer successful.", ((OkObjectResult)result).Value);
        }

        // Тест для переказу, коли не вдається виконати
        // Test for transferring when it fails
        [Fact]
        public async Task Transfer_ReturnsBadRequest_WhenTransferFails()
        {
            var transferRequest = new TransferRequest { FromAccountNumber = 1, ToAccountNumber = 2, Amount = 100 };
            _mockService.Setup(s => s.Transfer(1, 2, 100)).Returns(false);

            var result = await _controller.Transfer(transferRequest);

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task FullDataAnalyticsTest()
        {
            // 1. Створити новий рахунок
            var newAccount = new Account { Balance = 100 };
            _mockService.Setup(s => s.CreateAccount(It.IsAny<decimal>())).Returns(newAccount);
            var createResult = await _controller.CreateAccount(newAccount) as OkObjectResult;
            Assert.NotNull(createResult);
            Assert.Equal(newAccount, createResult.Value);

            // 2. Отримати деталі рахунку
            _mockService.Setup(s => s.GetAccountByNumber(1)).Returns(newAccount);
            var getAccountResult = await _controller.GetAccount(1) as OkObjectResult;
            Assert.NotNull(getAccountResult);
            Assert.Equal(newAccount, getAccountResult.Value);

            // 3. Створення списка аккаунтів
            var accountsList = new List<Account> { newAccount };
            _mockService.Setup(s => s.GetAllAccounts()).Returns(accountsList);
            var getAllAccountsResult = await _controller.GetAllAccounts() as OkObjectResult;
            Assert.NotNull(getAllAccountsResult);
            Assert.Equal(accountsList, getAllAccountsResult.Value);

            // 4. Внести кошти
            var updatedAccountAfterDeposit = new Account { AccountNumber = 1, Balance = 200 };
            _mockService.Setup(s => s.Deposit(1, 100)).Returns(updatedAccountAfterDeposit);
            var depositResult = await _controller.Deposit(1, 100) as OkObjectResult;
            Assert.NotNull(depositResult);
            var resultAccountAfterDeposit = depositResult.Value as Account;
            Assert.NotNull(resultAccountAfterDeposit);
            Assert.Equal(200, resultAccountAfterDeposit.Balance);

            // 5. Переглянути рахунок
            var getUpdatedAccountResult = await _controller.GetAccount(1) as OkObjectResult;
            Assert.NotNull(getUpdatedAccountResult);
            Assert.Equal(updatedAccountAfterDeposit, getUpdatedAccountResult.Value);

            // 6. Зняти кошти
            var updatedAccountAfterWithdraw = new Account { AccountNumber = 1, Balance = 150 };
            _mockService.Setup(s => s.Withdraw(1, 50)).Returns(updatedAccountAfterWithdraw);
            var withdrawResult = await _controller.Withdraw(1, 50) as OkObjectResult;
            Assert.NotNull(withdrawResult);
            var resultAccountAfterWithdraw = withdrawResult.Value as Account;
            Assert.NotNull(resultAccountAfterWithdraw);
            Assert.Equal(150, resultAccountAfterWithdraw.Balance);

            // 7. Переглянути рахунок
            var getAccountAfterWithdrawResult = await _controller.GetAccount(1) as OkObjectResult;
            Assert.NotNull(getAccountAfterWithdrawResult);
            Assert.Equal(updatedAccountAfterWithdraw, getAccountAfterWithdrawResult.Value);

            // 8. Переказати кошти між двома рахунками
            var secondAccount = new Account { AccountNumber = 2, Balance = 100 };
            _mockService.Setup(s => s.CreateAccount(It.IsAny<decimal>())).Returns(secondAccount);
            _mockService.Setup(s => s.Transfer(1, 2, 50)).Returns(true);
            var transferRequest = new TransferRequest { FromAccountNumber = 1, ToAccountNumber = 2, Amount = 50 };
            var transferResult = await _controller.Transfer(transferRequest) as OkObjectResult;
            Assert.NotNull(transferResult);
            Assert.Equal("Transfer successful.", transferResult.Value);

            // 9. Переглянути всі рахунки
            var allAccountsAfterTransfer = new List<Account> { updatedAccountAfterWithdraw, secondAccount };
            _mockService.Setup(s => s.GetAllAccounts()).Returns(allAccountsAfterTransfer);
            var finalAccountsResult = await _controller.GetAllAccounts() as OkObjectResult;
            Assert.NotNull(finalAccountsResult);
            Assert.Equal(allAccountsAfterTransfer, finalAccountsResult.Value);
        }
    }
}
