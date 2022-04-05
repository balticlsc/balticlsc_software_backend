using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.Types.Entities;
using System.Text.Json;
using Baltic.DataModel.Types;
using Microsoft.Extensions.Configuration;

namespace Baltic.UnitRegistry.DataAccess
{
    public class UnitRegistryInit
    {
        public IDictionary<string, List<string>> AppShelf = new ConcurrentDictionary<string, List<string>>();
        
        private IDataModelImplFactory _factory;

        private static Dictionary<string, string> _tmpFtpAccessCredential;

        private static int _globalDelay = 1;
        private static ComputationModuleRelease _mongoDbRelease;
        private static ComputationModuleRelease _ftpRelease;

        public UnitRegistryInit(IConfiguration configuration)
        {
            // IConfiguration uses appsettings.json from Baltic.Server project and environment variables
            // configuration from env variable
            _tmpFtpAccessCredential = new Dictionary<string, string>()
            {
                {"FtpHost", configuration["tmpFtpHost"]},
                {"FtpUser", configuration["tmpFtpUser"]},
                {"FtpPass", configuration["tmpFtpPass"]}
            };
            
            _factory = new DataModelImplFactory(configuration);
        }

        public static List<ComputationModule> GetInitModules()
        {
            List<ComputationModule> Cms = new List<ComputationModule>();

            AddDecisionModule(Cms);
            AddNeuralNetLearnerModule(Cms);
            AddNeuralNetRecognizerModule(Cms);
            AddMatrixOperationsModule(Cms);
            AddRegressionModule(Cms);
            AddSpectralModule(Cms);
            AddFtp2MongoModule(Cms);
            AddRgb2GrayModule(Cms);
            AddImageEdgerModule(Cms,"9104", "01");
            AddImageEdgerModule(Cms,"9107", "02");
            AddImageEdgerModule(Cms,"9108", "03");
            AddMongo2FtpModule(Cms);
            AddImageChannelSeparator(Cms);
            AddImageChannelJoiner(Cms);
            AddDataCopierModule(Cms);
            AddCopyOutModule(Cms);
            
            return Cms;
        }

        public static List<ComputationApplication> GetInitApplications()
        {
            List<ComputationApplication> Cas = new List<ComputationApplication>();

            AddFaceRecognizer(Cas);
            AddHullOptimizer(Cas);
            AddWildlifeRecognizer(Cas);
            AddSimpleImageProcessor(Cas);
            AddCovid2Analyzer(Cas);
            AddMarekImageProcessor(Cas);
            AddMarekImageProcessor2(Cas);

            return Cas;
        }

        // ========================================================================================

