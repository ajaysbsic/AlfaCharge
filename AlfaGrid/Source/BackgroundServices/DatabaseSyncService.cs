//namespace AlfaGrid.Source.BackgroundServices
//{
//    public class DatabaseSyncService
//    {
//        private readonly SyncServices syncServices;
//        public DatabaseSyncService()
//        {
//            syncServices = new SyncServices();
//        }

//        public async Task<bool> RunSyncToServerService()
//        {
//            try
//            {
//                //add all api calls here for syncing data with server
//                await syncServices.SyncServiceStatusData();
//                await syncServices.SyncStartServiceData();
//                await syncServices.SyncFinaliseServiceData();
//                await syncServices.SyncEwarrantyServiceData();
//                await syncServices.SyncCompleteServiceData();
//                App.SyncIsRunning = false;
//                return true;
//            }
//            catch (Exception)
//            {
//                App.SyncIsRunning = false;
//                return true;
//            }
//        }
//    }
//}