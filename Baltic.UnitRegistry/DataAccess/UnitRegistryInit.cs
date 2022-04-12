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
    
    // TODO - check and update to conform with UnitRegistryMock
    
    public class UnitRegistryInit : UnitGeneralDaoImpl
    {
        private string _tmpApp1DiagramUid;
        private string _tmpRel1DiagramUid;
        private string _tmpRel1InputPinUid;
        private string _tmpRel1OutputPinUid;
        private string _tmpApp2DiagramUid;
        private string _tmpRel2DiagramUid;
        private string _tmpRel2InputPinUid;
        private string _tmpRel2OutputPinUid;
        private string _tmpApp3DiagramUid;
        private string _tmpRel3DiagramUid;
        private string _tmpRel3InputPinUid;
        private string _tmpRel3OutputPinUid;
        private string _tmpRel3Output2PinUid;
        private string _tmpApp4DiagramUid;
        private string _tmpRel4DiagramUid;
        private string _tmpRel4Input1PinUid;
        private string _tmpRel4Input2PinUid;
        private string _tmpRel4OutputPinUid;

        private Dictionary<string, string> _tmpFtpAccessCredential;

        private int _globalDelay = 1;
        private ComputationModuleRelease _mongoDbRelease;
        private ComputationModuleRelease _ftpRelease;
        
        public List<DataType> Dts = new List<DataType>();
        public List<DataStructure> Dss = new List<DataStructure>();
        public List<AccessType> Ats = new List<AccessType>();

        public UnitRegistryInit(IConfiguration configuration)
        {
            // IConfiguration uses appsettings.json from Baltic.Server project and environment variables
            // configuration from env variable
            _tmpApp1DiagramUid = configuration["tmpApp1DiagramUid"];
            _tmpRel1DiagramUid = configuration["tmpRel1DiagramUid"];
            _tmpRel1InputPinUid = configuration["tmpRel1InputPinUid"];
            _tmpRel1OutputPinUid = configuration["tmpRel1OutputPinUid"];
            _tmpApp2DiagramUid = configuration["tmpApp2DiagramUid"];
            _tmpRel2DiagramUid = configuration["tmpRel2DiagramUid"];
            _tmpRel2InputPinUid = configuration["tmpRel2InputPinUid"];
            _tmpRel2OutputPinUid = configuration["tmpRel2OutputPinUid"];
            _tmpApp3DiagramUid = configuration["tmpApp3DiagramUid"];
            _tmpRel3DiagramUid = configuration["tmpRel3DiagramUid"];
            _tmpRel3InputPinUid = configuration["tmpRel3InputPinUid"];
            _tmpRel3OutputPinUid = configuration["tmpRel3OutputPinUid"];
            _tmpRel3Output2PinUid = configuration["tmpRel3Output2PinUid"];
            _tmpApp4DiagramUid = configuration["tmpApp4DiagramUid"];
            _tmpRel4DiagramUid = configuration["tmpRel4DiagramUid"];
            _tmpRel4Input1PinUid = configuration["tmpRel4Input1PinUid"];
            _tmpRel4Input2PinUid = configuration["tmpRel4Input2PinUid"];
            _tmpRel4OutputPinUid = configuration["tmpRel4OutputPinUid"];
            
            _tmpFtpAccessCredential = new Dictionary<string, string>()
            {
                {"FtpHost", configuration["tmpFtpHost"]},
                {"FtpUser", configuration["tmpFtpUser"]},
                {"FtpPass", configuration["tmpFtpPass"]},
                {"FtpHost2", configuration["tmpFtpHost2"]},
                {"FtpUser2", configuration["tmpFtpUser2"]},
                {"FtpPass2", configuration["tmpFtpPass2"]}
            };
        }

        public List<ComputationModule> GetInitModules()
        {
            List<ComputationModule> Cms = new List<ComputationModule>();
            
            AddFaceRecogniserModule(Cms); // module by Jan Bielecki
            
            AddImageChannelSeparator(Cms); // module by Marek Wdowiak
            AddImageChannelJoiner(Cms); // module by Marek Wdowiak
            AddImageEdgerModule(Cms, "9104", "01"); // module by Marek Wdowiak
            AddImageEdgerModule(Cms, "9107", "02");
            AddImageEdgerModule(Cms, "9108", "03");
            
            AddFaceDetectorModule(Cms); // module by Adam Gawieńczuk
            AddFaceContoursModule(Cms); // module by Robert Mazurek
            AddMoodRecogniserModule(Cms); // module by Jena Smruti
            AddDataSummarizerModule(Cms); // module by Adam Gawieńczuk

            AddImageClassificationTrainerModule(Cms); // module by Michał Pawlikowski

            // "Fake" modules
            AddDecisionModule(Cms);
            AddNeuralNetLearnerModule(Cms);
            AddNeuralNetRecognizerModule(Cms);
            AddMatrixOperationsModule(Cms);
            AddRegressionModule(Cms);
            AddSpectralModule(Cms);
            
            // Deprecated modules
            AddRgb2GrayModule(Cms);
            AddMongo2FtpModule(Cms);
            AddFtp2MongoModule(Cms);
            
            // System modules
            AddDataCopierModule(Cms);

            return Cms;
        }

        public List<ComputationApplication> GetInitApplications()
        {
            List<ComputationApplication> Cas = new List<ComputationApplication>();

            AddFaceRecognizer(Cas);
            AddMoodRecognizer(Cas);
            AddMarekImageProcessor2(Cas);
            AddImageClassificationTrainer(Cas);
            AddHullOptimizer(Cas);
            AddWildlifeRecognizer(Cas);
            AddSimpleImageProcessor(Cas);
            AddCovid2Analyzer(Cas);
            AddMarekImageProcessor(Cas);

            return Cas;
        }

        // ========================================================================================

        public List<TaskDataSet> GetInitDataSets()
        {
            List<TaskDataSet> ret = new List<TaskDataSet>();
            
            // -------- User: user1 -------------------
            
            TaskDataSet ds = GetNewEdgerDataSet("MyFirstFilm", "MyFirstFilm_001", "VideoFile", "films/film1.avi", 
                "user1", "", CMultiplicity.Single);
            ret.Add(ds);
            ds = GetNewEdgerDataSet("MySecondFilm", "MySecondFilm_001", "VideoFile", "films/film2.mov", 
                "user1", "", CMultiplicity.Single);
            ret.Add(ds);
            ds = GetNewEdgerDataSet("InputImages1", "InputImages_001", "ImageFile", "source1", 
                "user1", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewEdgerDataSet("OutputImages1", "OutputImages1_001", "ImageFile", "target1", 
                "user1", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewEdgerDataSet("OutputImages2", "OutputImages2_001", "ImageFile", "source2", 
                "user1", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewEdgerDataSet("OutputImages3", "OutputImages3_001", "ImageFile", "target2", 
                "user1", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewFaceDataSet("FaceInput1", "FaceInput1_001", "ImageFile", "source1", 
                "user1", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewFaceDataSet("FaceOutput1", "FaceOutput1_001", "ImageFile", "target1", 
                "user1", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewFaceDataSet("FaceInput2", "FaceInput2_001", "ImageFile", "source2", 
                "user1", "2", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewFaceDataSet("FaceOutput2", "FaceOutput2_001", "ImageFile", "target2", 
                "user1", "2", CMultiplicity.Multiple);
            ret.Add(ds);
            
            ds = GetNewFaceDataSet("FaceInput3", "FaceInput3_001", "ImageFile", "source3", 
                "user1", "", CMultiplicity.Multiple);
            ret.Add(ds);
            
            ds = GetNewFaceDataSet("FaceOutput3", "FaceOutput3_001", "ImageFile", "target3", 
                "user1", "", CMultiplicity.Multiple);
            ret.Add(ds);
            
            ds = GetNewFaceDataSet("FaceInput4", "FaceInput4_001", "JSON", "metadata.json", 
                "user1", "", CMultiplicity.Single);
            ret.Add(ds);
            ds = GetNewFaceDataSet("FaceOutput4", "FaceInput3_001", "DataFile", "model.xml", 
                "user1", "", CMultiplicity.Single);
            ret.Add(ds);

            // -------- User: demo -----------------
            
            ds = GetNewEdgerDataSet("EdgerDemoInput1", "DefaultDataSet_001", "ImageFile", "source1", 
                "demo", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewEdgerDataSet("EdgerDemoOutput1", "DefaultDataSet_002", "ImageFile", "target1", 
                "demo", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewEdgerDataSet("EdgerDemoInput2", "DefaultDataSet_003", "ImageFile", "source2", 
                "demo", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewEdgerDataSet("EdgerDemoOutput2", "DefaultDataSet_004", "ImageFile", "target2", 
                "demo", "", CMultiplicity.Multiple);
            ret.Add(ds);
            
            ds = GetNewFaceDataSet("FaceDemoInput1", "DefaultDataSet_005", "ImageFile", "source1", 
                "demo", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewFaceDataSet("FaceDemoOutput1", "DefaultDataSet_006", "ImageFile", "target1", 
                "demo", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewFaceDataSet("FaceDemoInput2", "DefaultDataSet_007", "ImageFile", "source2", 
                "demo", "", CMultiplicity.Multiple);
            ret.Add(ds);
            ds = GetNewFaceDataSet("FaceDemoOutput2", "DefaultDataSet_008", "ImageFile", "source1/windows.jpg", 
                "demo", "", CMultiplicity.Single);
            ret.Add(ds);

            return ret;
        }

       private TaskDataSet GetNewEdgerDataSet(string name, string uid, string type, string folder, string user, string postfix, 
            CMultiplicity mult)
        {
            return new TaskDataSet()
            {
                Name = name,
                Uid = uid,
                Access = MapAccessType(ATypeTable.Single(new {name = "FTP"})),
                Type =  MapDataType(DTypeTable.Single(new {name = type})),
                Structure = null,
                Data = new CDataSet() {Values = "{\n  \"ResourcePath\" : \"/files/edger/" + folder + "\"\n}"},
                AccessData = new CDataSet()
                {
                    Values = JsonSerializer.Serialize(new
                    {
                        Host = _tmpFtpAccessCredential["FtpHost" + postfix],
                        User = _tmpFtpAccessCredential["FtpUser" + postfix],
                        Password = _tmpFtpAccessCredential["FtpPass" + postfix],
                    }, new JsonSerializerOptions(){WriteIndented = true})
                },
                OwnerUid = user,
                Multiplicity = mult
            };
        }
        
        private TaskDataSet GetNewFaceDataSet(string name, string uid, string type, string folder, string user,
            string postfix, CMultiplicity mult)
        {
            return new TaskDataSet()
            {
                Name = name,
                Uid = uid,
                Access = MapAccessType(ATypeTable.Single(new {name = "FTP"})),
                Type = MapDataType(DTypeTable.Single(new {name = type})),
                Structure = null,
                Data = new CDataSet() {Values = "{\n  \"ResourcePath\" : \"/files/recogniser/" + folder + "\"\n}"},
                AccessData = new CDataSet()
                {
                    Values = JsonSerializer.Serialize(new
                    {
                        Host = _tmpFtpAccessCredential["FtpHost" + postfix],
                        User = _tmpFtpAccessCredential["FtpUser" + postfix],
                        Password = _tmpFtpAccessCredential["FtpPass" + postfix],
                    }, new JsonSerializerOptions() {WriteIndented = true})
                },
                OwnerUid = user,
                Multiplicity = mult
            };
        }
        
        //=========================================================================================

        public List<DataType> GetInitDataTypes()
        {
            if (0 != Dts.Count)
                return Dts;
            DataType dt = new DataType()
            {
                Name = "DirectData",
                Version = "1.0",
                Uid = "dd-999-000",
                IsBuiltIn = true,
                IsStructured = true
            };
            Dts.Add(dt);
            DataType dt0 = new DataType()
            {
                Name = "DataFile",
                Version = "1.0",
                Uid = "dd-001-000",
                IsBuiltIn = true,
                IsStructured = true
            };
            Dts.Add(dt0);
            dt = new DataType()
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
                IsStructured = false
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
                IsStructured = false
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

        public List<DataStructure> GetInitDataStructures()
        {
            if (0 != Dss.Count)
                return Dss;
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
            ds = new DataStructure()
            {
                Name = "ConnectionString",
                Version = "1.0",
                Uid = "dd-003-003",
                IsBuiltIn = false,
                DataSchema = "{\n\"connectionstring\" : \"string\",\"dir\" : \"string\" }"
            };
            Dss.Add(ds);

            return Dss;
        }

        public List<AccessType> GetInitAccessTypes()
        {
            if (0 != Ats.Count)
                return Ats;
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
                StorageUid = _mongoDbRelease.Uid // TODO  - update with proper storage
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
            at = new AccessType()
            {
                Name = "GridFS",
                Version = "1.0",
                Uid = "dd-004-002",
                IsBuiltIn = true,
                AccessSchema = "{\n\"Host\" : \"string\"," +
                               "\n\"Port\" : \"string\"," +
                               "\n\"User\" : \"string\"," +
                               "\n\"Password\" : \"string\"\n}",
                PathSchema = "{\n\"Database\" : \"string\"," +
                             "\n\"Bucket\" : \"string\"," +
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
                PathSchema = "{\n\"ResourcePath\" : \"string\" }",
                StorageUid = _mongoDbRelease.Uid // TODO  - update with proper storage
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
                ParentUid = at0.Uid,
                StorageUid = _mongoDbRelease.Uid // TODO  - update with proper storage
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
                ParentUid = at0.Uid,
                StorageUid = _ftpRelease.Uid // TODO  - update with proper storage
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
                ParentUid = at0.Uid,
                StorageUid = _ftpRelease.Uid // TODO  - update with proper storage
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
                ParentUid = at0.Uid,
                StorageUid = _ftpRelease.Uid // TODO  - update with proper storage
            };
            Ats.Add(at);

            return Ats;
        }
        
        //=========================================================================================
        
        public ComputationModule GetMongoDbService()
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
                AuthorUid = "system",
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
        
        public ComputationModule GetFtpService()
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
                AuthorUid = "system",
                IsService = true
            };

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "FTP_rel_001",
                Image = "stilliard/pure-ftpd",
                Parameters = new List<UnitParameter>()
                {
                    new UnitParameter()
                    {
                        // NameOrPath = "Port",
                        DefaultValue = "21",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30000",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30001",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30002",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30003",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30004",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30005",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30006",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30007",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30008",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    },
                    new UnitParameter()
                    {
                        DefaultValue = "30009",
                        Type = UnitParamType.Port,
                        IsMandatory = true,
                        Uid = Guid.NewGuid().ToString()
                    }
                },
                CredentialParameters = new List<CredentialParameter>()
                {
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "FTP_USER_NAME",
                        AccessCredentialName = "User",
                        // DefaultCredentialValue = "someuser" // TODO - randomize elsewhere
                        DefaultCredentialValue = _tmpFtpAccessCredential["FtpUser2"] // TODO - revert back to the above (FTP mocked)
                    },
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "FTP_USER_PASS",
                        AccessCredentialName = "Password",
                        // DefaultCredentialValue = "somepass" // TODO - randomize elsewhere
                        DefaultCredentialValue = _tmpFtpAccessCredential["FtpPass2"] // TODO - revert back to the above (FTP mocked)
                    },
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "FTP_USER_HOME",
                        // DefaultCredentialValue = "/home/someuser"
                        DefaultCredentialValue = "/files/" // TODO - revert back to the above (FTP mocked)
                    },
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "FTP_MAX_CLIENTS",
                        DefaultCredentialValue = "50" 
                    },
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "FTP_MAX_CONNECTIONS",
                        DefaultCredentialValue = "50"
                    },
                    new CredentialParameter()
                    {
                        EnvironmentVariableName = "ADDED_FLAGS",
                        DefaultCredentialValue = "-d"
                    },
                    new CredentialParameter()
                    {
                        AccessCredentialName = "Host", // TODO - remove this parameter (FTP mocked)
                        DefaultCredentialValue = _tmpFtpAccessCredential["FtpHost2"]
                    },
                    /* new CredentialParameter()
                    {
                        AccessCredentialName = "Port",
                        DefaultCredentialValue = "21"
                    } */
                },
                Version = "1.2.3",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 12, 08, 17, 13, 45),
                    Description = "Version 1.2.3",
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

        private void AddDecisionModule(List<ComputationModule> Cms)
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
                Status = UnitReleaseStatus.Deprecated,
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "input",
                Uid = "input01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output1",
                Uid = "output01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "output2",
                Uid = "output02",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private void AddNeuralNetLearnerModule(List<ComputationModule> Cms)
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
                Status = UnitReleaseStatus.Deprecated,
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "training",
                Uid = "training01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "trained_network",
                Uid = "trained_network01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
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
                Status = UnitReleaseStatus.Deprecated,
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
            
            cmr.Image = "Cryptic Build file Build = new contents";
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            dp2 = new DeclaredDataPin
            {
                Name = "training",
                Uid = "training01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            dp3 = new DeclaredDataPin
            {
                Name = "trained_network",
                Uid = "trained_network01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private void AddNeuralNetRecognizerModule(List<ComputationModule> Cms)
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
                Status = UnitReleaseStatus.Deprecated,
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
            
            cmr.Image = "Cryptic Build file Build = new contents";
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "input_value_set",
                Uid = "inputvalueset01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "input_classification_set",
                Uid = "inputclassset01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "classification_result",
                Uid = "classification_result01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private void AddMatrixOperationsModule(List<ComputationModule> Cms)
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
                Status = UnitReleaseStatus.Deprecated,
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
            
            cmr.Image = "Cryptic Build file Build = new contents";
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "matrix_operations",
                Uid = "matrix_operations01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output_matrix",
                Uid = "output_matrix01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private void AddRegressionModule(List<ComputationModule> Cms)
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
                Status = UnitReleaseStatus.Deprecated,
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
            
            cmr.Image = "Cryptic Build file Build = new contents";
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "parameters",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output_data",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        // ========================================================================================

        private void AddSpectralModule(List<ComputationModule> Cms)
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
                Status = UnitReleaseStatus.Deprecated,
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
            
            cmr.Image = "Cryptic Build file Build = new contents";
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "parameters",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output_data",
                Uid = Guid.NewGuid().ToString(),
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private void AddFtp2MongoModule(List<ComputationModule> Cms)
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
                Status = UnitReleaseStatus.Deprecated,
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Access = Ats.Find(a => a.Name == "FTP")
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "MongoDBFileWriter",
                Uid = "MongoDBFileWriter01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Multiple,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        private void AddRgb2GrayModule(List<ComputationModule> Cms)
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
                Status = UnitReleaseStatus.Deprecated,
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
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "UncannyParameters",
                Uid = "UncannyParameters01",
                Binding = DataBinding.RequiredWeak,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "JSON" == t.Name),
                Structure = null,
                Access = null // new AccessType(){Name = "JSON", AccessSchema = null}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "ImageWriter",
                Uid = "ImageWriter01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddImageEdgerModule(List<ComputationModule> Cms, string portNo, string suffix)
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
                        Memory = 1024, Storage = 1, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 3, Cpus = 2000, Gpus = 0
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
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "UncannyParameters",
                Uid = "UncannyParameters01",
                Binding = DataBinding.RequiredWeak,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "JSON" == t.Name),
                Structure = null, 
                Access = null // new AccessType(){Name = "JSON", AccessSchema = null}
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "ImageWriter",
                Uid = "ImageWriter" + suffix,
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddMongo2FtpModule(List<ComputationModule> Cms)
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
                Status = UnitReleaseStatus.Deprecated,
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "FTPFileWriter",
                Uid = "FTPFileWriter01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "FTP")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddImageChannelSeparator(List<ComputationModule> Cms)
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
                        Memory = 1024, Storage = 1, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 2, Cpus = 2000, Gpus = 0
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
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "Image channel 1",
                Uid = "Imagechannel1_01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Image channel 2",
                Uid = "Imagechannel2_01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "Image channel 3",
                Uid = "Imagechannel3_01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddImageChannelJoiner(List<ComputationModule> Cms)
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
                        Memory = 1024, Storage = 1, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 2, Cpus = 2000, Gpus = 0
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
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "Image channel 1",
                Uid = "Imagechannel1_02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Image channel 2",
                Uid = "Imagechannel2_02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "Image channel 3",
                Uid = "Imagechannel3_02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null, 
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddDataCopierModule(List<ComputationModule> Cms)
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
                        Memory = 512, Storage = 1, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 1024, Storage = 2, Cpus = 2000, Gpus = 0
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
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null, 
                Access = null
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "output",
                Uid = "output001",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Unspecified,
                DataMultiplicity = CMultiplicity.Unspecified,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null, 
                Access = null
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }
        
        private void AddFaceRecogniserModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Face Recogniser",
                Uid = "face_recogniser_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Recognises faces in photo files.",
                    ShortDescription = "Recognises faces",
                    Icon = "https://www.balticlsc.eu/model/_icons/fcr_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "face_recogniser_rel_001",
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
                        Memory = 1024, Storage = 2, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 3, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Image = "k4liber/face_recognition:latest";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "Input",
                Uid = "input002",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Output",
                Uid = "output002",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }
        
        private void AddFaceDetectorModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Face Detector",
                Uid = "face_detector_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Recognises faces in photo files.",
                    ShortDescription = "Recognises faces",
                    Icon = "https://www.balticlsc.eu/model/_icons/fcr_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "face_detector_rel_001",
                Version = "1.0",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2021, 3, 29, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 1024, Storage = 2, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 3, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Image = "gawienczuka/facedetection";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "Input",
                Uid = "fd_input001",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Output",
                Uid = "fd_output001",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }
        
        private void AddFaceContoursModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Face Points",
                Uid = "face_contours_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Recognises face contours in photo files.",
                    ShortDescription = "Recognises face contours",
                    Icon = "https://www.balticlsc.eu/model/_icons/fcr_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "face_contours_rel_001",
                Version = "1.0",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2021, 3, 29, 10, 00, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 1024, Storage = 2, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 3, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Image = "robertinio223/pointfindingmodule";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "Input",
                Uid = "fd_input001",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Output",
                Uid = "fd_output001",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }
        
        private void AddMoodRecogniserModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Mood Recogniser",
                Uid = "mood_recogniser_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Recognises moods based on face points",
                    ShortDescription = "Recognises moods",
                    Icon = "https://www.balticlsc.eu/model/_icons/fcr_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "mood_recogniser_rel_001",
                Version = "1.0",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2021, 5, 12, 10, 48, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 1024, Storage = 2, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 3, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Image = "sanjeeta/emotiondetection";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "Input",
                Uid = "mr_input001",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Output",
                Uid = "mr_output001",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }
        
        private void AddDataSummarizerModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Data Summarizer",
                Uid = "data_summarizer__001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Sends two files to outputs",
                    ShortDescription = "Sends two files",
                    Icon = "https://www.balticlsc.eu/model/_icons/fcr_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "file_summarizer_rel_001",
                Version = "1.0",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2021, 06, 10, 11, 55, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 1024, Storage = 2, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 3, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Image = "gawienczuka/datasummarizer";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "Input",
                Uid = "ds_input001",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "Output",
                Uid = "ds_output001",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Output2",
                Uid = "ds_output002",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }
        
        private void AddImageClassificationTrainerModule(List<ComputationModule> Cms)
        {
            // #### Computation Module ##################

            ComputationModule cm = new ComputationModule
            {
                Name = "Image Classification Trainer",
                Uid = "image_class_trainer_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "A neural network model trainer for classification of images. " +
                                      "Requires some input data for the process of learning and produces its neural network model to the output. " + 
                                      "Output model can be used for various predictions, depending on input image data type.",
                    ShortDescription = "A neural network model trainer for classification of images.",
                    Icon = "https://www.balticlsc.eu/model/_icons/ict_001.png",
                    Keywords = new List<string>(){"image classification trainer", "artificial intelligence", "neural network"}
                },
                AuthorUid = "user1"
            };
            Cms.Add(cm);

            // #### Computation Module Releases ##########

            ComputationModuleRelease cmr = new ComputationModuleRelease
            {
                Uid = "image_class_trainer_rel_001",
                Version = "1.0",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2021, 5, 12, 11, 03, 00),
                    Description = "The initial version",
                    IsOpenSource = false,
                },
                Status = UnitReleaseStatus.Approved,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 1024, Storage = 2, Cpus = 1000, Gpus = 0
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 2048, Storage = 3, Cpus = 2000, Gpus = 0
                    }
                }
            };
            
            cmr.Image = "michalpw159/image_classification_trainer:latest";
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "SYS_APP_PORT", 
                DefaultValue = "80",
                Type = UnitParamType.Port,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            cmr.Parameters.Add(new UnitParameter(){
                NameOrPath = "APP_ENVIRONMENT_FILENAME_EXPRESSION", 
                DefaultValue = "BalticLSC-.{6}-{FILENAME}",
                Type = UnitParamType.Variable,
                IsMandatory = true,
                Uid = Guid.NewGuid().ToString()
            });
            
            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "Images",
                Uid = "ict_input001",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "FTP" == ac.Name)
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "Metadata",
                Uid = "ict_input002",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "JSON" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "FTP" == ac.Name)
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "TrainedModel",
                Uid = "ict_output001",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = Ats.Find(ac => "FTP" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);
        }

        //=========================================================================================

         private void AddSimpleImageProcessor(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "Simple Image Processor",
                Uid = "SimpleImageProcessor_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Simple image processor that edges out photos.",
                    ShortDescription = "Edges out photos.",
                    Icon = "https://www.balticlsc.eu/model/_icons/yap_001.png"
                },
                AuthorUid = "user1"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "SimpleImageProcessor_rel_001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the processor",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "InputImpages",
                Uid = "f8596c3b-9a99-4c7c-ae27-db455d184941",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = null // new AccessType(){Name = "FTP Folder Name", AccessSchema = null}
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "OutputImages",
                Uid = "757c6eee-41f1-4c10-8b76-1c27b417a162",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = null // new AccessType(){Name = "FTP Server", AccessSchema = null}
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp03);
        }

        //=========================================================================================

        private void AddFaceRecognizer(List<ComputationApplication> Cas)
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
                DiagramUid = _tmpApp1DiagramUid //"74fe91c6-a2e7-4eba-9f3f-545703dc75ec"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "FaceRecognizer_rel_001",
                Version = "0.1",
                DiagramUid = _tmpRel1DiagramUid, // "949a3ce7-c76d-4992-9b1d-c34ff2e04b40",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the recognizer",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Approved
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "Input Photos",
                Uid = _tmpRel1InputPinUid, //"4268565f-212a-46ee-a8a0-61f105615d4e",
                Binding = DataBinding.RequiredStrong,
                DataMultiplicity = CMultiplicity.Multiple,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => "FTP" == a.Name)
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "Output Photos",
                Uid = _tmpRel1OutputPinUid, // "194cc803-d28b-4eaa-ba72-8eea7cb2b098",
                Binding = DataBinding.Provided,
                DataMultiplicity = CMultiplicity.Multiple,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => "FTP" == a.Name)
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
        }

        //=========================================================================================
        
        private void AddMoodRecognizer(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "MoodRecognizer",
                Uid = "MoodRecognizer_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Mood Recognition Application used to recognize face moods in the crowd",
                    ShortDescription = "Recognizes face moods in the crowd",
                    Icon = "https://www.balticlsc.eu/model/_icons/fcr_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = _tmpApp3DiagramUid //"74fe91c6-a2e7-4eba-9f3f-545703dc75ec"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "MoodRecognizer_rel_001",
                Version = "0.1",
                DiagramUid = _tmpRel3DiagramUid, // "949a3ce7-c76d-4992-9b1d-c34ff2e04b40",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2021, 03, 4, 12, 0, 0),
                    Description = "First version of the mood recognizer",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Approved
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "Input",
                Uid = _tmpRel3InputPinUid, //"4268565f-212a-46ee-a8a0-61f105615d4e",
                Binding = DataBinding.RequiredStrong,
                DataMultiplicity = CMultiplicity.Multiple,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => "FTP" == a.Name)
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "Output",
                Uid = _tmpRel3OutputPinUid, // "194cc803-d28b-4eaa-ba72-8eea7cb2b098",
                Binding = DataBinding.Provided,
                DataMultiplicity = CMultiplicity.Multiple,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => "FTP" == a.Name)
            };
            
            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "Output2",
                Uid = _tmpRel3Output2PinUid, // "194cc803-d28b-4eaa-ba72-8eea7cb2b098",
                Binding = DataBinding.Provided,
                DataMultiplicity = CMultiplicity.Multiple,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => "FTP" == a.Name)
            };
            
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);
        }

        //=========================================================================================

        private void AddHullOptimizer(List<ComputationApplication> Cas)
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
                AuthorUid = "user1"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "HullOptimizer_rel_001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the optimizer",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "HullInput",
                Uid = "HullData01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "OptimizationParams",
                Uid = "OptimizationParams01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "Hull",
                Uid = "Hull01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);
        }

        //=========================================================================================

        private void AddWildlifeRecognizer(List<ComputationApplication> Cas)
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
                AuthorUid = "user1"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "WildlifeRecognizer_rel_001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the recognizer",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "AnimalPhoto",
                Uid = "AnimalPhoto01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "RecognitionParams",
                Uid = "RecognitionParams02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "AnimalRecord",
                Uid = "AnimalRecord01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);
        }

        //=========================================================================================

        private void AddCovid2Analyzer(List<ComputationApplication> Cas)
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
                AuthorUid = "user1"
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "Covid2Analyzer_rel_001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the analyzer",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "GeographicDistribution",
                Uid = "GeographicDistribution01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "AnalysisParams",
                Uid = "AnalysisParams02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "AnalysisResults",
                Uid = "AnalysisResults01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "DataFile" == t.Name),
                Structure = null,
                Access = null
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);
        }

        private void AddMarekImageProcessor(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "Greying Image Processor",
                Uid = "MarekImageProcessor_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Processes images by splitting into RGB and greying.",
                    ShortDescription = "Greys color images.",
                    Icon = "https://www.balticlsc.eu/model/_icons/yap_001.png"
                },
                AuthorUid = "user1",
                // DiagramUid = "2b17d401-440b-4e78-acf3-2f055a6182a0",
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "MarekImageProcessor_rel_001",
                Version = "0.1",
                // DiagramUid = "fc9d3cb3-5aa0-4d2c-8e17-bf1acccbfdea",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 07, 17, 12, 0, 0),
                    Description = "First version of the processor",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "InputImages",
                Uid = "2727963b-d335-43b6-8698-4088abb7d16d",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => "FTP" == a.Name)
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "OutputImages",
                Uid = "85f80054-720e-4aee-b404-fcecc87fa95b",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => "FTP" == a.Name)
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp03);
        }
        
        private void AddMarekImageProcessor2(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "Edging Image Processor",
                Uid = "MarekImageProcessor2_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Processes images by splitting into RGB and edging.",
                    ShortDescription = "Edges color images.",
                    Icon = "https://www.balticlsc.eu/model/_icons/yap_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = _tmpApp2DiagramUid // "7fc58d9f-387c-4be2-8e71-99c54e5d1134",
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "MarekImageProcessor2_rel_001",
                Version = "0.1",
                DiagramUid = _tmpRel2DiagramUid, // "54b8e49f-2b07-4ab2-91e7-2fe6637a8ec7",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 07, 17, 12, 0, 0),
                    Description = "First version of the processor",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Approved
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "InputImages",
                Uid = _tmpRel2InputPinUid, // "a5301f9e-305a-4fab-beba-bfa1ba7ab0ee",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => "FTP" == a.Name)
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "OutputImages",
                Uid = _tmpRel2OutputPinUid, // "6a6b05da-cfae-4047-90cb-c711c71205f0",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Structure = null,
                Access = Ats.Find(a => "FTP" == a.Name)
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp03);
        }
        
         private void AddImageClassificationTrainer(List<ComputationApplication> Cas)
        {
            // #### Computation Application ######################################

            ComputationApplication capp = new ComputationApplication
            {
                Name = "Image Classification Trainer",
                Uid = "ImageClassTrainer_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "A neural network model trainer for classification of images. " + 
                                      "Requires some input data for the process of learning and produces its neural network model to the output. " + 
                                      "Output model can be used for various predictions, depending on input image data type. ",
                    ShortDescription = "A neural network model trainer for classification of images.",
                    Icon = "https://www.balticlsc.eu/model/_icons/ict_001.png"
                },
                AuthorUid = "user1",
                DiagramUid = _tmpApp4DiagramUid // "7fc58d9f-387c-4be2-8e71-99c54e5d1134",
            };
            Cas.Add(capp);

            // #### Computation Application Releases + Declared Pins ##############################

            ComputationApplicationRelease app = new ComputationApplicationRelease
            {
                Uid = "ImageClassTrainer_rel_001",
                Version = "0.1",
                DiagramUid = _tmpRel4DiagramUid, // "54b8e49f-2b07-4ab2-91e7-2fe6637a8ec7",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2021, 05, 13, 15, 16, 0),
                    Description = "First version of the classifier",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Approved
            };
            app.Unit = capp;
            capp.Releases.Add(app);

            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "Images",
                Uid = _tmpRel4Input1PinUid, // "a5301f9e-305a-4fab-beba-bfa1ba7ab0ee",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(d => d.Name == "ImageFile"),
                Structure = null,
                Access = Ats.Find(a => a.Name == "FTP")
            };
            
            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "Metadata",
                Uid = _tmpRel4Input2PinUid, // "a5301f9e-305a-4fab-beba-bfa1ba7ab0ee",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = Dts.Find(d => d.Name == "JSON"),
                Structure = null,
                Access = Ats.Find(a => a.Name == "FTP")
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "TrainedModel",
                Uid = _tmpRel4OutputPinUid, // "6a6b05da-cfae-4047-90cb-c711c71205f0",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(d => d.Name == "DataFile"),
                Structure = null,
                Access = Ats.Find(a => a.Name == "FTP")
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);
        }
    }
}