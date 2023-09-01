using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GeolocationApp.Model;
using GeolocationApp.Methods;
using GeolocationApp.Interface;
using SQLite;
using static GeolocationApp.Model.Enums;


namespace GeolocationApp.Service
{
    public class SqlLiteService : IDatabaseService
    {
        public readonly string DatabasePath = Path.Combine(GetPath.GetFullPath(), "SLite.db3");
        public ILoggingService LoggingService;
        public INotificationService NotificationService;
        public IHealthService HealthService;


        public SqlLiteService(INotificationService notificationService, ILoggingService loggingService, IHealthService healthService)
        {
            NotificationService = notificationService;
            LoggingService = loggingService;
            HealthService = healthService;
        }


        public async Task<bool> ValidateDatabase()
        {
            var result = false;
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = new SQLiteConnection(DatabasePath))
                    {
                        try
                        {
                            result = conn.ExecuteScalar<int>("SELECT 1") != 0;

                            if (result)
                            {
                                NotificationService.Success("Database access functions properly.");
                            }
                            else
                            {
                                NotificationService.Fail(
                                    "There was an issue accessing the database. Please contact support");
                            }
                        }
                        catch (Exception ex)
                        {
                            NotificationService.Fail(
                                "There was an issue accessing the database. Please contact support");
                            LoggingService.Log($"Exception: {ex.Message} \n");
                            HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                        }
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Fail("Could not connect to the database. Please contact support");
                    LoggingService.Log($"Exception: {ex.Message} \n");
                    HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);
                }
            });
            return result;
        }


        public async Task<bool> CheckTableExist<T>(T model)

        {
            var result = false;
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = new SQLiteConnection(DatabasePath))
                    {
                        try
                        {
                            result = ExecuteTableExist(model, conn);


                            if (result)
                                NotificationService.Success(
                                    $"The table '{model.GetType().Name}' exists within the database.");
                            else
                                NotificationService.Fail(
                                    $"The table '{model.GetType().Name}' was not found in the database. Please contact support");
                        }
                        catch (Exception ex)
                        {
                            NotificationService.Fail(
                                "There was an issue accessing the database. Please contact support");
                            LoggingService.Log($"Exception: {ex.Message} \n");
                            HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);
                        }
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Fail("Could not connect to the database. Please contact support");
                    LoggingService.Log($"Exception: {ex.Message} \n");
                    HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                }
            });

            return result;
        }

        public async Task<bool> CreateTable<T>(T model)
        {
            var data = GetQueryCreateTable(model);
            var result = false;
            var message = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = new SQLiteConnection(DatabasePath))
                    {
                        try
                        {
                            ExecuteCreateTable(data, conn);

                            result = ExecuteTableExist(model, conn);
                            ;
                            if (result)
                                NotificationService.Success(
                                    $"The table '{model.GetType().Name}' has been successfully created in the database.");
                            else
                                NotificationService.Fail(
                                    $"Failed to create the '{model.GetType().Name}' table in the database. Contact support.");
                        }


                        catch (Exception ex)
                        {
                            NotificationService.Fail(
                                "There was an issue accessing the database. Please contact support.\n " +
                                $"[Could not create {model.GetType().Name}]");
                            LoggingService.Log($"Exception: {ex.Message} \n");
                            HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                        }
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Fail("Could not connect to the database. Please contact support");
                    LoggingService.Log($"Exception: {ex.Message} \n");
                    HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);
                }
            });
            return result;
        }

        public async Task<IpAdressModel> GetIpAdress(IpDomainSearchModel searchModel)
        {
            LoggingService.Log($"{LogHelper.GetPropertyReader(searchModel)}", LogLevel.DetailedLogging);


            IpAdressModel geolocation = null;
            var message = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = new SQLiteConnection(DatabasePath))
                    {
                        try
                        {
                            geolocation = ExecuteGetIpAdress(searchModel, conn);
                        }

                        catch (Exception ex)
                        {
                            NotificationService.Fail(
                                "There was an issue accessing the database. Please contact support.\n " +
                                $"[Could not get the {searchModel.EntryDataType.ToString()} from database with '{searchModel.Id}']\n");
                            LoggingService.Log($"Exception: {ex.Message} \n");
                            HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                        }
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Fail("Could not connect to the database. Please contact support");
                    LoggingService.Log($"Exception: {ex.Message} \n");
                    HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                }
            });


            if (geolocation != null)
                NotificationService.Success(
                    $"Successfully retrieved from database {searchModel.EntryDataType.ToString()}:'{searchModel.Id}'.");
            else
                NotificationService.Notification(
                    $"{searchModel.EntryDataType.ToString()}:'{searchModel.Id}' is not in the database.");

            return geolocation ?? new IpAdressModel();
        }

        public async Task<List<DomainModel>> GetDomainList(IpDomainSearchModel searchModel)
        {
            LoggingService.Log($"{LogHelper.GetPropertyReader(searchModel)}", LogLevel.DetailedLogging);

            var domains = new List<DomainModel>();
            var message = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    using (var conn = new SQLiteConnection(DatabasePath))
                    {
                        try
                        {
                            domains = GetDomainNames(searchModel, conn);
                        }

                        catch (Exception ex)
                        {
                            NotificationService.Fail(
                                "There was an issue accessing the database. Please contact support.\n " +
                                $"[Could not get the {searchModel.EntryDataType.ToString()} from database with '{searchModel.Id}']\n");
                            LoggingService.Log($"Exception: {ex.Message} \n");
                            HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                        }
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Fail("Could not connect to the database. Please contact support");
                    LoggingService.Log($"Exception: {ex.Message} \n");
                    HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                }
            });

            if (domains.Count != 0)
            {
                NotificationService.Success(
                    $"Successfully retrieved list of domains from database {searchModel.EntryDataType.ToString()}:'{searchModel.Id}'.");
            }


            return domains;
        }

        public async Task<bool> Insert(IpAdressModel ipAdressModel, DomainModel domainModel)
        {
            LoggingService.Log(
                $"{LogHelper.GetPropertyReader(ipAdressModel)}\n{LogHelper.GetPropertyReader(domainModel)}",
                LogLevel.DetailedLogging);
            var result = false;

            if (CheckModelNotNull(ipAdressModel))
                await Task.Run(() =>
                {
                    try
                    {
                        using (var conn = new SQLiteConnection(DatabasePath))
                        {
                            try
                            {
                                if (CheckForConstraint(ipAdressModel, conn))
                                    result = ExecuteInsert(ipAdressModel, conn);
                                else
                                { 
                                    if (!CheckModelNotNull(domainModel))
                                    {
                                        NotificationService.Notification(
                                        $"The provided geolocation information with ip: '{ipAdressModel.Id}' already exists in the database and cannot be edited.");

                                    }
                                }

                                if (CheckModelNotNull(domainModel))
                                {
                                    if (CheckForConstraint(domainModel, conn))
                                    {
                                        result = ExecuteInsert(domainModel, conn);
                                    }
                                    else
                                        NotificationService.Notification(
                                            $"The provided searchModel with name: '{domainModel.Name}' already exists in the database and cannot be edited.");
                                }
                            }
                            catch (Exception ex)
                            {
                                NotificationService.Fail(
                                    "There was a issue while inserting data into the database. Please contact support.");
                                LoggingService.Log($"Exception: {ex.Message} \n");
                                HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        NotificationService.Fail("Could not connect to the database. Please contact support");
                        LoggingService.Log($"Exception: {ex.Message} \n");
                        HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                    }
                });

            return result;
        }

        public async Task<bool> Delete(IpDomainSearchModel searchModel)
        {
            var result = false;
            var message = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    using (var conn = new SQLiteConnection(DatabasePath))
                    {
                        try
                        {
                            ExecuteDeleteById(searchModel, conn);
                            var geolocation = ExecuteGetIpAdress(searchModel, conn);
                            var domains = GetDomainNames(searchModel, conn);
                            result = !CheckModelNotNull(geolocation) && !CheckModelNotNull(domains);
                        }
                        catch (Exception ex)
                        {
                            NotificationService.Fail(
                                "There was a issue while deleting data from the database. Please contact support.");
                            LoggingService.Log($"Exception: {ex.Message} \n");
                            HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);

                        }
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Fail("Could not connect to the database. Please contact support");
                    LoggingService.Log($"Exception: {ex.Message} \n");
                    HealthService.UpdateHealthState(HealthServiceType.SqlLiteService, ResponseState.Fail);
                }
            });
            return result;
        }


        #region methods

        public bool ExecuteTableExist<T>(T model, SQLiteConnection conn)
        {
            if (model.GetType().Name == nameof(DomainModel) || model.GetType().Name == nameof(IpAdressModel))
                return conn.ExecuteScalar<int>(
                    $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{model.GetType().Name}'") != 0;
            return false;
        }

        public string GetQueryCreateTable<T>(T model)
        {
            var nameOfTModel = string.Empty;
            if (nameof(IpAdressModel) == model.GetType().Name)
                nameOfTModel = $@"CREATE TABLE {nameof(IpAdressModel)} (
                                {nameof(IpAdressModel.Id)}          TEXT,
                                City        TEXT,
                                Region      TEXT,
                                Country     TEXT,
                                Latitude    TEXT,                        
                                Longitude   TEXT,
                                PRIMARY KEY( {nameof(IpAdressModel.Id)} )); ";
            if (nameof(DomainModel) == model.GetType().Name)
                nameOfTModel = $@"CREATE TABLE {nameof(DomainModel)} (
                                 {nameof(DomainModel.Name)}           TEXT,
                                 {nameof(DomainModel.IpAdressId)}     TEXT,   
                                FOREIGN KEY({nameof(DomainModel.IpAdressId)} ) REFERENCES {nameof(IpAdressModel)}({nameof(IpAdressModel.Id)})
                                PRIMARY KEY({nameof(DomainModel.Name)})); ";
            return nameOfTModel;
        }

        public bool CheckModelNotNull<T>(T model)
        {
            if (model is DomainModel hostModel)
                return !string.IsNullOrEmpty(hostModel.IpAdressId) && !string.IsNullOrEmpty(hostModel.Name);
            if (model is IpAdressModel ipAdressModel) return !string.IsNullOrEmpty(ipAdressModel.Id);
            return false;
        }

        public bool ExecuteInsert<T>(T model, SQLiteConnection conn)
        {
            if (model is DomainModel domainModel)
                return conn.Insert(domainModel) != 0;
            if (model is IpAdressModel ipAdressModel) return conn.Insert(ipAdressModel) != 0;
            return false;
        }

        public void ExecuteDeleteById(IpDomainSearchModel searchModel, SQLiteConnection conn)
        {
            var ipAdressModel = ExecuteGetIpAdress(searchModel, conn);
            conn.Execute(
                $"DELETE FROM {nameof(DomainModel)} WHERE {nameof(DomainModel.IpAdressId)} = '{ipAdressModel.Id}'");
            conn.Execute(
                $"DELETE FROM {nameof(IpAdressModel)} WHERE {nameof(IpAdressModel.Id)}= '{ipAdressModel.Id}' ");
        }

        public void ExecuteCreateTable(string model, SQLiteConnection conn)
        {
            conn.Execute("BEGIN TRANSACTION;");
            conn.Execute("PRAGMA foreign_keys=ON");
            conn.Execute(model);
            conn.Execute("COMMIT;");
            conn.Execute("VACUUM;");
        }

        public bool CheckForConstraint<T>(T model, SQLiteConnection conn)
        {
            if (model is DomainModel domain)
                return conn.Table<IpAdressModel>().Select(f => f).ToList().Select(f => f.Id).Contains(domain.IpAdressId)
                       && !conn.Table<DomainModel>().Select(f => f).ToList().Select(f => f.Name).Contains(domain.Name);

            if (model is IpAdressModel ipAdress)
                return !conn.Table<IpAdressModel>().Select(f => f).ToList().Select(f => f.Id).Contains(ipAdress.Id);
            return false;
        }

        public IpAdressModel ExecuteGetIpAdress(IpDomainSearchModel searchModel, SQLiteConnection conn)
        {
            if (searchModel.EntryDataType == EntryType.IPAddress)
            {
                return conn.Table<IpAdressModel>().Where(f => f.Id == searchModel.Id).FirstOrDefault();
            }

            if (searchModel.EntryDataType == EntryType.DomainName)
            {
                var domain = conn.Table<DomainModel>().Where(x => x.Name == searchModel.Id).FirstOrDefault();
                if (domain != null)
                {
                    return conn.Table<IpAdressModel>().Where(f => f.Id == domain.IpAdressId).FirstOrDefault();
                }
            }

            return new IpAdressModel();
        }

        public List<DomainModel> GetDomainNames(IpDomainSearchModel searchModel, SQLiteConnection conn)
        {
            if (searchModel.EntryDataType == EntryType.IPAddress)
                return conn.Table<DomainModel>().Where(x => x.IpAdressId == searchModel.Id).ToList();
            if (searchModel.EntryDataType == EntryType.DomainName)
                return conn.Table<DomainModel>().Where(x => x.Name == searchModel.Id).ToList();
            return new List<DomainModel>();
        }

        #endregion
    }
}