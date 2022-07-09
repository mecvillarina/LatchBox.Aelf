//using Client.App.Infrastructure.Managers;
//using Client.App.Services;
//using System;
//using System.Timers;

//namespace Client.App.PeriodicExecutors
//{
//    public class FetchDataExecutor : IDisposable
//    {
//        private readonly IAccountManager _accountManager;
//        private readonly IGeneralManager _generalManager;
//        private readonly IWalletManager _walletManager;
//        private readonly IExceptionHandler _exceptionHandler;

//        private Timer _timer;
//        private bool _running;
//        bool _isFetching;

//        public FetchDataExecutor(IExceptionHandler exceptionHandler, IAccountManager accountManager, IWalletManager walletManager, IGeneralManager generalManager)
//        {
//            _exceptionHandler = exceptionHandler;
//            _accountManager = accountManager;
//            _walletManager = walletManager;
//            _generalManager = generalManager;
//        }

//        public void StartExecuting()
//        {
//            if (!_running)
//            {
//                _timer = new Timer();
//                _timer.Interval = 10000;
//                _timer.Elapsed += HandleTimer;
//                _timer.AutoReset = true;
//                _timer.Enabled = true;
//                _running = true;
//            }
//        }

//        async void HandleTimer(object source, ElapsedEventArgs e)
//        {
//            try
//            {
//                if (_isFetching)
//                {
//                    return;
//                }

//                _isFetching = true;

////#if RELEASE
//                var profile = await _exceptionHandler.HandlerRequestTaskAsync(() => _accountManager.GetProfileAsync());
//                if (profile != null)
//                {
//                    await _exceptionHandler.HandlerRequestTaskAsync(() => _walletManager.GetWalletInfoAsync());
//                }
////#endif
//            }
//            catch
//            {
//                Console.WriteLine($"Fetch Wallet Executor: Fetch Error");
//            }
//            finally
//            {
//                _isFetching = false;
//            }
//        }

//        public void Dispose()
//        {
//            _timer?.Stop();
//            _timer?.Dispose();
//        }
//    }

//}
