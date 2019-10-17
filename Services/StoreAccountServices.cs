using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Dhipaya.DAL;
using Dhipaya.Models;
using Microsoft.AspNetCore.Http;

namespace Dhipaya.Services
{
    public class StoreAccountServices
    {
        private ChFrontContext _context;

        public int? StoreAccountID { get; set; }

        //public StoreAccount sessionStoreAccount
        //{
        //    get
        //    {
        //        if (this.StoreAccountID == null)
        //        {
        //            var query = this._context.StoreAccounts
        //                .Include(s => s.StoreTransactions)
        //                .Where(s => s.IsOpen == true)
        //                .FirstOrDefault();
        //            return query;
        //        }
        //        else
        //        {
        //            var query = this._context.StoreAccounts
        //                .Include(s => s.StoreTransactions)
        //                .Where(s => s.ID == this.StoreAccountID.Value)
        //                .FirstOrDefault();
        //            return query;
        //        }
        //    }
        //}

        //public StoreAccount StoreAccount(int ID)
        //{

        //    var query = this._context.StoreAccounts
        //        .Include(s => s.StoreTransactions)
        //        .Where(s => s.ID == ID)
        //        .FirstOrDefault();
        //    return query;
        //}


        public StoreAccountServices(ChFrontContext context)
        {
            this._context = context;
        }

        //public decimal CurrentBalance()
        //{
        //    var query = this._context.StoreTransactions
        //        .Where(s => s.StoreAccountID == this.sessionStoreAccount.ID)
        //        .ToList();
        //    decimal bal = 0;
        //    if (query != null)
        //    {
        //        bal = query.Sum(s => s.TransactionAmount);
        //    }
        //    return bal;
        //}

        //public decimal TotalAmountByType(TransactionType transactionType)
        //{
        //    var query = this.sessionStoreAccount.StoreTransactions
        //        .Where(s => s.TransactionType == transactionType)
        //        .ToList();
        //    decimal bal = 0;
        //    if (query != null)
        //    {
        //        bal = query.Sum(s => s.TransactionAmount);
        //    }
        //    return bal;
        //}

        //public int TransactionCount()
        //{
        //    int count = 0;
        //    count = this.sessionStoreAccount.StoreTransactions.Count();
        //    return count;
        //}

        //public IEnumerable<dynamic> GetSummary()
        //{
        //    var query = this._context.StoreTransactions
        //        .Include(s => s.BankAccount)
        //            .ThenInclude(b => b.Bank)
        //        .Where(s => s.StoreAccountID == this.sessionStoreAccount.ID)
        //        .OrderBy(s => s.BankAccount.BankID)
        //        .GroupBy(s => new { s.BankAccountID });
        //    var summaryList = new List<dynamic>();
        //    foreach (var item in query)
        //    {
        //        dynamic summary = new ExpandoObject();
        //        summary.AccountNumber = item.FirstOrDefault().BankAccount.AccountNumber;
        //        summary.AccountName = item.FirstOrDefault().BankAccount.AccountName;
        //        summary.BankName = item.FirstOrDefault().BankAccount.Bank.BankName;
        //        summary.Credit = item.Sum(c => c.TransactionAmount);
        //        summaryList.Add(summary);
        //    }
        //    return summaryList;
        //}
    }
}