        public static List<TaskDataSet> GetInitDataSets()
        {
            List<TaskDataSet> ret = new List<TaskDataSet>();

            TaskDataSet ds = GetNewDataSet("MyFirstFilm", "VideoFile", "films/film1.avi", 
                "user1", CMultiplicity.Single);
            ret.Add(ds);
            ds = GetNewDataSet("MySecondFilm", "VideoFile", "films/film2.mov", 
                "user1", CMultiplicity.Single);
            ret.Add(ds);
            ds = GetNewDataSet("InputImages", "ImageFile", "in", 
                "user1", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewDataSet("OutputImages1", "ImageFile", "out1", 
                "user1", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewDataSet("OutputImages2", "ImageFile", "out2", 
                "user1", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewDataSet("OutputImages3", "ImageFile", "out3", 
                "user1", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewDataSet("OutputImages4", "ImageFile", "out4", 
                "user1", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewDataSet("OutputImages5", "ImageFile", "out5", 
                "user1", CMultiplicity.Multiple);
            ret.Add(ds);

            return ret;
        }

        private static TaskDataSet GetNewDataSet(string name, string type, string folder, string user,
            CMultiplicity mult)
        {
            return new TaskDataSet()
            {
                Name = name,
                Uid = name + "_123",
                Type = new DataType(){Name = type},
                Structure = null,
                Access = new AccessType(){Name= "FTP"},
                Data = new CDataSet() {Values = "{\"ResourcePath\" : \"/files/images/" + folder + "\"}"},
                AccessData = new CDataSet()
                {
                    Values = JsonSerializer.Serialize(new
                    {
                        Host = _tmpFtpAccessCredential["FtpHost"],
                        User = _tmpFtpAccessCredential["FtpUser"],
                        Password = _tmpFtpAccessCredential["FtpPass"],
                    })
                },
                OwnerUid = user,
                Multiplicity = mult
            };
        }
        
        //=========================================================================================

        public static List<DataType> GetInitDataTypes()
        {
            List<DataType> Dts = new List<DataType>();

            DataType dt0 = new DataType()
            {
                Name = "DataFile",
                Version = "1.0",
                Uid = "dd-001-000",
                IsBuiltIn = true,
                IsStructured = true
            };
            Dts.Add(dt0);
            DataType dt = new DataType()
            {
                Name = "JSON",
                Version = "2.3.1",
                Uid = "dd-001-001",
                IsBuiltIn = true,
                IsStructured = true,
                ParentUid = dt0.Uid
            };
            Dts.Add(dt);
            dt = new DataType()
            {
                Name = "XML",
                Version = "1.1",
                Uid = "dd-001-002",
                IsBuiltIn = true,
                IsStructured = true,
                ParentUid = dt0.Uid
            };
            Dts.Add(dt);
            
            dt0 = new DataType()
            {
                Name = "VideoFile",
                Version = "1.0",
                Uid = "dd-002-000",
                IsBuiltIn = true,
                IsStructured = true
            };
            Dts.Add(dt0);
            dt = new DataType()
            {
                Name = "MPEG4",
                Version = "2.0",
                Uid = "dd-002-001",
                IsBuiltIn = true,
                IsStructured = false,
                ParentUid = dt0.Uid
            };
            Dts.Add(dt);
            dt = new DataType()
            {
                Name = "AVI",
                Version = "2.0",
                Uid = "dd-002-002",
                IsBuiltIn = true,
                IsStructured = false,
                ParentUid = dt0.Uid
            };
            Dts.Add(dt);
            
            dt0 = new DataType()
            {
                Name = "ImageFile",
                Version = "1.0",
                Uid = "dd-003-000",
                IsBuiltIn = true,
                IsStructured = true
            };
            Dts.Add(dt0);
            dt = new DataType()
            {
                Name = "JPEG",
                Version = "2.0",
                Uid = "dd-003-001",
                IsBuiltIn = true,
                IsStructured = false,
                ParentUid = dt0.Uid
            };
            Dts.Add(dt);
            dt = new DataType()
            {
                Name = "PNG",
                Version = "2.0",
                Uid = "dd-003-002",
                IsBuiltIn = true,
                IsStructured = false,
                ParentUid = dt0.Uid
            };
            Dts.Add(dt);

            return Dts;
        }

        public static List<DataStructure> GetInitDataStructures()
        {
            List<DataStructure> Dss = new List<DataStructure>();

            DataStructure ds = new DataStructure()
            {
                Name = "Address",
                Version = "1.0",
                Uid = "dd-003-001",
                IsBuiltIn = true,
                DataSchema = "{\n\"street\" : \"string\" }"
            };
            Dss.Add(ds);
            ds = new DataStructure()
            {
                Name = "ComputationParams",
                Version = "1.0",
                Uid = "dd-003-002",
                IsBuiltIn = true,
                DataSchema = "{\n\"iterations\" : \"long\" }"
            };
            Dss.Add(ds);

            return Dss;
        }

        public static List<AccessType> GetInitAccessTypes()
        {
            List<AccessType> Ats = new List<AccessType>();

            AccessType at0 = new AccessType()
            {
                Name = "NoSQL_DB",
                Version = "1.0",
                Uid = "dd-004-000",
                IsBuiltIn = true,
                AccessSchema = "{\n\"Host\" : \"string\"," +
                               "\n\"Port\" : \"string\"," +
                               "\n\"User\" : \"string\"," +
                               "\n\"Password\" : \"string\"\n}",
                PathSchema = "{\n\"ResourcePath\" : \"string\" }",
            };
            Ats.Add(at0);
            AccessType at = new AccessType()
            {
                Name = "MongoDB",
                Version = "1.0",
                Uid = "dd-004-001",
                IsBuiltIn = true,
                AccessSchema = "{\n\"Host\" : \"string\"," +
                               "\n\"Port\" : \"string\"," +
                               "\n\"User\" : \"string\"," +
                               "\n\"Password\" : \"string\"\n}",
                PathSchema = "{\n\"Database\" : \"string\"," +
                             "\n\"Collection\" : \"string\"," +
                             "\n\"ObjectId\" : \"string\"\n}",
                StorageUid = _mongoDbRelease.Uid,
                ParentUid = at0.Uid
            };
            Ats.Add(at);
            at0 = new AccessType()
            {
                Name = "RelationalDB",
                Version = "1.0",
                Uid = "dd-005-000",
                IsBuiltIn = true,
                AccessSchema = "{\n\"Host\" : \"string\"," +
                               "\n\"Port\" : \"string\"," +
                               "\n\"User\" : \"string\"," +
                               "\n\"Password\" : \"string\"\n}",
                PathSchema = "{\n\"ResourcePath\" : \"string\" }"
            };
            Ats.Add(at0);
            at = new AccessType()
            {
                Name = "MySQL",
                Version = "1.0",
                Uid = "dd-005-001",
                IsBuiltIn = true,
                AccessSchema = "{\n\"Host\" : \"string\"," +
                               "\n\"Port\" : \"string\"," +
                               "\n\"User\" : \"string\"," +
                               "\n\"Password\" : \"string\"\n}",
                PathSchema = "{\n\"ResourcePath\" : \"string\" }",
                ParentUid = at0.Uid
            };
            Ats.Add(at);
            at = new AccessType()
            {
                Name = "FTP",
                Version = "1.0",
                Uid = "dd-006-000",
                IsBuiltIn = true,
                AccessSchema = "{\n\"Host\" : \"string\"," +
                               "\n\"Port\" : \"string\"," +
                               "\n\"User\" : \"string\"," +
                               "\n\"Password\" : \"string\"\n}",
                PathSchema = "{\n\"ResourcePath\" : \"string\" }",
                StorageUid = _ftpRelease.Uid
            };
            Ats.Add(at);
            at = new AccessType()
            {
                Name = "FileUpload",
                Version = "1.0",
                Uid = "dd-007-000",
                IsBuiltIn = true,
                AccessSchema = "",
                PathSchema = "{\n\"LocalPath\" : \"string\" }",
                ParentUid = at0.Uid
            };
            Ats.Add(at);
            at = new AccessType()
            {
                Name = "AzureDataLake",
                Version = "1.0",
                Uid = "dd-008-000",
                IsBuiltIn = true,
                AccessSchema = "{\n\"AccountName\" : \"string\"," +
                               "\n\"ClientId\" : \"string\"," +
                               "\n\"ClientSecret\" : \"string\"," +
                               "\n\"TenantId\" : \"string\"," +
                               "\n\"FileSystemName\" : \"string\"\n}",
                PathSchema = "{\n\"ResourcePath\" : \"string\" }",
                ParentUid = at0.Uid
            };
            Ats.Add(at);
            at = new AccessType()
            {
                Name = "AWSS3",
                Version = "1.0",
                Uid = "dd-009-000",
                IsBuiltIn = true,
                AccessSchema = "{\n\"AccessKey\" : \"string\"," +
                               "\n\"SecretKey\" : \"string\"," +
                               "\n\"BucketRegion\" : \"string\"," +
                               "\n\"BucketName\" : \"string\"\n}",
                PathSchema = "{\n\"ResourcePath\" : \"string\" }",
                ParentUid = at0.Uid
            };
            Ats.Add(at);

            return Ats;
        }
        
        //=========================================================================================
        
        public static ComputationModule GetMongoDbService()
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "MongoDB",
                Uid = "MongoDB_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription =
                        "Mongo Database infrastructural service1",
                    ShortDescription = "Mongo Database",
                    Icon = "https://www.balticlsc.eu/model/_icons/mongo_001.png"
                },
                AuthorUid = "user1",
                IsService = true
            };

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "MongoDB_rel_001",
                Image = "mongo:4.2.3-bionic",
                Parameters = new List<UnitParameter>()
                {
                    new UnitParameter()
                    {
                        NameOrPath = "Port",
                        DefaultValue = "27017",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    }
                },
                CredentialParameters = new List<CredentialParameter>()
                {
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "MONGO_INITDB_ROOT_USERNAME",
                        AccessCredentialName = "User",
                        DefaultCredentialValue = "someuser" // TODO - randomize elsewhere
                    },
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "MONGO_INITDB_ROOT_PASSWORD",
                        AccessCredentialName = "Password",
                        DefaultCredentialValue = "somepass" // TODO - randomize elsewhere
                    },
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "MONGO_INITDB_DATABASE",
                        DefaultCredentialValue = "images" // TODO - change from "images" to something generic
                    },
                    new CredentialParameter()
                    {
                        AccessCredentialName = "Port",
                        DefaultCredentialValue = "27017"
                    }
                },
                Version = "4.2.3",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 12, 08, 17, 13, 45),
                    Description = "Version 4.2.3",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 1024, Storage = 1 , Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 4096, Storage = 4, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            _mongoDbRelease = cmr;

