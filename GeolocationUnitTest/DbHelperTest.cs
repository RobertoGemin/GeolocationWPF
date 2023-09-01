using GeolocationApp.Model;
using GeolocationApp.ViewModel.Interface;
using GeolocationUnitTest.Function;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GeolocationApp.ViewModel.Service;


namespace GeolocationUnitTest
{
    [TestClass]
    public class DbHelperTest
    {
        public TestContext TestContext { get; set; }

        private readonly string DBMemory = ":memory:";

        public Mock<ILoggingService> MockILoggingService;
        public Mock<INotificationManager> MockNotificationManager;
        public InputValidationService InputValidationService;


        public SqlLiteService SqlLiteDbHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            MockILoggingService = new Mock<ILoggingService>();
            MockNotificationManager = new Mock<INotificationManager>();
            SqlLiteDbHelper = new SqlLiteService(MockNotificationManager.Object, MockILoggingService.Object);
            InputValidationService = new InputValidationService(
                MockNotificationManager.Object,
                MockILoggingService.Object
            );
        }

        [TestMethod]
        public void CreateTable_GeolocationModel_ReturnTrue()
        {
            // Arrange
            var error = string.Empty;
            var result = false;
            var nameOfModel = new IPAdressModel();


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
            var ipAdressModelName = new IPAdressModel();
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
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };


            geolocation = new IPAdressModel
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
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };
            var duplicateKeyError = new IPAdressModel
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
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
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
                IPAdressId = "89.64.75.189",
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
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);


            var geolocation = new IPAdressModel
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
                IPAdressId = "89.64.75.189",
                Name = "www.google.com"
            };
            var fqdnModel2 = new DomainModel
            {
                IPAdressId = "89.64.75.189",
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
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
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
                IPAdressId = "89.64.75.000",
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
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };

            IPAdressModel ipadress = null;
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
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);


            var geolocation = new IPAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };


            var geolocationChange = new IPAdressModel
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

            IPAdressModel ipadress = null;
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
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IPAdressModel();
            var domainCreateQuery = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationCreateQuery = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
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
                IPAdressId = "89.64.75.189",
                Name = "www.google.com"
            };

            var fqdnModelName = "www.google.com";
            var searchModel = InputValidationService.GetIpDomainSearchModel(fqdnModelName);

            IPAdressModel ipadress = null;

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

            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IPAdressModel();
            var domainCreateQuery = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationCreateQuery = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);


            var geolocation = new IPAdressModel
            {
                Id = "89.64.75.189",
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };
            var geolocationChange = new IPAdressModel
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
                IPAdressId = "89.64.75.189",
                Name = "www.google.com"
            };

            var input = "www.google.com";
            var ipDomainSearchModel = InputValidationService.GetIpDomainSearchModel(input);
            IPAdressModel ipadress = null;

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
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);


            var geolocation = new IPAdressModel
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
                IPAdressId = "89.64.75.189",
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
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
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
                IPAdressId = "89.64.75.189",
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
        public void Function_GeolocationModelCheckForConstraint_ReturnFalse()
        {
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);


            var geolocation = new IPAdressModel
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
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
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
        public void Function_fqdnModelModelCheckForConstraint_ReturnTrue()
        {
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
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
                IPAdressId = "89.64.75.189",
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

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Function_fqdnModelModelCheckForConstraint_ReturnFalse()
        {
            // Arrange
            var result = false;
            var domainModelName = new DomainModel();
            var ipAdressModelName = new IPAdressModel();
            var domainModel = SqlLiteDbHelper.GetQueryCreateTable(domainModelName);
            var geolocationModel = SqlLiteDbHelper.GetQueryCreateTable(ipAdressModelName);

            var geolocation = new IPAdressModel
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
                IPAdressId = "89.64.75.189",
                Name = "www.google.com"
            };

            var geolocationOther = new IPAdressModel
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
                IPAdressId = "89.64.75.180",
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

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void Function_GeolocationModelcheckModelNotNull_ReturnTrue()
        {
            // Arrange
            var result = false;

            // Act
            var geolocation = new IPAdressModel
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
            var geolocation = new IPAdressModel
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
                IPAdressId = "89.64.75.189",
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
                IPAdressId = "89.64.75.189",
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
            MockNotificationManager = null;
            SqlLiteDbHelper = null;
        }
    }
}
