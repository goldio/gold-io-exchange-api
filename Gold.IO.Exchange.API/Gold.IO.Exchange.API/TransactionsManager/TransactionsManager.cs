﻿using Gold.IO.Exchange.API.BlockExplorer.Blockchain;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Info.Blockchain.API.Models;
using Gold.IO.Exchange.API.BlockExplorer.Blockcypher;

namespace Gold.IO.Exchange.API.TransactionsManager
{
    public class TransactionsManager
    {
        private IUserWalletService UserWalletService { get; set; }
        private IUserWalletOperationService UserWalletOperationService { get; set; }
        private ICoinAddressService CoinAddressService { get; set; }

        public TransactionsManager()
        {
            var myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(CheckOperations);
            myTimer.Interval = 63250 * 3;
            myTimer.Start();
        }

        public void SetServices(
            IUserWalletService userWalletService, 
            IUserWalletOperationService userWalletOperationService,
            ICoinAddressService coinAddressService)
        {
            if (UserWalletService == null)
                UserWalletService = userWalletService;

            if (UserWalletOperationService == null)
                UserWalletOperationService = userWalletOperationService;

            if (CoinAddressService == null)
                CoinAddressService = coinAddressService;
        }

        public void CheckOperations(object source, ElapsedEventArgs e)
        {
            var operations = UserWalletOperationService.GetAll().ToList();

            CheckBitcoinOperations(operations.Where(x => x.Address.Wallet.Coin.ShortName == "BTC" && x.Status == UserWalletOperationStatus.InProgress).ToList());
            CheckEthereumOperations(operations.Where(x => x.Address.Wallet.Coin.ShortName == "ETH" && x.Status == UserWalletOperationStatus.InProgress).ToList());
            CheckEosOperations(operations.Where(x => x.Address.Wallet.Coin.ShortName == "EOS" && x.Status == UserWalletOperationStatus.InProgress).ToList());
        }

        public long GetTransactionConfirmations(Transaction tx)
        {
            if (tx.BlockHeight == -1)
                return 0;

            using (var client = new BlockchainClient())
            {
                var blocksCountTask = client.GetBlockCount();
                blocksCountTask.Wait();

                return blocksCountTask.Result - tx.BlockHeight;
            }
        }

        public long GetTransactionConfirmations(string txHash)
        {
            using (var client = new BlockchainClient())
            {
                var txTask = client.GetTransaction(txHash);
                txTask.Wait();

                if (txTask.Result.BlockHeight == -1)
                    return 0;

                var blocksCountTask = client.GetBlockCount();
                blocksCountTask.Wait();

                return blocksCountTask.Result - txTask.Result.BlockHeight;
            }
        }

        //private void UpdateLocalWallet(Address address)
        //{
        //    var wallet = UserWalletService.GetAll().FirstOrDefault(x => x..Address == address.Base58Check);
        //    if (wallet == null)
        //        return;

        //    var addressBalance = (double)address.FinalBalance.GetBtc();
        //    if (wallet.Balance < addressBalance)
        //    {
                
        //    }
        //}

        private void CheckBitcoinOperations(List<UserWalletOperation> operations)
        {
            if (operations.FirstOrDefault(x => !x.Address.Wallet.Coin.ShortName.Equals("BTC")) != null)
                return;

            using (var client = new BlockchainClient())
            {
                var blockHeightTask = client.GetBlockCount();
                blockHeightTask.Wait();

                foreach (var o in operations)
                {
                    var addressTask = client.GetAddress(o.Address.PublicAddress);
                    addressTask.Wait();

                    var address = addressTask.Result;
                    var addressBalance = (double)address.FinalBalance.GetBtc();

                    if (o.Address.Wallet.Balance != addressBalance)
                    {
                        var tx = address.Transactions.FirstOrDefault();
                        var txConfs = GetTransactionConfirmations(tx);

                        if (txConfs < 2)
                        {
                            o.Address.IsUsing = false;
                            CoinAddressService.Update(o.Address);

                            o.Amount = (double)address.FinalBalance.GetBtc();
                            o.Confirmations = txConfs;
                            o.Time = tx.Time;
                            o.Status = UserWalletOperationStatus.InProgress;
                            UserWalletOperationService.Update(o);
                        }
                        else if (txConfs > 1)
                        {
                            o.Status = UserWalletOperationStatus.Completed;
                            UserWalletOperationService.Update(o);

                            o.Address.Wallet.Balance += o.Amount;
                            UserWalletService.Update(o.Address.Wallet);
                        }
                    }
                }
            }
        }

        private void CheckEthereumOperations(List<UserWalletOperation> operations)
        {
            if (operations.FirstOrDefault(x => !x.Address.Wallet.Coin.ShortName.Equals("BTC")) != null)
                return;

            using (var client = new BlockcypherClient())
            {
                foreach (var o in operations)
                {
                    var addressTask = client.CheckAddress(o.Address.PublicAddress);
                    addressTask.Wait();

                    var address = addressTask.Result;
                    if (address == null || address.Txrefs == null || address.Txrefs.Length == 0)
                        return;

                    var addressBalance = ConvertToEth(address.Balance);

                    if (o.Address.Wallet.Balance != addressBalance)
                    {
                        var tx = address.Txrefs.FirstOrDefault();

                        o.Address.Wallet.Balance = addressBalance;
                        UserWalletService.Update(o.Address.Wallet);

                        o.Address.IsUsing = false;
                        CoinAddressService.Update(o.Address);

                        o.Amount = addressBalance;
                        o.Confirmations = tx.Confirmations;
                        o.Time = DateTime.Parse(tx.TimeStamp);
                        o.Status = UserWalletOperationStatus.Completed;
                        UserWalletOperationService.Update(o);

                        o.Address.Wallet.Balance += o.Amount;
                        UserWalletService.Update(o.Address.Wallet);
                    }
                }
            }
        }

        private void CheckEosOperations(List<UserWalletOperation> operations)
        {

        }

        private double ConvertToWei(double amountInEth)
        {
            return amountInEth * 1000000000000000000;
        } 

        private double ConvertToEth(double amountInWei)
        {
            return amountInWei / 1000000000000000000;
        }

        private double ConvertToEth(long amountInWei)
        {
            return amountInWei / 1000000000000000000;
        }
    }
}