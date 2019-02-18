using Gold.IO.Exchange.API.BlockExplorer.Blockchain;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Info.Blockchain.API.Models;

namespace Gold.IO.Exchange.API.TransactionsManager
{
    public class TransactionsManager
    {
        private IUserWalletService UserWalletService { get; set; }
        private IUserWalletOperationService UserWalletOperationService { get; set; }

        public TransactionsManager()
        {
            var myTimer = new Timer();
            myTimer.Elapsed += new ElapsedEventHandler(CheckOperations);
            myTimer.Interval = 60000 * 10;
            myTimer.Start();
        }

        public void SetServices(IUserWalletService userWalletService, IUserWalletOperationService userWalletOperationService)
        {
            if (UserWalletService == null)
                UserWalletService = userWalletService;

            if (UserWalletOperationService == null)
                UserWalletOperationService = userWalletOperationService;
        }

        public void CheckOperations(object source, ElapsedEventArgs e)
        {
            var wallets = UserWalletService.GetAll().ToList();

            CheckBitcoinOperations(wallets.Where(x => x.Coin.ShortName == "BTC").ToList());
            CheckEthereumOperations(wallets.Where(x => x.Coin.ShortName == "ETH").ToList());
            CheckEosOperations(wallets.Where(x => x.Coin.ShortName == "EOS").ToList());
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

        private void UpdateLocalWallet(Address address)
        {
            var wallet = UserWalletService.GetAll().FirstOrDefault(x => x.Address.Address == address.Base58Check);
            if (wallet == null)
                return;

            var addressBalance = (double)address.FinalBalance.GetBtc();
            if (wallet.Balance < addressBalance)
            {
                
            }
        }

        private void CheckBitcoinOperations(List<UserWallet> wallets)
        {
            using(var client = new BlockchainClient())
            {
                var blockHeightTask = client.GetBlockCount();
                blockHeightTask.Wait();

                foreach (var w in wallets)
                {
                    var addressTask = client.GetAddress(w.Address.Address);
                    addressTask.Wait();

                    var address = addressTask.Result;
                    var addressBalance = (double)address.FinalBalance.GetBtc();

                    if (w.Balance != addressBalance)
                    {
                        w.Balance = (double)address.FinalBalance.GetBtc();

                        var operations = UserWalletOperationService.GetAll().Where(x => x.Wallet == w && x.Type == UserWalletOperationType.Deposit);
                        foreach (var o in operations)
                            UserWalletOperationService.Delete(o);

                        foreach (var tx in address.Transactions)
                        {
                            var operation = new UserWalletOperation
                            {
                                Amount = (double)tx.Outputs.FirstOrDefault(x => x.Address == w.Address.Address).Value.GetBtc(),
                                Confirmations = blockHeightTask.Result - tx.BlockHeight,
                                Time = tx.Time,
                                Type = UserWalletOperationType.Deposit,
                                Status = UserWalletOperationStatus.Completed,
                                Wallet = w
                            };

                            UserWalletOperationService.Create(operation);
                        }
                    }
                }
            }
        }

        private void CheckEthereumOperations(List<UserWallet> wallets)
        {

        }

        private void CheckEosOperations(List<UserWallet> wallets)
        {

        }
    }
}
