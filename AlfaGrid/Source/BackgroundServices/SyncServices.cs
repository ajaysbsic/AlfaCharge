//using System;
//using System.Collections.ObjectModel;
//using Framework.Data.Cache.SqliteDatabase;

//namespace AlfaGrid.Source.BackgroundServices
//{
//    public class SyncServices
//    {
//        private readonly ISqliteBaseDao<AssignserviceRequestEntity> assignserviceRequestEntityRepo;
//        private readonly ISqliteBaseDao<StartServiceEntity> startServiceEntityRepo;
//        private readonly ISqliteBaseDao<FinalizeServiceEntity> finalizeServiceEntity;
//        private readonly ISqliteBaseDao<EwarrantyRequestModel> ewarrantyRequestModelRepo;
//        private readonly ISqliteBaseDao<CompleteServiceModel> completeServiceModelRepo;

//        public SyncServices()
//        {
//            assignserviceRequestEntityRepo = new SqliteBaseDao<AssignserviceRequestEntity>();
//            startServiceEntityRepo = new SqliteBaseDao<StartServiceEntity>();
//            finalizeServiceEntity = new SqliteBaseDao<FinalizeServiceEntity>();
//            ewarrantyRequestModelRepo = new SqliteBaseDao<EwarrantyRequestModel>();
//            completeServiceModelRepo = new SqliteBaseDao<CompleteServiceModel>();
//        }

//        public async Task SyncServiceStatusData()
//        {
//            await assignserviceRequestEntityRepo.InitializeAsync();
//            var dbOutput = await assignserviceRequestEntityRepo.GetAsync();
//            var dbResult = new ObservableCollection<AssignserviceRequestEntity>(dbOutput);
//            foreach (var item in dbResult)
//            {
//                item.IsOffline = true;
//                var response = await new AssignedServicesUseCases().UpdateServiceStatus(item);
//                if (response.IsSuccess)
//                {
//                    await assignserviceRequestEntityRepo.RemoveAsync(item);
//                }
//            }
//        }

//        public async Task SyncStartServiceData()
//        {
//            await startServiceEntityRepo.InitializeAsync();
//            var dbOutput = await startServiceEntityRepo.GetAsync();
//            var dbResult = new ObservableCollection<StartServiceEntity>(dbOutput);
//            foreach (var item in dbResult)
//            {
//                item.IsOffline = true;
//                var response = await new StartServiceUseCase().StartService(item);
//                if (response.IsSuccess)
//                {
//                    await startServiceEntityRepo.RemoveAsync(item);
//                }
//            }
//        }

//        public async Task SyncFinaliseServiceData()
//        {
//            await finalizeServiceEntity.InitializeAsync();
//            var dbOutput = await finalizeServiceEntity.GetAsync();
//            var dbResult = new ObservableCollection<FinalizeServiceEntity>(dbOutput);
//            foreach (var item in dbResult)
//            {
//                item.IsOffline = true;
//                var response = await new FinalizeServiceUseCase().FinalizeService(item);
//                if (response.IsSuccess)
//                {
//                    await finalizeServiceEntity.RemoveAsync(item);
//                }
//            }
//        }

//        public async Task SyncEwarrantyServiceData()
//        {
//            await ewarrantyRequestModelRepo.InitializeAsync();
//            var dbOutput = await ewarrantyRequestModelRepo.GetAsync();
//            var dbResult = new ObservableCollection<EwarrantyRequestModel>(dbOutput);
//            foreach (var item in dbResult)
//            {
//                item.IsOffline = true;
//                await new VerifyWarrantyUseCase().GenerateEWarrantyPolicy(item);
//                await ewarrantyRequestModelRepo.RemoveAsync(item);
//                //var ewarrantyPolicy = await new VerifyWarrantyUseCase().GenerateEWarrantyPolicy(item);
//                //if (ewarrantyPolicy.IsSuccess)
//                //{
//                //    await ewarrantyRequestModelRepo.RemoveAsync(item);
//                //}
//            }
//        }

//        public async Task SyncCompleteServiceData()
//        {
//            await completeServiceModelRepo.InitializeAsync();
//            var dbOutput = await completeServiceModelRepo.GetAsync();
//            var dbResult = new ObservableCollection<CompleteServiceModel>(dbOutput);
//            foreach (var item in dbResult)
//            {
//                item.IsOffline = true;
//                await new CompleteServiceUseCases().CompleteService(item);
//                await completeServiceModelRepo.RemoveAsync(item);
//                //var response = await new CompleteServiceUseCases().CompleteService(item);
//                //if (response.IsSuccess)
//                //{
//                //    await completeServiceModelRepo.RemoveAsync(item);
//                //}
//            }
//        }
//    }
//}