﻿using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features {
    public class WithdrawMoney {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService) {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount) {
            var from = this.accountRepository.GetAccountById(fromAccountId);
            from.WithdrawMoney(amount);

            this.accountRepository.Update(from);

            if (from.Balance < 500m) {
                this.notificationService.NotifyFundsLow(from.User.Email);
            }
        }
    }
}
