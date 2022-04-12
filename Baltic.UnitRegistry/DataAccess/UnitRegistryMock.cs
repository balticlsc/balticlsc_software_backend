using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    public class UnitRegistryMock
    {
        public List<ComputationApplicationRelease> Cars = new List<ComputationApplicationRelease>();
        public List<ComputationApplication> Cas = new List<ComputationApplication>();
        public List<ComputationModuleRelease> Cmrs = new List<ComputationModuleRelease>();
        public List<ComputationModule> Cms = new List<ComputationModule>();

        public IDictionary<string, List<string>> Toolbox = new ConcurrentDictionary<string, List<string>>();
        public IDictionary<string, List<string>> AppShelf = new ConcurrentDictionary<string, List<string>>();

        public IDictionary<string, List<TaskDataSet>> DataShelf = new ConcurrentDictionary<string, List<TaskDataSet>>();
        
        public List<DataType> Dts = new List<DataType>();
        public List<DataStructure> Dss = new List<DataStructure>();
        public List<AccessType> Ats = new List<AccessType>();

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

        public UnitRegistryMock(IConfiguration configuration)
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
            
            AddDataTypes();
            AddDataStructures();
            AddServices();
            AddAccessTypes();
            AddModules();
            AddApplications();
            AddDataSets();
        }
        
        private void AddServices()
        {
            AddMongoDbService();
            AddFtpService();
        }

        private void AddModules()
        {
            AddFaceRecogniserModule(); // module by Jan Bielecki
            
            AddImageChannelSeparator(); // module by Marek Wdowiak
            AddImageChannelJoiner(); // module by Marek Wdowiak
            AddImageEdgerModule("9104", "01"); // module by Marek Wdowiak
            AddImageEdgerModule("9107", "02");
            AddImageEdgerModule("9108", "03");
            
            AddFaceDetectorModule(); // module by Adam Gawieńczuk
            AddFaceContoursModule(); // module by Robert Mazurek
            AddMoodRecogniserModule(); // module by Jena Smruti
            AddDataSummarizerModule(); // module by Adam Gawieńczuk

            AddImageClassificationTrainerModule(); // module by Michał Pawlikowski

            // "Fake" modules
            AddDecisionModule();
            AddNeuralNetLearnerModule();
            AddNeuralNetRecognizerModule();
            AddMatrixOperationsModule();
            AddRegressionModule();
            AddSpectralModule();
            
            // Deprecated modules
            AddRgb2GrayModule();
            AddMongo2FtpModule();
            AddFtp2MongoModule();
            AddCopyOutModule();
            
            // System modules
            AddDataCopierModule();
            
            AddModulesToToolbox();
        }

        private void AddApplications()
        {
            AddFaceRecognizer();
            AddMoodRecognizer();
            AddMarekImageProcessor2();
            AddImageClassificationTrainer();
            AddHullOptimizer();
            AddWildlifeRecognizer();
            AddSimpleImageProcessor();
            AddCovid2Analyzer();
            AddMarekImageProcessor();
            AddYetAnotherImageProcessor();
        }

        // ========================================================================================

        private void AddDataSets()
        {
            // -------- User: user1 -------------------
            
            List<TaskDataSet> tds1 = new List<TaskDataSet>();

            TaskDataSet ds = GetNewEdgerDataSet("MyFirstFilm", "MyFirstFilm_001", "VideoFile", "films/film1.avi", 
                "user1", "", CMultiplicity.Single);
            tds1.Add(ds);
            ds = GetNewEdgerDataSet("MySecondFilm", "MySecondFilm_001", "VideoFile", "films/film2.mov", 
                "user1", "", CMultiplicity.Single);
            tds1.Add(ds);
            ds = GetNewEdgerDataSet("InputImages1", "InputImages_001", "ImageFile", "source1", 
                "user1", "", CMultiplicity.Multiple);
            tds1.Add(ds);
            ds = GetNewEdgerDataSet("OutputImages1", "OutputImages1_001", "ImageFile", "target1", 
                "user1", "", CMultiplicity.Multiple);
            tds1.Add(ds);
            ds = GetNewEdgerDataSet("OutputImages2", "OutputImages2_001", "ImageFile", "source2", 
                "user1", "", CMultiplicity.Multiple);
            tds1.Add(ds);
            ds = GetNewEdgerDataSet("OutputImages3", "OutputImages3_001", "ImageFile", "target2", 
                "user1", "", CMultiplicity.Multiple);
            tds1.Add(ds);
            ds = GetNewFaceDataSet("FaceInput1", "FaceInput1_001", "ImageFile", "source1", 
                "user1", "", CMultiplicity.Multiple, true);
            tds1.Add(ds);
            ds = GetNewFaceDataSet("FaceOutput1", "FaceOutput1_001", "ImageFile", "target1", 
                "user1", "", CMultiplicity.Multiple, true);
            tds1.Add(ds);
            ds = GetNewFaceDataSet("FaceInput2", "FaceInput2_001", "ImageFile", "source2", 
                "user1", "2", CMultiplicity.Multiple, true);
            tds1.Add(ds);
            ds = GetNewFaceDataSet("FaceOutput2", "FaceOutput2_001", "ImageFile", "target2", 
                "user1", "2", CMultiplicity.Multiple, true);
            tds1.Add(ds);
            
            ds = GetNewFaceDataSet("FaceInput3", "FaceInput3_001", "ImageFile", "source3", 
                "user1", "", CMultiplicity.Multiple, true);
            tds1.Add(ds);
            
            ds = GetNewFaceDataSet("FaceOutput3", "FaceOutput3_001", "ImageFile", "target3", 
                "user1", "", CMultiplicity.Multiple, true);
            tds1.Add(ds);
            
            ds = GetNewFaceDataSet("FaceInput4", "FaceInput4_001", "JSON", "metadata.json", 
                "user1", "", CMultiplicity.Single, true);
            tds1.Add(ds);
            ds = GetNewFaceDataSet("FaceOutput4", "FaceInput3_001", "DataFile", "model.xml", 
                "user1", "", CMultiplicity.Single, true);
            tds1.Add(ds);

            // -------- User: demo -----------------
            
            List<TaskDataSet> tds2 = new List<TaskDataSet>();
            
            ds = GetNewEdgerDataSet("EdgerDemoInput1", "DefaultDataSet_001", "ImageFile", "source1", 
                "demo", "", CMultiplicity.Multiple);
            tds2.Add(ds);
            ds = GetNewEdgerDataSet("EdgerDemoOutput1", "DefaultDataSet_002", "ImageFile", "target1", 
                "demo", "", CMultiplicity.Multiple);
            tds2.Add(ds);
            ds = GetNewEdgerDataSet("EdgerDemoInput2", "DefaultDataSet_003", "ImageFile", "source2", 
                "demo", "", CMultiplicity.Multiple);
            tds2.Add(ds);
            ds = GetNewEdgerDataSet("EdgerDemoOutput2", "DefaultDataSet_004", "ImageFile", "target2", 
                "demo", "", CMultiplicity.Multiple);
            tds2.Add(ds);
            
            ds = GetNewFaceDataSet("FaceDemoInput1", "DefaultDataSet_005", "ImageFile", "source1", 
                "demo", "", CMultiplicity.Multiple, true);
            tds2.Add(ds);
            ds = GetNewFaceDataSet("FaceDemoOutput1", "DefaultDataSet_006", "ImageFile", "target1", 
                "demo", "", CMultiplicity.Multiple, true);
            tds2.Add(ds);
            ds = GetNewFaceDataSet("FaceDemoInput2", "DefaultDataSet_007", "ImageFile", "source2", 
                "demo", "", CMultiplicity.Multiple, true);
            tds2.Add(ds);
            ds = GetNewFaceDataSet("FaceDemoOutput2", "DefaultDataSet_008", "ImageFile", "source1/windows.jpg", 
                "demo", "", CMultiplicity.Single, true);
            tds2.Add(ds);
            
            // "{\n  \"connectionstring\" : \"ftp://K4liber:jfcXKuL8cYeTFzS@ftp.drivehq.com\"," +
            //"\n  \"dir\" : \"/baltic_test/input_folder\"\n}"
           
            DataShelf.Add("user1", tds1);
            DataShelf.Add("demo", tds2);
        }
        
        private TaskDataSet GetNewEdgerDataSet(string name, string uid, string type, string folder, string user, string postfix, 
            CMultiplicity mult)
        {
            return new TaskDataSet()
            {
                Name = name,
                Uid = uid,
                Type = Dts.Find(dt => type == dt.Name),
                Structure = null,
                Access = Ats.Find(at => "FTP" == at.Name),
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
            string postfix, CMultiplicity mult, bool newVersion)
        {
            if (newVersion)
                return new TaskDataSet()
                {
                    Name = name,
                    Uid = uid,
                    Type = Dts.Find(dt => type == dt.Name),
                    Structure = null,
                    Access = Ats.Find(at => "FTP" == at.Name),
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
            
            return new TaskDataSet()
            {
                Name = name,
                Uid = uid,
                Type = Dts.Find(dt => type == dt.Name),
                Structure = null,
                Access = Ats.Find(at => "ftp" == at.Name),
                Data = new CDataSet()
                {
                    Values =
                        $"{{\n  \"connectionstring\" : \"ftp://{_tmpFtpAccessCredential["FtpUser" + postfix]}:" +
                        $"{_tmpFtpAccessCredential["FtpPass" + postfix]}@{_tmpFtpAccessCredential["FtpHost" + postfix]}\"," +
                        "\n  \"dir\" : \"/files/recogniser/" + folder + "\"\n}"
                },
                OwnerUid = user,
                Multiplicity = mult
            };
        }
        
        //=========================================================================================
        
        private void AddDataTypes()
        {
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
        }

        private void AddDataStructures()
        {
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
        }

        private void AddAccessTypes()
        {
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
            at = new AccessType()
            {
                Name = "ftp",
                Version = "1.0",
                Uid = "dd-010-000",
                IsBuiltIn = true,
                AccessSchema = "{\n\"Host\" : \"string\"," +
                               "\n\"Port\" : \"string\"," +
                               "\n\"User\" : \"string\"," +
                               "\n\"Password\" : \"string\"\n}",
                PathSchema = "{\n\"ResourcePath\" : \"string\" }",
                StorageUid = _ftpRelease.Uid
            };
            Ats.Add(at);
        }
        
        //=========================================================================================
        
        private void AddMongoDbService()
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
            Cms.Add(cm);

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

            Cmrs.Add(cmr);
            _mongoDbRelease = cmr;
        }
        
        //=========================================================================================
        
        private void AddFtpService()
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
            Cms.Add(cm);

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

            Cmrs.Add(cmr);
            _ftpRelease = cmr;
        }

        //=========================================================================================

        private void AddDecisionModule()
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

            Cmrs.Add(cmr);
        }

        // ========================================================================================

        private void AddNeuralNetLearnerModule()
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

            Cmrs.Add(cmr);

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

            Cmrs.Add(cmr);
        }

        // ========================================================================================

        private void AddNeuralNetRecognizerModule()
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

            Cmrs.Add(cmr);
        }

        // ========================================================================================

        private void AddMatrixOperationsModule()
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

            Cmrs.Add(cmr);
        }

        // ========================================================================================

        private void AddRegressionModule()
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

            Cmrs.Add(cmr);
        }

        // ========================================================================================

        private void AddSpectralModule()
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

            Cmrs.Add(cmr);
        }

        private void AddFtp2MongoModule()
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
                Type = new DataType() {Name = "ftp"},
                Access = Ats.Find(a => a.Name == "FTP")
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "MongoDBFileWriter",
                Uid = "MongoDBFileWriter01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Multiple,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
        }

        private void AddRgb2GrayModule()
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
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
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
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddImageEdgerModule(string portNo, string suffix)
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
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
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
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddMongo2FtpModule()
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
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "FTPFileWriter",
                Uid = "FTPFileWriter01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "ftp"},
                Access = Ats.Find(a => a.Name == "FTP")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddImageChannelSeparator()
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
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "Image channel 1",
                Uid = "Imagechannel1_01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Image channel 2",
                Uid = "Imagechannel2_01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "Image channel 3",
                Uid = "Imagechannel3_01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddImageChannelJoiner()
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
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "Image channel 1",
                Uid = "Imagechannel1_02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Image channel 2",
                Uid = "Imagechannel2_02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            DeclaredDataPin dp4 = new DeclaredDataPin
            {
                Name = "Image channel 3",
                Uid = "Imagechannel3_02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "mongo"},
                Access = Ats.Find(a => a.Name == "MongoDB")
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.DeclaredPins.Add(dp4);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
            cmr.RequiredServiceUids.Add(_mongoDbRelease.Uid);
        }

        private void AddDataCopierModule()
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

            Cmrs.Add(cmr);
        }
        
        private void AddFaceRecogniserModule()
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
                DataMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Access = Ats.Find(ac => "FTP" == ac.Name)
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Output",
                Uid = "output002",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Access = Ats.Find(ac => "FTP" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
        }
        
        private void AddFaceDetectorModule()
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
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
        }
        
        private void AddFaceContoursModule()
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
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
        }
        
        private void AddMoodRecogniserModule()
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
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
        }
        
        private void AddDataSummarizerModule()
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
                Access = Ats.Find(ac => "MongoDB" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
        }
        
        private void AddImageClassificationTrainerModule()
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
                Access = Ats.Find(ac => "FTP" == ac.Name)
            };
            cmr.DeclaredPins.Add(dp1);
            cmr.DeclaredPins.Add(dp2);
            cmr.DeclaredPins.Add(dp3);
            cmr.Unit = cm;
            cm.Releases.Add(cmr);

            Cmrs.Add(cmr);
        }

        private void AddCopyOutModule()
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

            Cmrs.Add(cmr);
        }

        private void AddModulesToToolbox()
        {
            if (!Toolbox.ContainsKey("user1"))
                Toolbox.Add("user1", new List<string>());
            // Toolbox["user1"].Add("ftp2mongodb_rel_001");
            Toolbox["user1"].Add("grey2edge_rel_001");
            Toolbox["user1"].Add("grey2edge_rel_002");
            Toolbox["user1"].Add("grey2edge_rel_003");
            // Toolbox["user1"].Add("rgb2gray-mongo_rel_001");
            // Toolbox["user1"].Add("mongodb2ftp_rel_001");
            Toolbox["user1"].Add("imagechannelseparator_rel_001");
            Toolbox["user1"].Add("imagechanneljoiner_rel_001");
            // Toolbox["user1"].Add("fs001");
            // Toolbox["user1"].Add("is001");
            // Toolbox["user1"].Add("ip002");
            // Toolbox["user1"].Add("im003");
            Toolbox["user1"].Add("face_recogniser_rel_001");
        }

        //=========================================================================================

        private void AddSimpleImageProcessor()
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
                Type = new DataType() {Name = "ftp"},
                Access = null // new AccessType(){Name = "FTP Folder Name", AccessSchema = null}
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "OutputImages",
                Uid = "757c6eee-41f1-4c10-8b76-1c27b417a162",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "ftp"},
                Access = null // new AccessType(){Name = "FTP Server", AccessSchema = null}
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp03);

            Cars.Add(app);
        }

        private void AddYetAnotherImageProcessor()
        {
            // #### Computation Modules ##################

            ComputationModule cmo00 = new ComputationModule
            {
                Name = "Frame Splitter",
                Uid = Guid.NewGuid().ToString(),
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Splits films into sequences of frames (snapshots)",
                    ShortDescription = "Films to frames",
                    Icon = "https://www.balticlsc.eu/model/_icons/fs_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cmo00);
            ComputationModule cmo0 = new ComputationModule
            {
                Name = "Image Splitter",
                Uid = Guid.NewGuid().ToString(),
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Splits images into sequences of tiles",
                    ShortDescription = "Images to tiles",
                    Icon = "https://www.balticlsc.eu/model/_icons/is_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cmo0);
            ComputationModule cmo1 = new ComputationModule
            {
                Name = "Image Processor",
                Uid = Guid.NewGuid().ToString(),
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Processes an image in a very specific way",
                    ShortDescription = "Processes an image",
                    Icon = "https://www.balticlsc.eu/model/_icons/ip_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cmo1);
            ComputationModule cmo2 = new ComputationModule
            {
                Name = "Image Merger",
                Uid = Guid.NewGuid().ToString(),
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Merges sequences of tiles into frames (snapshots) and then into films",
                    ShortDescription = "Tiles to films",
                    Icon = "https://www.balticlsc.eu/model/_icons/im_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cmo2);
            ComputationModule cmm = new ComputationModule
            {
                Name = "Image Splitter 2",
                Uid = Guid.NewGuid().ToString(),
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Splits tiles into frames (snapshots)",
                    ShortDescription = "Tiles to frames",
                    Icon = "https://www.balticlsc.eu/model/_icons/is_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cmm);
            ComputationModule cmo4 = new ComputationModule
            {
                Name = "New Image Processor",
                Uid = "nip_12345",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Processes films...",
                    ShortDescription = "Processes films...",
                    Icon = "https://www.balticlsc.eu/model/_icons/ip_001.png"
                },
                AuthorUid = "user1"
            };
            Cms.Add(cmo4);

            // #### Computation Unit Releases ##########

            ComputationApplicationRelease app;

            //--------------------------------------------
            ComputationModuleRelease cm00 = new ComputationModuleRelease
            {
                Uid = "fs001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 08, 13, 30, 00),
                    Description = "First version of the splitter",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 10, Storage = 10, Cpus = 1, Gpus = 2
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 100, Storage = 100, Cpus = 10, Gpus = 10
                    }
                }
            };
            cm00.Image = "fs001";
            DeclaredDataPin mdp10 = new DeclaredDataPin
            {
                Name = "film",
                Uid = "film01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "MPEG4"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin mdp20 = new DeclaredDataPin
            {
                Name = "filmf1",
                Uid = "filmf01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            cm00.DeclaredPins.Add(mdp10);
            cm00.DeclaredPins.Add(mdp20);
            cm00.Unit = cmo00;
            cmo00.Releases.Add(cm00);

            Cmrs.Add(cm00);

            //---------------------------------------------
            ComputationModuleRelease cm0 = new ComputationModuleRelease
            {
                Uid = "is001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 1, 12, 10, 15, 0),
                    Description = "First version of the splitter",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 10, Storage = 20, Cpus = 1, Gpus = 1
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 100, Storage = 200, Cpus = 10, Gpus = 20
                    }
                }
            };
            cm0.Image = "is001";

            DeclaredDataPin mdp01 = new DeclaredDataPin
            {
                Name = "image",
                Uid = "image01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin mdp02 = new DeclaredDataPin
            {
                Name = "imagep1",
                Uid = "imagep01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin mdp03 = new DeclaredDataPin
            {
                Name = "imagep2",
                Uid = "imagep02",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin mdp04 = new DeclaredDataPin
            {
                Name = "imagep3",
                Uid = "imagep03",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            cm0.DeclaredPins.Add(mdp01);
            cm0.DeclaredPins.Add(mdp02);
            cm0.DeclaredPins.Add(mdp03);
            cm0.DeclaredPins.Add(mdp04);
            cm0.Unit = cmo0;
            cmo0.Releases.Add(cm0);

            Cmrs.Add(cm0);

            //--------------------------------------------
            ComputationModuleRelease cm1 = new ComputationModuleRelease
            {
                Uid = "ip002",
                IsMultitasking = true,
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 1, 12, 10, 15, 0),
                    Description = "First version of the processor",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 10, Storage = 20, Cpus = 1, Gpus = 1
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 100, Storage = 200, Cpus = 10, Gpus = 20
                    }
                }
            };
            cm1.Image = "ip002";
            DeclaredDataPin mdp11 = new DeclaredDataPin
            {
                Name = "imagep",
                Uid = "imagep00",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin mdp12 = new DeclaredDataPin
            {
                Name = "imagepp",
                Uid = "imagepp00",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            cm1.DeclaredPins.Add(mdp11);
            cm1.DeclaredPins.Add(mdp12);
            cm1.Unit = cmo1;
            cmo1.Releases.Add(cm1);


            Cmrs.Add(cm1);

            //--------------------------------------------
            ComputationModuleRelease cm2 = new ComputationModuleRelease
            {
                Uid = "im003",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 1, 12, 10, 15, 0),
                    Description = "First version of the merger",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated,
                SupportedResourcesRange = new ResourceReservationRange()
                {
                    MinReservation = new ResourceReservation()
                    {
                        Memory = 10, Storage = 20, Cpus = 1, Gpus = 1
                    },
                    MaxReservation = new ResourceReservation()
                    {
                        Memory = 100, Storage = 200, Cpus = 10, Gpus = 20
                    }
                }
            };
            cm2.Image = "im003";
            DeclaredDataPin mdp21 = new DeclaredDataPin
            {
                Name = "imagerp1",
                Uid = "imagerp01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin mdp22 = new DeclaredDataPin
            {
                Name = "imagerp2",
                Uid = "imagerp02",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin mdp23 = new DeclaredDataPin
            {
                Name = "imagerp3",
                Uid = "imagerp03",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "any_picture"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            DeclaredDataPin mdp24 = new DeclaredDataPin
            {
                Name = "fimage",
                Uid = "fimage00",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "MPEG4"},
                Structure = new DataStructure() {DataSchema = "JSONSchema contents"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema contents"}
            };
            cm2.DeclaredPins.Add(mdp21);
            cm2.DeclaredPins.Add(mdp22);
            cm2.DeclaredPins.Add(mdp23);
            cm2.DeclaredPins.Add(mdp24);
            cm2.Unit = cmo2;
            cmo2.Releases.Add(cm2);

            Cmrs.Add(cm2);

            ComputationModuleRelease cm = new ComputationModuleRelease
            {
                Uid = "is002",
                Version = "0.1",
                Status = UnitReleaseStatus.Deprecated
            };
            cm.Image = "is002";
            DeclaredDataPin mdp1 = new DeclaredDataPin
            {
                Name = "image", Uid = "image01", Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Multiple,
                Access = new AccessType() {AccessSchema = "file"}
            };
            DeclaredDataPin mdp2 = new DeclaredDataPin
            {
                Name = "image_no", Uid = "image_no01", Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Access = new AccessType() {AccessSchema = "file"}
            };
            DeclaredDataPin mdp3 = new DeclaredDataPin
            {
                Name = "stream", Uid = "stream_no01", Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Access = new AccessType() {AccessSchema = "file"}
            };
            cm.DeclaredPins.Add(mdp1);
            cm.DeclaredPins.Add(mdp2);
            cm.DeclaredPins.Add(mdp3);
            cm.Unit = cmm;
            cmm.Releases.Add(cm);

            Cmrs.Add(cm);

            // #### Computation Applications ######################################

            //----------------------------------------------
            ComputationApplication capp = new ComputationApplication
            {
                Name = "YetAnotherImageProcessor",
                Uid = "YetAnotherImageProcessor_001",
                Releases = new List<ComputationUnitRelease>(),
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Yet Another Image Processor by Michal Smialek and Kamil Rybinski",
                    ShortDescription = "Yet Another Image Processor",
                    Icon = "https://www.balticlsc.eu/model/_icons/yap_001.png"
                },
                AuthorUid = "user1"
            };
            Cas.Add(capp);

            ComputationApplication capp2 = new ComputationApplication
            {
                Name = "My Reworked Film Processor",
                Uid = "MyApp_001",
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Not yet finished processor based on a previous one",
                    ShortDescription = "FIlm processor.",
                    Icon = "https://www.balticlsc.eu/model/_icons/im_001.png"
                },
                AuthorUid = "user1"
            };
            Cas.Add(capp2);
            ComputationApplication capp3 = new ComputationApplication
            {
                Name = "Dev Spectral Analysis App",
                Uid = "DevSpecApp_001",
                Descriptor = new UnitDescriptor()
                {
                    LongDescription = "Currently developed Spectral Analysis App made from several ready modules.",
                    ShortDescription = "Spectral analyser app.",
                    Icon = "https://www.balticlsc.eu/model/_icons/spa_001.png"
                },
                AuthorUid = "user1"
            };
            Cas.Add(capp3);

            // #### Computation Application Releases ##############################

            app = new ComputationApplicationRelease
            {
                Uid = "YetAnotherImageProcessor_rel_001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the processor.",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated
            };
            app.Unit = capp;
            capp.Releases.Add(app);
            DeclaredDataPin dp01 = new DeclaredDataPin
            {
                Name = "Film",
                Uid = "d6234567-1234-1234-1234-1234567890ab", // "Film01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "MPEG4"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };
            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "Proc_Film",
                Uid = "06234567-1234-1234-1234-1234567890ab", // "Proc_Film01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "MPEG4"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            
            app = new ComputationApplicationRelease
            {
                Uid = "DevSpecApp_rel_001",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2019, 12, 4, 12, 0, 0),
                    Description = "First version of the analyser.",
                    IsOpenSource = false
                },
                Status = UnitReleaseStatus.Deprecated
            };
            app.Unit = capp3;
            capp3.Releases.Add(app);
            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "Film",
                Uid = "d6234567-1234-1234-1234-1234567890ac", // "Film01",
                Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "MPEG4"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };
            DeclaredDataPin dp04 = new DeclaredDataPin
            {
                Name = "Proc_Film",
                Uid = "06234567-1234-1234-1234-1234567890ac", // "Proc_Film01",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                Type = new DataType() {Name = "MPEG4"},
                Access = new AccessType() {Name = "file", AccessSchema = "JSONSchema content"}
            };
            app.DeclaredPins.Add(dp03);
            app.DeclaredPins.Add(dp04);

            //--------------------------
            UnitCall uc00 = new UnitCall
            {
                Name = "frame splitter",
                Strength = UnitStrength.Strong,
                Unit = cm00
            };
            ComputedDataPin cdp10 = new ComputedDataPin
            {
                Uid = "film_1212",
                Declared = mdp10
            };
            ComputedDataPin cdp20 = new ComputedDataPin
            {
                Uid = "filmf1_1212",
                Declared = mdp20
            };

            uc00.Pins.Add(cdp10);
            uc00.Pins.Add(cdp20);
            // app.Calls.Add(uc00);

            //--------------------------
            UnitCall uc0 = new UnitCall
            {
                Name = "splitter",
                Strength = UnitStrength.Strong,
                Unit = cm0
            };
            ComputedDataPin cdp01 = new ComputedDataPin
            {
                Uid = "image02",
                Declared = mdp01
            };
            ComputedDataPin cdp02 = new ComputedDataPin
            {
                Uid = "image_no021",
                Declared = mdp02
            };
            ComputedDataPin cdp03 = new ComputedDataPin
            {
                Uid = "image_no022",
                Declared = mdp03
            };
            ComputedDataPin cdp04 = new ComputedDataPin
            {
                Uid = "image_no023",
                Declared = mdp04
            };
            uc0.Pins.Add(cdp01);
            cdp01.Call = uc0;
            uc0.Pins.Add(cdp02);
            cdp02.Call = uc0;
            uc0.Pins.Add(cdp03);
            cdp03.Call = uc0;
            uc0.Pins.Add(cdp04);
            cdp04.Call = uc0;
            // app.Calls.Add(uc0);

            //--------------------------
            UnitCall uc1 = new UnitCall
            {
                Name = "processor1",
                Strength = UnitStrength.Strong,
                Unit = cm1
            };
            ComputedDataPin cdp11 = new ComputedDataPin
            {
                Uid = "image021",
                Declared = mdp11
            };
            ComputedDataPin cdp12 = new ComputedDataPin
            {
                Uid = "image_nop021",
                Declared = mdp12
            };
            uc1.Pins.Add(cdp11);
            cdp11.Call = uc1;
            uc1.Pins.Add(cdp12);
            cdp12.Call = uc1;
            // app.Calls.Add(uc1);

            //--------------------------
            UnitCall uc2 = new UnitCall
            {
                Name = "processor2",
                Strength = UnitStrength.Strong,
                Unit = cm1
            };
            ComputedDataPin cdp21 = new ComputedDataPin
            {
                Uid = "image031",
                Declared = mdp11
            };
            ComputedDataPin cdp22 = new ComputedDataPin
            {
                Uid = "image_nop031",
                Declared = mdp12
            };
            uc2.Pins.Add(cdp21);
            cdp21.Call = uc2;
            uc2.Pins.Add(cdp22);
            cdp22.Call = uc2;
            // app.Calls.Add(uc2);

            //--------------------------
            UnitCall uc3 = new UnitCall
            {
                Name = "processor3",
                Strength = UnitStrength.Strong,
                Unit = cm1
            };
            ComputedDataPin cdp31 = new ComputedDataPin
            {
                Uid = "image041",
                Declared = mdp11
            };
            ComputedDataPin cdp32 = new ComputedDataPin
            {
                Uid = "image_nop041",
                Declared = mdp12
            };
            uc3.Pins.Add(cdp31);
            cdp31.Call = uc3;
            uc3.Pins.Add(cdp32);
            cdp32.Call = uc3;
            // app.Calls.Add(uc3);

            //--------------------------
            UnitCall uc4 = new UnitCall
            {
                Name = "merger",
                Strength = UnitStrength.Strong,
                Unit = cm2
            };
            ComputedDataPin cdp41 = new ComputedDataPin
            {
                Uid = "imager021",
                Declared = mdp21
            };
            ComputedDataPin cdp42 = new ComputedDataPin
            {
                Uid = "imager022",
                Declared = mdp22
            };
            ComputedDataPin cdp43 = new ComputedDataPin
            {
                Uid = "imagr023",
                Declared = mdp23
            };
            ComputedDataPin cdp44 = new ComputedDataPin
            {
                Uid = "fimage02",
                Declared = mdp24
            };

            uc4.Pins.Add(cdp41);
            cdp41.Call = uc4;
            uc4.Pins.Add(cdp42);
            cdp42.Call = uc4;
            uc4.Pins.Add(cdp43);
            cdp43.Call = uc4;
            uc4.Pins.Add(cdp44);
            cdp44.Call = uc4;

            //----------------------------
            PinGroup pg1 = new PinGroup
            {
                Name = "images"
            };
            pg1.Depths.Add(0);
            pg1.Depths.Add(1);
            cdp41.Group = pg1;
            cdp42.Group = pg1;
            cdp43.Group = pg1;

            // app.Calls.Add(uc4);

            //--------------------------------------------------
            DataFlow df00 = new DataFlow
            {
                Source = dp01,
                Target = cdp10
            };
            dp01.Outgoing = df00;
            cdp10.Incoming = df00;

            DataFlow df01 = new DataFlow
            {
                Source = cdp20,
                Target = cdp01
            };
            cdp20.Outgoing = df01;
            cdp01.Incoming = df01;

            DataFlow df02 = new DataFlow
            {
                Source = cdp02,
                Target = cdp11
            };
            cdp02.Outgoing = df02;
            cdp11.Incoming = df02;
            DataFlow df03 = new DataFlow
            {
                Source = cdp03,
                Target = cdp21
            };
            cdp03.Outgoing = df03;
            cdp21.Incoming = df03;
            DataFlow df04 = new DataFlow
            {
                Source = cdp04,
                Target = cdp31
            };
            cdp04.Outgoing = df04;
            cdp31.Incoming = df04;

            DataFlow df05 = new DataFlow
            {
                Source = cdp12,
                Target = cdp41
            };
            cdp12.Outgoing = df05;
            cdp41.Incoming = df05;
            DataFlow df06 = new DataFlow
            {
                Source = cdp22,
                Target = cdp42
            };
            cdp22.Outgoing = df06;
            cdp42.Incoming = df06;
            DataFlow df07 = new DataFlow
            {
                Source = cdp32,
                Target = cdp43
            };
            cdp32.Outgoing = df07;
            cdp43.Incoming = df07;

            DataFlow df08 = new DataFlow
            {
                Source = cdp44,
                Target = dp02
            };
            cdp44.Outgoing = df08;
            dp02.Incoming = df08;

            // app.Flows.Add(df01);
            // app.Flows.Add(df02);
            // app.Flows.Add(df03);
            // app.Flows.Add(df04);
            // app.Flows.Add(df05);
            // app.Flows.Add(df06);
            // app.Flows.Add(df07);
            // app.Flows.Add(df08);

            Cars.Add(app);

            //##################################################################

            app = new ComputationApplicationRelease
            {
                Uid = "ala_r13y6",
                Version = "0.1",
                Descriptor = new ReleaseDescriptor()
                {
                    Date = new DateTime(2020, 4, 7, 15, 35, 2),
                    Description = "Very interesting release",
                    IsOpenSource = true
                },
                Status = UnitReleaseStatus.Deprecated
            };
            app.Unit = capp2;
            capp2.Releases.Add(app);

            DeclaredDataPin dp1 = new DeclaredDataPin
            {
                Name = "Image", Uid = "Image01", Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Multiple
            };
            DeclaredDataPin dp2 = new DeclaredDataPin
            {
                Name = "Proc_Image", Uid = "Proc_Image01", Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single
            };
            DeclaredDataPin dp3 = new DeclaredDataPin
            {
                Name = "Stream", Uid = "Stream01", Binding = DataBinding.RequiredStrong,
                TokenMultiplicity = CMultiplicity.Single
            };
            app.DeclaredPins.Add(dp1);
            app.DeclaredPins.Add(dp2);
            app.DeclaredPins.Add(dp3);

            UnitCall uc = new UnitCall
            {
                Name = "splitter", Strength = UnitStrength.Strong, Unit = cm
            };
            // app.Calls.Add(uc);

            ComputedDataPin cdp1 = new ComputedDataPin
            {
                Uid = "image02", Declared = mdp1
            };
            ComputedDataPin cdp2 = new ComputedDataPin
            {
                Uid = "image_no02", Declared = mdp2
            };
            ComputedDataPin cdp3 = new ComputedDataPin
            {
                Uid = "stream_no02", Declared = mdp3
            };
            uc.Pins.Add(cdp1);
            cdp1.Call = uc;
            uc.Pins.Add(cdp2);
            cdp2.Call = uc;
            uc.Pins.Add(cdp3);
            cdp3.Call = uc;

            DataFlow df1 = new DataFlow
            {
                Source = dp1, Target = cdp1
            };
            DataFlow df2 = new DataFlow
            {
                Source = cdp2, Target = dp2
            };
            DataFlow df3 = new DataFlow
            {
                Source = dp3, Target = cdp3
            };
            // app.Flows.Add(df1);
            // app.Flows.Add(df2);
            // app.Flows.Add(df3);

            dp1.Outgoing = df1;
            dp2.Incoming = df2;
            dp3.Outgoing = df3;
            cdp1.Incoming = df1;
            cdp2.Outgoing = df2;
            cdp3.Incoming = df3;

            Cars.Add(app);
        }

        //=========================================================================================

        private void AddFaceRecognizer()
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
                // Structure = Dss.Find(d => "ConnectionString" == d.Name),
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Access = Ats.Find(a => "FTP" == a.Name)
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "Output Photos",
                Uid = _tmpRel1OutputPinUid, // "194cc803-d28b-4eaa-ba72-8eea7cb2b098",
                Binding = DataBinding.Provided,
                DataMultiplicity = CMultiplicity.Multiple,
                TokenMultiplicity = CMultiplicity.Single,
                // Structure = Dss.Find(d => "ConnectionString" == d.Name),
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Access = Ats.Find(a => "FTP" == a.Name)
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);

            Cars.Add(app);
        }

        //=========================================================================================
        
        private void AddMoodRecognizer()
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
                // Structure = Dss.Find(d => "ConnectionString" == d.Name),
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Access = Ats.Find(a => "FTP" == a.Name)
            };

            DeclaredDataPin dp02 = new DeclaredDataPin
            {
                Name = "Output",
                Uid = _tmpRel3OutputPinUid, // "194cc803-d28b-4eaa-ba72-8eea7cb2b098",
                Binding = DataBinding.Provided,
                DataMultiplicity = CMultiplicity.Multiple,
                TokenMultiplicity = CMultiplicity.Single,
                // Structure = Dss.Find(d => "ConnectionString" == d.Name),
                Type = Dts.Find(t => "ImageFile" == t.Name),
                Access = Ats.Find(a => "FTP" == a.Name)
            };
            
            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "Output2",
                Uid = _tmpRel3Output2PinUid, // "194cc803-d28b-4eaa-ba72-8eea7cb2b098",
                Binding = DataBinding.Provided,
                DataMultiplicity = CMultiplicity.Multiple,
                TokenMultiplicity = CMultiplicity.Single,
                // Structure = Dss.Find(d => "ConnectionString" == d.Name),
                Type = Dts.Find(t => "DataFile" == t.Name),
                Access = Ats.Find(a => "FTP" == a.Name)
            };
            
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);

            Cars.Add(app);
        }

        //=========================================================================================

        private void AddHullOptimizer()
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

            Cars.Add(app);
        }

        //=========================================================================================

        private void AddWildlifeRecognizer()
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

            Cars.Add(app);
        }

        //=========================================================================================

        private void AddCovid2Analyzer()
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

            Cars.Add(app);
        }

        private void AddMarekImageProcessor()
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

            Cars.Add(app);
        }
        
        private void AddMarekImageProcessor2()
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
                Type = new DataType() {Name = "ftp"},
                Access = Ats.Find(a => a.Name == "FTP")
            };

            DeclaredDataPin dp03 = new DeclaredDataPin
            {
                Name = "OutputImages",
                Uid = _tmpRel2OutputPinUid, // "6a6b05da-cfae-4047-90cb-c711c71205f0",
                Binding = DataBinding.Provided,
                TokenMultiplicity = CMultiplicity.Single,
                DataMultiplicity = CMultiplicity.Multiple,
                Type = new DataType() {Name = "ftp"},
                Access = Ats.Find(a => a.Name == "FTP")
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp03);

            Cars.Add(app);
        }
        
         private void AddImageClassificationTrainer()
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
                Access = Ats.Find(a => a.Name == "FTP")
            };
            app.DeclaredPins.Add(dp01);
            app.DeclaredPins.Add(dp02);
            app.DeclaredPins.Add(dp03);

            Cars.Add(app);
        }
    }
}