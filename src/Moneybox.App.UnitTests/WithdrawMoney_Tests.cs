using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App;
using System;
using Xunit;
using Moq;
using Moneybox.App.Features;

namespace UnitTests {
    public class WithdrawMoney_Tests {
        [Theory]
        [InlineData(100.50, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 100, 0, "dartvader@gmail.com")]
        [InlineData(400, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 0, 0, "dartvader@gmail.com")]
        public void WithdrawMoney_Account_Balance_Update_Tests(decimal amount, string fromAccountIdS, int fromBalance, int fromWithdrawn, int fromPaidIn, string fromEmail) {
            var fromAccountId = Guid.Parse(fromAccountIdS);

            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = fromAccountId,
                PaidIn = fromPaidIn,
                Withdrawn = fromWithdrawn,
                User = new User() {
                    Email = fromEmail
                }
            };

            var accountMock = GetMockIAccountRepository(fromAccount);
            var notificationMock = GetMockINotificationService(fromEmail);
            WithdrawMoney withdrawMoney = new WithdrawMoney(accountMock.Object, notificationMock.Object);
            withdrawMoney.Execute(fromAccountId, amount);
            accountMock.Verify(n => n.GetAccountById(fromAccountId), Times.Once);
            accountMock.Verify(n => n.Update(fromAccount), Times.Once);

            Assert.Equal(fromAccount.Balance, fromBalance - amount);
            Assert.Equal(fromAccount.Withdrawn, fromWithdrawn - amount);
        }

        [Theory]
        [InlineData(600, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 100, 0, "dartvader@gmail.com")]
        [InlineData(10, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 0, 0, 0, "dartvader@gmail.com")]
        public void WithdrawMoney_Account_Balance_Unsuccessfull_Tests(decimal amount, string fromAccountIdS, int fromBalance, int fromWithdrawn, int fromPaidIn, string fromEmail) {
            var fromAccountId = Guid.Parse(fromAccountIdS);

            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = fromAccountId,
                PaidIn = fromPaidIn,
                Withdrawn = fromWithdrawn,
                User = new User() {
                    Email = fromEmail
                }
            };

            var accountMock = GetMockIAccountRepository(fromAccount);
            var notificationMock = GetMockINotificationService(fromEmail);
            WithdrawMoney withdrawMoney = new WithdrawMoney(accountMock.Object, notificationMock.Object);
            Assert.Throws<InvalidOperationException>(() => withdrawMoney.Execute(fromAccountId, amount));
            accountMock.Verify(n => n.GetAccountById(fromAccountId), Times.Once);
            accountMock.Verify(n => n.Update(fromAccount), Times.Never);
        }

        [Theory]
        [InlineData(100.50, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 0, "dartvader@gmail.com")]
        [InlineData(400, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 500, 0, "dartvader@gmail.com")]
        public void TransferMoney_Notification_Called_Tests(decimal amount, string fromAccountIdS, int fromBalance, int fromPaidIn, string fromEmail) {
            var fromAccountId = Guid.Parse(fromAccountIdS);


            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = fromAccountId,
                PaidIn = fromPaidIn,
                User = new User() {
                    Email = fromEmail
                }
            };

            var accountMock = GetMockIAccountRepository(fromAccount);
            var notificationMock = GetMockINotificationService(fromEmail);
            WithdrawMoney withdrawMoney = new WithdrawMoney(accountMock.Object, notificationMock.Object);
            withdrawMoney.Execute(fromAccountId, amount);
            notificationMock.Verify(n => n.NotifyFundsLow(fromEmail), Times.Once);
        }

        [Theory]
        [InlineData(100.50, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 2000, 0, "dartvader@gmail.com")]
        [InlineData(300, "dde39d5d-86ff-4e90-a6fe-a42c8a557fe0", 5000, 0, "dartvader@gmail.com")]
        public void TransferMoney_Notification_NonCalled_Tests(decimal amount, string fromAccountIdS, int fromBalance, int fromPaidIn, string fromEmail) {
            var fromAccountId = Guid.Parse(fromAccountIdS);

            Account fromAccount = new Account() {
                Balance = fromBalance,
                Id = fromAccountId,
                PaidIn = fromPaidIn,
                User = new User() {
                    Email = fromEmail
                }
            };

            var accountMock = GetMockIAccountRepository(fromAccount);
            var notificationMock = GetMockINotificationService(fromEmail);
            WithdrawMoney withdrawMoney = new WithdrawMoney(accountMock.Object, notificationMock.Object);
            withdrawMoney.Execute(fromAccountId, amount);
            notificationMock.Verify(n => n.NotifyFundsLow(fromEmail), Times.Never);
        }

        Mock<IAccountRepository> GetMockIAccountRepository(Account fromAccount) {
            var accountMock = new Mock<IAccountRepository>();
            accountMock.Setup(m => m.GetAccountById(fromAccount.Id)).Returns(fromAccount);
            accountMock.Setup(m => m.Update(fromAccount)).Verifiable();
            return accountMock;
        }

        Mock<INotificationService> GetMockINotificationService(string fromEmail) {
            var notficationMock = new Mock<INotificationService>();
            notficationMock.Setup(m => m.NotifyFundsLow(fromEmail)).Verifiable();
            return notficationMock;
        }
    }
}
