using System;
using GeolocationApp.Model;
using GeolocationApp.Interface;
using GeolocationApp.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SQLite;

namespace GeolocationUnitTest
{
    [TestClass]
    public class SqlLiteCommandTest
    {
        private readonly string DBMemory = ":memory:";
        public InputValidationService InputValidationService;

        public Mock<ILoggingService> MockILoggingService;
        public Mock<INotificationService> MockNotificationService;
        public Mock<IHealthService> MockHealthService;


        public SqlLiteService SqlLiteDbHelper;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            MockILoggingService = new Mock<ILoggingService>();
            MockNotificationService = new Mock<INotificationService>();
            MockHealthService = new Mock<IHealthService>();

            SqlLiteDbHelper = new SqlLiteService(MockNotificationService.Object, MockILoggingService.Object, MockHealthService.Object);
            InputValidationService = new InputValidationService(
                MockNotificationService.Object,
                MockILoggingService.Object, MockHealthService.Object
            );
        }

        [TestMethod]
        public void CreateTable_GeolocationModel_ReturnTrue()
        {
            // Arrange
            var error = string.Empty;
            var result = false;
            var nameOfModel = new IpAdressModel();


            var model = SqlLiteDbHelper.GetQueryCreateTable(nameOfModel);
            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(model, conn);
                    // Assert
                    result = SqlLiteDbHelper.ExecuteTableExist(nameOfModel, conn);
                    TestContext.WriteLine($"{nameOfModel} is created {result} ");
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
                error = ex.Message;
            }

            Assert.IsTrue(string.IsNullOrEmpty(error));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CreateTable_AllModels_ReturnTrue()
        {
            // Arrange
            var error = string.Empty;
            var result = false;


            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);