            return cm;
        }
        
        //=========================================================================================
        
        public static ComputationModule GetFtpService()
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "FTP",
                Uid = "FTP_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription =
                        "FTP infrastructural service",
                    ShortDescription = "FTP service",
                    Icon = "https://www.balticlsc.eu/model/_icons/ftp_001.png"
                },
                AuthorUid = "user1",
                IsService = true
            };

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "FTP_rel_001",
                Image = "ftp-mock:this-is-mock",
                Parameters = new List<UnitParameter>()
                {
                    new UnitParameter()
                    {
                        NameOrPath = "Port",
                        DefaultValue = "27017",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    }
                },
                CredentialParameters = new List<CredentialParameter>()
                {
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "FTP_ROOT_USERNAME",
                        AccessCredentialName = "User",
                        DefaultCredentialValue = "someuser" // TODO - randomize elsewhere
                    },
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "FTP_ROOT_PASSWORD",
                        AccessCredentialName = "Password",
                        DefaultCredentialValue = "somepass" // TODO - randomize elsewhere
                    },
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "FTP_FOLDER",
                        DefaultCredentialValue = "images" // TODO - change from "images" to something generic
                    },
                    new CredentialParameter()
                    {
                        AccessCredentialName = "Port",
                        DefaultCredentialValue = "27017"
                    }
                },
                Version = "4.2.3",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 12, 08, 17, 13, 45),
                    Description = "Version 4.2.3",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 1024, Storage = 1 , Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 4096, Storage = 4, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            _ftpRelease = cmr;

            return cm;
        }

        //=========================================================================================

        private static void AddDecisionModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "User Decision",
                Uid = "UserDecision_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription =
                        "Allows to enter a decision by the user that controls passing data from the input to one of the outputs",
                    ShortDescription = "User decision on passing data from input to outputs",
                    Icon = "https://www.balticlsc.eu/model/_icons/udc_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "UserDecision_rel_001",
                Version = "0.1.0",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 11, 05, 14, 30, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 1, Storage = 0, Cpus = 1, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 1, Storage = 0, Cpus = 1, Gpus = 0
                    }
                }
            };

            cmr.Image = "Cryptic Build file contents";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "decision",
                Uid = "decision01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "user_input"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "input",
                Uid = "input01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "any_data"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output1",
                Uid = "output01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "any_data"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "output2",
                Uid = "output02",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "any_data"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private static void AddNeuralNetLearnerModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Neural Network Learner",
                Uid = "NeuralNetworkLearner_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription =
                        "Generates a trained neural network with a specified topology, based on a sequence of training data.",
                    ShortDescription = "Trains neural net from training data.",
                    Icon = "https://www.balticlsc.eu/model/_icons/nnl_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "NeuralNetworkLearner_rel_001",
                Version = "0.1.0",
                Descriptor =
                    new ReleaseDescriptor()
                    {
                        Date = new DateTime(2020, 2, 07, 14, 30, 00),
                        Description = "The initial version",
                        IsOpenSource = false,
                    },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation() {Memory = 50, Storage = 50, Cpus = 1, Gpus = 1},
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 500, Storage = 500, Cpus = 100, Gpus = 50
                    }
                },
                Image = "Cryptic Build file Build = new contents"
            };
            
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });

            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "network_parameters",
                Uid = "network_parameters01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "network_params"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "training",
                Uid = "training01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "training_set"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "trained_network",
                Uid = "trained_network01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "neural_network"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            // #### Computation Module Releases ##########

            cmr = new ComputationModuleRelease
            {
                Uid = "NeuralNetworkLearner_rel_002",
                Version = "0.2.0",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 14, 30, 00),
                    Description = "The second version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 50, Storage = 50, Cpus = 1, Gpus = 1
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 500, Storage = 500, Cpus = 100, Gpus = 50
                    }
                }
            };

            cmr.Image = "Cryptic Build file contents";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            dp1 = new DeclaredDataPin
            {
                Name = "network_parameters",
                Uid = "network_parameters01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "network_params"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            dp2 = new DeclaredDataPin
            {
                Name = "training",
                Uid = "training01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "training_set"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            dp3 = new DeclaredDataPin
            {
                Name = "trained_network",
                Uid = "trained_network01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "neural_network"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private static void AddNeuralNetRecognizerModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Neural Network Classifier",
                Uid = "NeuralNetworkCLassifier_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Classifies the input data set into a specific category.",
                    ShortDescription = "Classify data set.",
                    Icon = "https://www.balticlsc.eu/model/_icons/nnc_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "NeuralNetworkClassifier_rel_001",
                Version = "0.1.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 2, 09, 10, 30, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 50, Storage = 50, Cpus = 1, Gpus = 1
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 500, Storage = 500, Cpus = 100, Gpus = 50
                    }
                }
            };

            cmr.Image = "Cryptic Build file contents";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "trained_network",
                Uid = "trained_network01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "neural_network"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "input_value_set",
                Uid = "inputvalueset01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "value_set"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "input_classification_set",
                Uid = "inputclassset01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "value_set"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "classification_result",
                Uid = "classification_result01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "integer"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private static void AddMatrixOperationsModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Matrix Operations",
                Uid = "MatrixOperations_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Performs a sequence of operations on a matrix.",
                    ShortDescription = "Operations on a matrix.",
                    Icon = "https://www.balticlsc.eu/model/_icons/mop_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "MatrixOperations_rel_001",
                Version = "0.1.a",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 50, Storage = 10, Cpus = 1, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 500, Storage = 20, Cpus = 100, Gpus = 0
                    }
                }
            };

            cmr.Image = "Cryptic Build file contents";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "input_matrix",
                Uid = "input_matrix01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "matrix"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "matrix_operations",
                Uid = "matrix_operations01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "matrix_formula"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output_matrix",
                Uid = "output_matrix01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "matrix"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private static void AddRegressionModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Regression Algorithm",
                Uid = "RegressionAlgorithm_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Applies a regression algorithm to a specified data set..",
                    ShortDescription = "Applies regression to data.",
                    Icon = "https://www.balticlsc.eu/model/_icons/rga_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "RegressionAlgorithm_rel_001",
                Version = "0.1.a",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 50, Storage = 10, Cpus = 1, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 500, Storage = 20, Cpus = 100, Gpus = 0
                    }
                }
            };

            cmr.Image = "Cryptic Build file contents";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "input_data",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "any_data"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "parameters",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "parameters_data"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output_data",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "matrix"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private static void AddSpectralModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Spectral Analysis",
                Uid = "SpectralAnalysis_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Performs spectral analysis of images.",
                    ShortDescription = "Spectral analysis of images.",
                    Icon = "https://www.balticlsc.eu/model/_icons/spa_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "SpectralAnalysis_rel_001",
                Version = "0.1.a",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 50, Storage = 10, Cpus = 1, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 500, Storage = 20, Cpus = 100, Gpus = 0
                    }
                }
            };

            cmr.Image = "Cryptic Build file contents";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "input_data",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "any_data"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "parameters",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "parameters_data"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output_data",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "matrix"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "any_access", AccessSchema = "JSONSchema contents"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private static void AddFtp2MongoModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "FTP 2 MongoDB",
                Uid = "ftp2mongodb_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Copies files from an FTP server to a Mongo database..",
                    ShortDescription = "Copies from FTP to MongoDB.",
                    Icon = "https://www.balticlsc.eu/model/_icons/ftp2mongo_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "ftp2mongodb_rel_001",
                Version = "0.1.a",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 1, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 3, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
            cmr.Image = "balticlsc/blsc_cm_ftp2mongo:latest";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });

            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "/app/configs/params_Ftp2mongo.json", 
                DefaultValue = $"{{\n \"delay\": {_globalDelay}\n}}",
                Type = UnitParamType.Config,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "FTPDataReader",
                Uid = "FTPDataReader01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "ftp"},
                Access = new AccessType(){Name = "FTP"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "MongoDBFileWriter",
                Uid = "MongoDBFileWriter01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Multiple,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private static void AddRgb2GrayModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Image Greyer",
                Uid = "rgb2gray-mongo_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Transforms an RGB image into a monochrome image.",
                    ShortDescription = "From RGB image to monochrome.",
                    Icon = "https://www.balticlsc.eu/model/_icons/grey_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "rgb2gray-mongo_rel_001",
                Version = "0.1.a",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 50, Storage = 10, Cpus = 1, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 500, Storage = 20, Cpus = 100, Gpus = 0
                    }
                }
            };

            cmr.Image = "http://localhost:9102";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "ImageReader",
                Uid = "ImageReader01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "UncannyParameters",
                Uid = "UncannyParameters01",
                Binding = DataBinding.RequiredWeak,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "json"},
                Access = null // new AccessType(){Name = "JSON", AccessSchema = null}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "ImageWriter",
                Uid = "ImageWriter01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private static void AddImageEdgerModule(List<ComputationModule> Cms, string portNo, string suffix)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Image Edger" + suffix,
                Uid = "grey2edge_0" + suffix,
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Transforms a monochrome image into edged image (contours).",
                    ShortDescription = "From monochrome image to edges.",
                    Icon = "https://www.balticlsc.eu/model/_icons/edge_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "grey2edge_rel_0" + suffix,
                Version = "0.1.4",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 1, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 4096, Storage = 3, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Image = "balticlsc/blsc_cm_gray2edge:latest";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "/app/configs/params_Gray2edge.json", 
                DefaultValue = $"{{\n \"delay\": {_globalDelay},\n \"minVal\": 100,\n \"maxVal\": 200\n}}",
                Type = UnitParamType.Config,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });

            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "ImageReader",
                Uid = "ImageReader" + suffix,
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "UncannyParameters",
                Uid = "UncannyParameters01",
                Binding = DataBinding.RequiredWeak,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "json"},
                Access = null // new AccessType(){Name = "JSON", AccessSchema = null}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "ImageWriter",
                Uid = "ImageWriter" + suffix,
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private static void AddMongo2FtpModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "MongoDB 2 FTP",
                Uid = "mongodb2ftp_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Performs the specified computations.",
                    ShortDescription = "Specified computations",
                    Icon = "https://www.balticlsc.eu/model/_icons/mongo2ftp_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "mongodb2ftp_rel_001",
                Version = "0.1.a",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 1, Cpus = 10000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 4096, Storage = 3, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Image = "balticlsc/blsc_cm_mongo2ftp:latest";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });

            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "/app/configs/params_Mongo2ftp.json", 
                DefaultValue = $"{{\n \"delay\": {_globalDelay}\n}}",
                Type = UnitParamType.Config,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });

            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "MongoDBFileReader",
                Uid = "MongoDBFileReader01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "FTPFileWriter",
                Uid = "FTPFileWriter01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "ftp"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private static void AddImageChannelSeparator(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Image Channel Separator",
                Uid = "imagechannelseparator_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Separates colour channels for an image.",
                    ShortDescription = "Separates colour channels.",
                    Icon = "https://www.balticlsc.eu/model/_icons/split_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "imagechannelseparator_rel_001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 5, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 4096, Storage = 2, Cpus = 2000, Gpus = 1
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 8192, Storage = 4, Cpus = 4000, Gpus = 1
                    }
                }
            };
            
            cmr.Image = "balticlsc/blsc_cm_imgchannelsep:latest";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "/app/configs/params_ImgChannelSep.json", 
                DefaultValue = $"{{\n \"delay\": {_globalDelay}\n}}",
                Type = UnitParamType.Config,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "Input Color Image",
                Uid = "InputColorImage01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "Image channel 1",
                Uid = "Imagechannel1_01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Image channel 2",
                Uid = "Imagechannel2_01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "Image channel 3",
                Uid = "Imagechannel3_01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private static void AddImageChannelJoiner(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Image Channel Joiner",
                Uid = "imagechanneljoiner_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Joins colour channels for an image.",
                    ShortDescription = "Joins colour channels.",
                    Icon = "https://www.balticlsc.eu/model/_icons/join_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "imagechanneljoiner_rel_001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 5, 16, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 4096, Storage = 2, Cpus = 2000, Gpus = 1
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 8192, Storage = 4, Cpus = 4000, Gpus = 1
                    }
                }
            };
            
            cmr.Image = "balticlsc/blsc_cm_imgchanneljoin:latest";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });

            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "/app/configs/params_ImgChannelJoin.json", 
                DefaultValue = $"{{\n \"delay\": {_globalDelay}\n}}",
                Type = UnitParamType.Config,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });

            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "Output Color Image",
                Uid = "OutputColorImage01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "Image channel 1",
                Uid = "Imagechannel1_02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Image channel 2",
                Uid = "Imagechannel2_02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "Image channel 3",
                Uid = "Imagechannel3_02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = new AccessType(){Name= "MongoDB"}
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private static void AddDataCopierModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "data-copier",
                Uid = "data_copier_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Copies data from some source to some destination.",
                    ShortDescription = "Copies data",
                    Icon = "https://www.balticlsc.eu/model/_icons/default.png"
                },
                AuthorUid = "system"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "data_copier_rel_001",
                Version = "1.0",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 50, Storage = 10, Cpus = 1, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 500, Storage = 20, Cpus = 100, Gpus = 0
                    }
                }
            };

            cmr.Image = "balticlsc/blsc_data_copier:latest";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "input",
                Uid = "input001",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Unspecified,
                DataMultiplicity = CMultiplicity.Unspecified,
                Type = null,
                Access = null
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output",
                Uid = "output001",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Unspecified,
                DataMultiplicity = CMultiplicity.Unspecified,
                Type = null,
                Access = null
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private static void AddCopyOutModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "copy-out",
                Uid = "copy_out_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Copy-out.",
                    ShortDescription = "Copy-out",
                    Icon = "https://www.balticlsc.eu/model/_icons/default.png"
                },
                AuthorUid = "system"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "copy_out_rel_001",
                Version = "1.0",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 3, 15, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 50, Storage = 10, Cpus = 1, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 500, Storage = 20, Cpus = 100, Gpus = 0
                    }
                }
            };

            cmr.Image = "copy-out";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "input",
                Uid = "input002",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = null,
                Access = null
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output",
                Uid = "output002",
                Binding = DataBinding.ProvidedExternal,
                TokenMultiplicity = CMultiplicity.Single,
                Type = null,
                Access = null
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        //=========================================================================================

        public static void AddSimpleImageProcessor(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "Simple Image Processor",
                Uid = "SimpleImageProcessor_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Simple image processor that grays out photos.",
                    ShortDescription = "Grays out photos.",
                    Icon = "https://www.balticlsc.eu/model/_icons/yap_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "SimpleImageProcessor_rel_001",
                Version = "0.1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the processor",
                    IsOpenSource = false
                }
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "InputImpages",
                Uid = "11234567-1234-1234-1234-1234567890ab",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "ftp"},
                Access = null // new AccessType(){Name = "FTP Folder Name", AccessSchema = null}
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "OutputImages",
                Uid = "10234567-1234-1234-1234-1234567890ab",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "ftp"},
                Access = null // new AccessType(){Name = "FTP Server", AccessSchema = null}
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp03);
        }

        //=========================================================================================

        public static void AddFaceRecognizer(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "FaceRecognizer",
                Uid = "FaceRecognizer_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Face Recognition Application used to recognize faces in the crowd",
                    ShortDescription = "Recognizes faces in the crowd",
                    Icon = "https://www.balticlsc.eu/model/_icons/fcr_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "FaceRecognizer_rel_001",
                Version = "0.1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the recognizer",
                    IsOpenSource = false
                }
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "Face",
                Uid = "Face01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "picture"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "RecognitionParams",
                Uid = "RecognitionParams01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "picture"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "PersonRecord",
                Uid = "PersonRecord01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "person"},
                Access = null
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);
        }

        //=========================================================================================

        public static void AddHullOptimizer(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "Hull Optimizer",
                Uid = "HullOptimizer_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription =
                        "Ship hull optimization application used to optimize shapes of hulls under various criteria.",
                    ShortDescription = "Optimizes ship hulls.",
                    Icon = "https://www.balticlsc.eu/model/_icons/hlo_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "HullOptimizer_rel_001",
                Version = "0.1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the optimizer",
                    IsOpenSource = false
                }
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "HullInput",
                Uid = "HullData01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "picture"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "OptimizationParams",
                Uid = "OptimizationParams01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "picture"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "Hull",
                Uid = "Hull01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "person"},
                Access = null
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);
        }

        //=========================================================================================

        public static void AddWildlifeRecognizer(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "WildlifeRecognizer",
                Uid = "WildlifeRecognizer_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Wildlife Recognition Application used to recognize wild animals",
                    ShortDescription = "Recognizes animals",
                    Icon = "https://www.balticlsc.eu/model/_icons/wlr_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "WildlifeRecognizer_rel_001",
                Version = "0.1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the recognizer",
                    IsOpenSource = false
                }
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "AnimalPhoto",
                Uid = "AnimalPhoto01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "picture"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "RecognitionParams",
                Uid = "RecognitionParams02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "picture"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "AnimalRecord",
                Uid = "AnimalRecord01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "animal"},
                Access = null
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);
        }

        //=========================================================================================

        public static void AddCovid2Analyzer(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "Covid-2 Analyzer",
                Uid = "Covid2Analyzer_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Analyzes pandemics data, specific to Covid-2",
                    ShortDescription = "Analysis of Covid-2",
                    Icon = "https://www.balticlsc.eu/model/_icons/cva_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab",
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "Covid2Analyzer_rel_001",
                Version = "0.1",
                DiagramUid = "d2234567-1234-1234-1234-1234567890ab",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the analyzer",
                    IsOpenSource = false
                }
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "GeographicDistribution",
                Uid = "GeographicDistribution01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "picture"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "AnalysisParams",
                Uid = "AnalysisParams02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "picture"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "AnalysisResults",
                Uid = "AnalysisResults01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "distribution"},
                Access = null
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);
        }

        public static void AddMarekImageProcessor(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "Marek's Image Processor",
                Uid = "MarekImageProcessor_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Processes images by splitting into RGB and edging.",
                    ShortDescription = "Processes images in RGB.",
                    Icon = "https://www.balticlsc.eu/model/_icons/yap_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = "2b17d401-440b-4e78-acf3-2f055a6182a0",
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "MarekImageProcessor_rel_001",
                Version = "0.1",
                DiagramUid = "fc9d3cb3-5aa0-4d2c-8e17-bf1acccbfdea",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 07, 17, 12, 0, 0),
                    Description = "First version of the processor",
                    IsOpenSource = false
                }
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "InputImages",
                Uid = "2727963b-d335-43b6-8698-4088abb7d16d",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "ftp"},
                Access = null // new AccessType(){Name = "FTP Folder Name", AccessSchema = null}
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "OutputImages",
                Uid = "85f80054-720e-4aee-b404-fcecc87fa95b",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "ftp"},
                Access = null // new AccessType(){Name = "FTP Server", AccessSchema = null}
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp03);
        }
        
        public static void AddMarekImageProcessor2(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "Marek's Image Processor 2",
                Uid = "MarekImageProcessor2_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Processes images by splitting into RGB and edging.",
                    ShortDescription = "Processes images in RGB.",
                    Icon = "https://www.balticlsc.eu/model/_icons/yap_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = "7fc58d9f-387c-4be2-8e71-99c54e5d1134",
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "MarekImageProcessor2_rel_001",
                Version = "0.1",
                DiagramUid = "54b8e49f-2b07-4ab2-91e7-2fe6637a8ec7",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 07, 17, 12, 0, 0),
                    Description = "First version of the processor",
                    IsOpenSource = false
                }
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "InputImages",
                Uid = "a5301f9e-305a-4fab-beba-bfa1ba7ab0ee",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "ftp"},
                Access = new AccessType(){Name= "FTP"}
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "OutputImages",
                Uid = "6a6b05da-cfae-4047-90cb-c711c71205f0",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "ftp"},
                Access = new AccessType(){Name= "FTP"}
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp03);
        }
    }
}