                    // Assert
                    result = SqlLiteDbHelper.ExecuteTableExist(ipAdressModelName, conn) &&
                             SqlLiteDbHelper.ExecuteTableExist(domainModelName, conn);
                    TestContext.WriteLine($"{domainModelName} is created {result} ");
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
            }

            Assert.IsTrue(string.IsNullOrEmpty(error));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CreateTable_NonExistModel_ReturnFalse()
        {
            // Arrange
            var error = string.Empty;
            var result = false;
            var nameOfModel = "NonExistMode";
            var model = SqlLiteDbHelper.GetQueryCreateTable(nameOfModel);
            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(model, conn);
                    // Assert
                    result = SqlLiteDbHelper.ExecuteTableExist(nameOfModel, conn);
                    TestContext.WriteLine($"{nameOfModel} is created {result} ");
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
                error = ex.Message;
            }

            Assert.IsFalse(string.IsNullOrEmpty(error));
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Insert_IPAdressToGeolocation_ReturnTrue()
        {
            // Arrange
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };


            geolocation = new IpAdressModel
            {
                Id = "89.64.75.189", City = "Ursynów", Region = "Mazovia", Country = "PL", Latitude = "52.1540",
                Longitude = "21.0514"
            };

            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);

                    // Assert
                    result = SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Insert_IPAdressToGeolocationConstraintError_ReturnFalse()
        {
            // Arrange
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };
            var duplicateKeyError = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Amsterdam",
                Region = "Noord-Holland",
                Country = "NL",
                Latitude = "2.1540",
                Longitude = "1.0514"
            };


            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);


                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    // Assert
                    result = SqlLiteDbHelper.ExecuteInsert(duplicateKeyError, conn);
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Insert_fqdnModelNameGeolocation_ReturnTrue()
        {
            // Arrange
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };

            var fqdnModel = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };


            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);
                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    // Assert
                    result = SqlLiteDbHelper.ExecuteInsert(fqdnModel, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Insert_fqdnModelNameGeolocationConstraintError_ReturnFalse()
        {
            // Arrange
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);


            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };

            var fqdnModel = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };
            var fqdnModel2 = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };


            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);
                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    SqlLiteDbHelper.ExecuteInsert(fqdnModel, conn);

                    // Assert
                    result = SqlLiteDbHelper.ExecuteInsert(fqdnModel2, conn);
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Insert_fqdnModelNameConstraintError_ReturnFalse()
        {
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };

            var fqdnModel = new DomainModel
            {
                IpAdressId = "89.64.75.000",
                Name = "www.google.com"
            };
            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);
                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);

                    // Assert
                    result = SqlLiteDbHelper.ExecuteInsert(fqdnModel, conn);
                }
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
            }

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void Read_GeolocationbyIPAdress_ReturnTrue()
        {
            // Arrange
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };

            IpAdressModel ipadress = null;
            var ipDomainSearchModel = InputValidationService.GetIpDomainSearchModel(geolocation.Id);
            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);


                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    // Assert
                    ipadress = SqlLiteDbHelper.ExecuteGetIpAdress(ipDomainSearchModel, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var compareStrings = CompareModels(geolocation, ipadress);
            LogErrorMessage(compareStrings);
            Assert.IsTrue(string.IsNullOrEmpty(compareStrings));
        }

        [TestMethod]
        public void Read_GeolocationbyIPAdress_ReturnFalse()
        {
            // Arrange
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);


            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };


            var geolocationChange = new IpAdressModel
            {
                Id = "89.64.75.186",
                City = "Ursynow",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };

            var input = "89.64.75.189";

            var ipDomainSearchModel = InputValidationService.GetIpDomainSearchModel(input);

            IpAdressModel ipadress = null;
            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);


                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    // Assert
                    ipadress = SqlLiteDbHelper.ExecuteGetIpAdress(ipDomainSearchModel, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var compareStrings = CompareModels(geolocation, geolocationChange);
            LogErrorMessage(compareStrings);
            Assert.IsFalse(string.IsNullOrEmpty(compareStrings));
        }


        [TestMethod]
        public void Read_GeolocationbyfqdnModel_ReturnTrue()
        {
            // Arrange
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainCreateQuery = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationCreateQuery = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };
            var domainModel = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };

            var fqdnModelName = "www.google.com";
            var searchModel = InputValidationService.GetIpDomainSearchModel(fqdnModelName);

            IpAdressModel ipadress = null;

            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationCreateQuery, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainCreateQuery, conn);


                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    SqlLiteDbHelper.ExecuteInsert(domainModel, conn);
                    // Assert
                    ipadress = SqlLiteDbHelper.ExecuteGetIpAdress(searchModel, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var compareStrings = CompareModels(geolocation, ipadress);
            LogErrorMessage(compareStrings);
            Assert.IsTrue(string.IsNullOrEmpty(compareStrings));
        }

        [TestMethod]
        public void Read_GeolocationbyfqdnModel_ReturnFalse()
        {
            // Arrange
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainCreateQuery = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationCreateQuery = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };
            var geolocationChange = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1640",
                Longitude = "21.0314"
            };
            var fqdnModel = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };

            var input = "www.google.com";
            var ipDomainSearchModel = InputValidationService.GetIpDomainSearchModel(input);
            IpAdressModel ipadress = null;

            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationCreateQuery, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainCreateQuery, conn);


                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    SqlLiteDbHelper.ExecuteInsert(fqdnModel, conn);
                    // Assert
                    ipadress = SqlLiteDbHelper.ExecuteGetIpAdress(ipDomainSearchModel, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var compareStrings = CompareModels(geolocation, geolocationChange);
            LogErrorMessage(compareStrings);
            Assert.IsFalse(string.IsNullOrEmpty(compareStrings));
        }


        [TestMethod]
        public void Delete_GeolocationbyIPAdress_ReturnTrue()
        {
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);


            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };
            var fqdnModel = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };

            var input = geolocation.Id;
            var ipDomainSearchModel = InputValidationService.GetIpDomainSearchModel(input);

            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);


                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);

                    SqlLiteDbHelper.ExecuteDeleteById(ipDomainSearchModel, conn);
                    // Assert
                    result = SqlLiteDbHelper.ExecuteGetIpAdress(ipDomainSearchModel, conn) == null;
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void Delete_DomainNamebyIPAdress_ReturnTrue()
        {
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };
            var fqdnModel = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };


            var input = fqdnModel.Name;
            var ipDomainSearchModel = InputValidationService.GetIpDomainSearchModel(input);
            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);

                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    SqlLiteDbHelper.ExecuteInsert(fqdnModel, conn);

                    SqlLiteDbHelper.ExecuteDeleteById(ipDomainSearchModel, conn);


                    // Assert
                   var  ipAdressModel = SqlLiteDbHelper.ExecuteGetIpAdress(ipDomainSearchModel, conn);
                  result = string.IsNullOrEmpty(ipAdressModel.Id);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void Function_GeolocationModelCheckForConstraint_ReturnFalse()
        {
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);


            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };


            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);
                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);

                    // Assert
                    result = SqlLiteDbHelper.CheckForConstraint(geolocation, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Function_GeolocationModelCheckForConstraint_ReturnTrue()
        {
            // Arrange
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };


            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);
                    // Assert
                    result = SqlLiteDbHelper.CheckForConstraint(geolocation, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Function_fqdnModelModelCheckForConstraint_ReturnIsFalse()
        {
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };


            var fqdnModel = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };


            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);
                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    SqlLiteDbHelper.ExecuteInsert(fqdnModel, conn);

                    // Assert
                    result = SqlLiteDbHelper.CheckForConstraint(fqdnModel, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Function_fqdnModelModelCheckForConstraint_ReturnIsTrue()
        {
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IpAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };


            var domain = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };

            var geolocationOther = new IpAdressModel
            {
                Id = "89.64.75.180",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };

            var domainFail = new DomainModel
            {
                IpAdressId = "89.64.75.180",
                Name = "www.google.nl"
            };


            // Act
            try
            {
                using (var conn = new SQLiteConnection(DBMemory))
                {
                    SqlLiteDbHelper.ExecuteCreateTable(geolocationModel, conn);
                    SqlLiteDbHelper.ExecuteCreateTable(domainModel, conn);
                    SqlLiteDbHelper.ExecuteInsert(geolocation, conn);
                    SqlLiteDbHelper.ExecuteInsert(domain, conn);
                    SqlLiteDbHelper.ExecuteInsert(geolocationOther, conn);


                    // Assert
                    result = SqlLiteDbHelper.CheckForConstraint(domainFail, conn);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void Function_GeolocationModelcheckModelNotNull_ReturnTrue()
        {
            // Arrange
            var result = false;

            // Act
            var geolocation = new IpAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };

            // Assert
            result = SqlLiteDbHelper.CheckModelNotNull(geolocation);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Function_GeolocationModelcheckModelNotNull_ReturnFalse()
        {
            // Arrange
            var result = false;

            // Act
            var geolocation = new IpAdressModel
            {
                Id = "",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };

            // Assert
            result = SqlLiteDbHelper.CheckModelNotNull(geolocation);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Function_fqdnModelModelcheckModelNotNull_ReturnTrue()
        {
            // Arrange
            var result = false;

            // Act
            var model = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = "www.google.com"
            };

            // Assert
            result = SqlLiteDbHelper.CheckModelNotNull(model);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Function_fqdnModelModelcheckModelNotNull_ReturnFalse()
        {
            // Arrange
            var result = false;

            // Act
            var model = new DomainModel
            {
                IpAdressId = "89.64.75.189",
                Name = ""
            };

            // Assert
            result = SqlLiteDbHelper.CheckModelNotNull(model);

            Assert.IsFalse(result);
        }


        public string CompareModels(object model_Insert, object model_Read)
        {
            // to do refactor
            if (model_Insert.GetType() != model_Read.GetType())
                return $"Models are of different types.\n {model_Insert.GetType()} : {model_Read.GetType()} ";

            var properties = model_Insert.GetType().GetProperties();
            var mismatches = "";

            foreach (var property in properties)
            {
                var value1 = property.GetValue(model_Insert)?.ToString();
                var value2 = property.GetValue(model_Read)?.ToString();

                if (!value1.Equals(value2))
                    mismatches += $"{property.Name}: " +
                                  $"{value1} {value2}\n";
            }

            return mismatches;
        }


        private void LogErrorMessage(string error)
        {
            if (!string.IsNullOrEmpty(error)) TestContext.WriteLine(error);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            MockILoggingService = null;
            MockNotificationService = null;
            SqlLiteDbHelper = null;
        }
    }
}