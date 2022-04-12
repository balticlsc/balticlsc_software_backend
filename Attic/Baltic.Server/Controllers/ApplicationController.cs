using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Baltic.Server.Controllers.Models;
using ComputationApplication = Baltic.Server.Models.App.ComputationApplication;
using ComputationApplicationRelease = Baltic.Server.Models.App.ComputationApplicationRelease;


namespace Baltic.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("app")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        /// <summary>
        /// [GetShelfApps] Returns list of computation applications from user's application shelf.
        /// </summary>
        /// <returns>Computation applications from user's shelf as an array.</returns>
        [HttpGet]
        [Route("shelf")]
        public IEnumerable< ComputationApplication> GetShelfApps()
        {
            return
                
            #region Mock List<ComputationApplication>
            
            new List<ComputationApplication>()
            {
                new ComputationApplication()
                {
                    Id = 3140,
                    AuthorFullName = "John Walker",
                    AuthorId = 1311,
                    ForkId = 1080,
                    FullDescription =
                        "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                    Icon = "pi_computer.png",
                    Keywords = new List<string>() {"pi", "computing", "high", "accuracy"},
                    LastUpdateDate = new DateTime(2019, 3, 14),
                    Name = "Pi Computer",
                    OpenSource = true,
                    Rate = 10,
                    Releases = new List<Server.Models.App.ComputationApplicationRelease>()
                    {
                        new ComputationApplicationRelease()
                        {
                            Id = 4141,
                            OpenSource = false,
                            FullDescription =
                                "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                            AuthorId = 1311,
                            AssetId = 1786,
                            AuthorName = "John Walker",
                            Available = false,
                            ComputationApplicationId = 3140,
                            ParentId = 5745,
                            Private = true,
                            ReleaseDate = new DateTime(2018, 3, 14),
                            UsageCounter = 4646644,
                            Version = "v1.0"
                        },
                        new ComputationApplicationRelease()
                        {
                            Id = 3842,
                            OpenSource = true,
                            FullDescription =
                                "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                            AuthorId = 1311,
                            AssetId = 1789,
                            AuthorName = "John Walker",
                            Available = true,
                            ComputationApplicationId = 3140,
                            ParentId = 5745,
                            Private = false,
                            ReleaseDate = new DateTime(2018, 6, 14),
                            UsageCounter = 464600000,
                            Version = "v2.0"
                        }
                    },
                    ShortDescription = "Pi Computing app",
                    TimesUsed = 6000
                },

                
                new ComputationApplication()
                {
                    Id = 1111,
                    AuthorFullName = "Caitlyn Ross",
                    AuthorId = 9876,
                    ForkId = 4444,
                    FullDescription =
                        "Material usage computing application that counts optimal number of composite materials for building plane frame",
                    Icon = "material_computer.png",
                    Keywords = new List<string>() {"plane", "computing", "composite", "material"},
                    LastUpdateDate = new DateTime(2017, 4, 24),
                    Name = "Compositer",
                    OpenSource = false,
                    Rate = 7,
                    Releases = new List<Server.Models.App.ComputationApplicationRelease>()
                    {
                        new ComputationApplicationRelease()
                        {
                            Id = 8450,
                            OpenSource = false,
                            FullDescription =
                                "Material usage computing application that counts optimal number of composite materials for building plane frame",
                            AuthorId = 1311,
                            AssetId = 1506,
                            AuthorName = "Caitlyn Ross",
                            Available = true,
                            ComputationApplicationId = 1111,
                            ParentId = 1225,
                            Private = true,
                            ReleaseDate = new DateTime(2018, 3, 14),
                            UsageCounter = 12343454,
                            Version = "v1.1"
                        },
                        new ComputationApplicationRelease()
                        {
                            Id = 6735,
                            OpenSource = false,
                            FullDescription =
                                "Material usage computing application that counts optimal number of composite materials for building plane frame",
                            AuthorId = 1311,
                            AssetId = 1555,
                            AuthorName = "Caitlyn Ross",
                            Available = true,
                            ComputationApplicationId = 1111,
                            ParentId = 5333,
                            Private = true,
                            ReleaseDate = new DateTime(2018, 6, 14),
                            UsageCounter = 16755,
                            Version = "v1.2"
                        }
                    },
                    ShortDescription = "Composite Material Computing app",
                    TimesUsed = 1200
                }
            };
            

            #endregion Mock List<ComputationApplication>
        }

        /// <summary>
        /// [FindApps] Return list of computation applications consistent with given application list request.
        /// </summary>
        /// <param name="request">Request dicribing criteria for computation applications.</param>
        /// <returns>Computation applications consistent with request as an array.</returns>
        [HttpGet]
        [Route("list")]
        public IEnumerable<ComputationApplication> FindApps()
        {
            return

                #region Mock  List<ComputationApplication>

                new List<ComputationApplication>()
                {
                    new ComputationApplication()
                    {
                        Id = 1,
                        ForkId = 1,
                        Name = "Ship trunk speed counter",
                        ShortDescription = "Max speed of ship predictor.",
                        FullDescription = "Application which predicts possible max speed of ship.",
                        Rate = 3,
                        AuthorId = 1,
                        AuthorFullName = "John Brown",
                        Icon = "",
                        Keywords = new List<string>() {"ship", "speed", "trunk"},
                        LastUpdateDate = new DateTime(2010, 7, 3),
                        TimesUsed = 300,
                        OpenSource = false,
                        Releases = new[]
                        {
                            new ComputationApplicationRelease()
                            {
                                Id = 102,
                                OpenSource = false,
                                FullDescription = "Application which predicts possible max speed of ship.",
                                AuthorId = 1,
                                AssetId = 17,
                                AuthorName = "John Brown",
                                Available = true,
                                ComputationApplicationId = 1,
                                ParentId = 57,
                                Private = false,
                                ReleaseDate = new DateTime(2010, 7, 3),
                                UsageCounter = 4,
                                Version = "v1.0"
                            },
                        }
                    },
                    new ComputationApplication()
                    {
                        Id = 4,
                        ForkId = 5,
                        Name = "Economic growth predictor.",
                        ShortDescription = "Economic growth predictor let you predict economic growth in your country.",
                        FullDescription =
                            "Economic growth predictor let you predict economic growth in your country by lots of parameters.",
                        Rate = 4,
                        AuthorId = 4,
                        AuthorFullName = "Postgres Bachamow",
                        Icon = "",
                        Keywords = new List<string>() {"economic", "growth", "count"},
                        LastUpdateDate = new DateTime(2017, 7, 3),
                        TimesUsed = 600,
                        OpenSource = true,
                        Releases = new[]
                        {
                            new ComputationApplicationRelease()
                            {
                                Id = 124,
                                OpenSource = false,
                                FullDescription =
                                    "Economic growth predictor let you predict economic growth in your country.",
                                AuthorId = 4,
                                AssetId = 17,
                                AuthorName = "Postgres Bachamow",
                                Available = true,
                                ComputationApplicationId = 4,
                                ParentId = 7,
                                Private = false,
                                ReleaseDate = new DateTime(2017, 7, 3),
                                UsageCounter = 10,
                                Version = "v1.0"
                            },
                        }
                    },
                    new ComputationApplication()
                    {
                        Id = 8,
                        ForkId = 12,
                        Name = "Trebuchet range.",
                        ShortDescription = "Count possible trebuchet range.",
                        FullDescription =
                            "Application using advanced neural networks to predict trebuchet range and show you how to improve your results.",
                        Rate = 6,
                        AuthorId = 10,
                        AuthorFullName = "Leon Icniv",
                        Icon = "",
                        Keywords = new List<string>() {"trebuchet", "range", "neural networks"},
                        LastUpdateDate = new DateTime(2018, 3, 3),
                        TimesUsed = 100,
                        OpenSource = true,
                        Releases = new[]
                        {
                            new ComputationApplicationRelease()
                            {
                                Id = 114,
                                OpenSource = true,
                                FullDescription =
                                    "Application using advanced neural networks to predict trebuchet range and show you how to improve your results.",
                                AuthorId = 8,
                                AssetId = 15,
                                AuthorName = "Leon Icniv",
                                Available = true,
                                ComputationApplicationId = 8,
                                ParentId = 9,
                                Private = false,
                                ReleaseDate = new DateTime(2018, 8, 3),
                                UsageCounter = 12,
                                Version = "v1.0"
                            },
                        }
                    },
                };

            #endregion Mock  List<ComputationApplication>
        }

        /// <summary>
        /// [GetApp] Return computation applications with given id.
        /// </summary>
        /// <param name="appId">Id of computation application.</param>
        /// <returns>Single computation application with given id.</returns>
        [HttpGet]
        public ComputationApplication GetApp([FromQuery] int appId)
        {
            return

                #region Mock ComputationApplication

                new ComputationApplication()
                {
                    Id = appId,
                    AuthorFullName = "John Walker",
                    AuthorId = 1311,
                    ForkId = 1080,
                    FullDescription =
                        "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                    Icon = "pi_computer.png",
                    Keywords = new List<string>() {"pi", "computing", "high", "accuracy"},
                    LastUpdateDate = new DateTime(2019, 3, 14),
                    Name = "Pi Computer",
                    OpenSource = true,
                    Rate = 10,
                    Releases = new List<ComputationApplicationRelease>()
                    {
                        new ComputationApplicationRelease()
                        {
                            Id = 5673,
                            OpenSource = false,
                            FullDescription =
                                "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                            AuthorId = 1311,
                            AssetId = 1786,
                            AuthorName = "John Walker",
                            Available = false,
                            ComputationApplicationId = appId,
                            ParentId = 5745,
                            Private = true,
                            ReleaseDate = new DateTime(2018, 3, 14),
                            UsageCounter = 4646644,
                            Version = "v1.0"
                        },
                        new ComputationApplicationRelease()
                        {
                            Id = 7777,
                            OpenSource = true,
                            FullDescription =
                                "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                            AuthorId = 1311,
                            AssetId = 1789,
                            AuthorName = "John Walker",
                            Available = true,
                            ComputationApplicationId = appId,
                            ParentId = 5745,
                            Private = false,
                            ReleaseDate = new DateTime(2018, 6, 14),
                            UsageCounter = 464600000,
                            Version = "v2.0"
                        }
                    },
                    ShortDescription = "Pi Computing app",
                    TimesUsed = 6000
                };

            #endregion Mock ComputationApplication
        }

        /// <summary>
        /// [AddAppToShelf] Add computation application with given id to user's application shelf.
        /// </summary>
        /// <param name="appId">Id of computation application.</param>
        /// <returns>Computation application shelf as an array after new position was added.</returns>
        [HttpPost]
        [Route("shelf")]
        public string AddAppToShelf([FromBody]  string appId)
        {
            return  "200";

            #region Mock  List<ComputationApplication>

            //    new List<ComputationApplication>()
            //    {
            //        new ComputationApplication()
            //        {
            //            Id = 120,
            //            ForkId =121 ,
            //            Name = "Hypergraphs",
            //            ShortDescription = "Hypergraphs Based on Pythagorean Fuzzy Soft Model.", 
            //            FullDescription = "  Abstract A Pythagorean fuzzy soft set (PFSS) model is an extension of" +
            //                              " an intuitionistic fuzzy soft set (IFSS) model to deal with vague knowledge according " +
            //                              "to different parameters.",
            //            Rate = 8, AuthorId = 122, AuthorFullName = "Mary Brown", Icon = " ",
            //            Keywords = new List<string>() {"Application"," Computation Application","Hypergraphs","PFSS"},
            //            LastUpdateDate = new DateTime(2019,12,3,13,09,21),
            //            TimesUsed = 123,
            //            OpenSource = true,
            //            Releases = new List<ComputationApplicationRelease>()
            //            {
            //                new ComputationApplicationRelease()
            //                {
            //                    AssetId =124 ,AuthorId =122 ,AuthorName = "Mary Brown" ,Available = true ,ComputationApplicationId =  120,
            //                    FullDescription = "  New Release Abstract A Pythagorean fuzzy soft set (PFSS) model is an extension of" +
            //                                      " an intuitionistic fuzzy soft set (IFSS) model to deal with vague knowledge according " +
            //                                      "to different parameters.",
            //                    Id = 125,
            //                    OpenSource = true ,
            //                    ParentId = 120 ,
            //                    Private =  false, 
            //                },
            //                new ComputationApplicationRelease()
            //                {
            //                AssetId =126,
            //                AuthorId =122,
            //                AuthorName = "Mary Brown",
            //                Available = true,
            //                ComputationApplicationId =  120,
            //                FullDescription = "  New Release Abstract A Pythagorean fuzzy soft set (PFSS) model is an extension of" +
            //                                  " an intuitionistic fuzzy soft set (IFSS) model to deal with vague knowledge according " +
            //                                  "to different parameters.",
            //                Id = 127,
            //                OpenSource = true,
            //                ParentId = 120,
            //                Private =  false,
            //                },
            //                new ComputationApplicationRelease()
            //                {
            //                    AssetId =128,
            //                    AuthorId =122,
            //                    AuthorName = "Mary Brown",
            //                    Available = true,
            //                    ComputationApplicationId =  120,
            //                    FullDescription = "  New Release Abstract A Pythagorean fuzzy soft set (PFSS) model is an extension of" +
            //                                      " an intuitionistic fuzzy soft set (IFSS) model to deal with vague knowledge according " +
            //                                      "to different parameters."
            //                    ,Id = 129,
            //                    OpenSource = true,
            //                    ParentId = 120,
            //                    Private =  false,
            //                }
            //            }
            //        },
            //        new ComputationApplication()
            //        {
            //            Id = 130,
            //            ForkId =131 ,
            //            Name = "Optimizing the Maximal Perturbation", 
            //            ShortDescription = "Optimizing the Maximal Perturbation in Point Sets while Preserving the Order Type",
            //            FullDescription = " Abstract Recently a new kind of fiducial marker based on order type (OT) has been proposed. Using OT one can unequivocally identify a set of points through its triples of point orientation, and therefore, there is no need to use metric information.",
            //            Rate = 7,
            //            AuthorId = 132,
            //            AuthorFullName = "Karen Brooks", 
            //            Icon = " ",
            //            Keywords = new List<string>() {"Application"," Computation Application","Perturbation","Optimizing" },
            //            LastUpdateDate = new DateTime(2019,12,3,13,20,15),
            //            TimesUsed = 133,
            //            OpenSource = true,
            //            Releases = new List<ComputationApplicationRelease>()
            //            {
            //                new ComputationApplicationRelease()
            //                {
            //                    AssetId =134,
            //                    AuthorId =132,
            //                    AuthorName = "Karen Brooks",
            //                    Available = true,
            //                    ComputationApplicationId =130,
            //                    FullDescription =" New release Abstract Recently a new kind of fiducial marker based on order type (OT) has been proposed. Using OT one can unequivocally identify a set of points through its triples of point orientation, and therefore, there is no need to use metric information.",
            //                    Id = 135,
            //                    OpenSource = true,
            //                    ParentId = 130,
            //                    Private =  false,
            //                },
            //                new ComputationApplicationRelease()
            //                {
            //                AssetId =136,
            //                AuthorId =132,
            //                AuthorName = "Karen Brooks",
            //                Available = true,
            //                ComputationApplicationId =130,
            //                FullDescription = " New Relaease Abstract Recently a new kind of fiducial marker based on order type (OT) has been proposed. Using OT one can unequivocally identify a set of points through its triples of point orientation, and therefore, there is no need to use metric information.",
            //                Id = 137,
            //                OpenSource = true,
            //                ParentId = 130,
            //                Private =  false,
            //                },
            //                new ComputationApplicationRelease()
            //                {
            //                    AssetId =128,
            //                    AuthorId =132,
            //                    AuthorName = "Karen Brooks",
            //                    Available = true ,ComputationApplicationId =130,
            //                    FullDescription = "Abstract Recently a new kind of fiducial marker based on order type (OT) has been proposed. Using OT one can unequivocally identify a set of points through its triples of point orientation, and therefore, there is no need to use metric information.",
            //                    Id = 139, OpenSource = true, 
            //                    ParentId = 130, Private =  false,
            //                }
            //            }
            //        },
            //        new ComputationApplication()
            //        {
            //            Id = 140,
            //            ForkId =141,
            //            Name = "Hydraulic Network",
            //            ShortDescription = "Modeling and Simulation of a Hydraulic Network for Leak Diagnosis",
            //            FullDescription = "This app presents the modeling and simulation of a hydraulic network with four nodes and two branches that form a two-level water distribution system. It also proposes a distribution of hydraulic valves that allows emulating a leak using a valve and different network",
            //            Rate = 6, AuthorId = 142, AuthorFullName = "Ruth Evans", Icon = " ",
            //            Keywords = new List<string>() {"Hydraulic","Application"," Computation Application","Simulation", },
            //            LastUpdateDate = new DateTime(2019,12,3,13,23,59),
            //            TimesUsed = 143,
            //            OpenSource = true,
            //            Releases = new List<ComputationApplicationRelease>()
            //            {
            //                new ComputationApplicationRelease()
            //                {
            //                    AssetId =144 ,AuthorId =142 ,AuthorName = "Ruth Evans",Available = true ,ComputationApplicationId =  140,
            //                    FullDescription =" New relaese of application,which presents the modeling and simulation of a hydraulic network with four nodes and two branches that form a two-level water distribution system. It also proposes a distribution of hydraulic valves that allows emulating a leak using a valve and different network",
            //                    Id = 145 , OpenSource = true , ParentId = 140 , Private =  false,
            //                },
            //                new ComputationApplicationRelease()
            //                {
            //                AssetId =146 ,AuthorId =142 ,AuthorName ="Ruth Evans" ,Available = true ,ComputationApplicationId =  140,
            //                FullDescription = " New relaese of application,which presents the modeling and simulation of a hydraulic network with four nodes and two branches that form a two-level water distribution system. It also proposes a distribution of hydraulic valves that allows emulating a leak using a valve and different network",
            //                Id = 147 , OpenSource = true , ParentId = 140 , Private =  false,
            //                },
            //                new ComputationApplicationRelease()
            //                {
            //                    AssetId =148 ,AuthorId =142 ,AuthorName = "Ruth Evans" ,Available = true ,ComputationApplicationId =  140,
            //                    FullDescription =" New relaese of application,which presents the modeling and simulation of a hydraulic network with four nodes and two branches that form a two-level water distribution system. It also proposes a distribution of hydraulic valves that allows emulating a leak using a valve and different network",
            //                    Id = 149 , OpenSource = true , ParentId = 140 , Private =  false,
            //                }
            //            }
            //        }
            //    };

            #endregion Mock  List<ComputationApplication>
        }

        /// <summary>
        /// [RemoveAppFromShelf] Remove computation application with given id from user's application shelf.
         /// </summary>
         /// <param name="appId">Id of computation application.</param> 
        /// <returns>Computation application shelf as an array after one position was deleted.</returns>
        [HttpDelete]
        [Route("shelf")]
        public IEnumerable<ComputationApplication> RemoveAppFromShelf(string appId)
        {
            return new List<ComputationApplication>()
                {
                    new ComputationApplication()
                    {
                        Id = 3140,
                        AuthorFullName = "John Walker",
                        AuthorId = 1311,
                        ForkId = 1080,
                        FullDescription =
                            "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                        Icon = "pi_computer.png",
                        Keywords = new List<string>() {"pi", "computing", "high", "accuracy"},
                        LastUpdateDate = new DateTime(2019, 3, 14),
                        Name = "Pi Computer",
                        OpenSource = true,
                        Rate = 10,
                        Releases = new List<Server.Models.App.ComputationApplicationRelease>()
                        {
                            new ComputationApplicationRelease()
                            {
                                Id = 4141,
                                OpenSource = false,
                                FullDescription =
                                    "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                                AuthorId = 1311,
                                AssetId = 1786,
                                AuthorName = "John Walker",
                                Available = false,
                                ComputationApplicationId = 3140,
                                ParentId = 5745,
                                Private = true,
                                ReleaseDate = new DateTime(2018, 3, 14),
                                UsageCounter = 4646644,
                                Version = "v1.0"
                            },
                            new ComputationApplicationRelease()
                            {
                                Id = 3842,
                                OpenSource = true,
                                FullDescription =
                                    "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                                AuthorId = 1311,
                                AssetId = 1789,
                                AuthorName = "John Walker",
                                Available = true,
                                ComputationApplicationId = 3140,
                                ParentId = 5745,
                                Private = false,
                                ReleaseDate = new DateTime(2018, 6, 14),
                                UsageCounter = 464600000,
                                Version = "v2.0"
                            }
                        },
                        ShortDescription = "Pi Computing app",
                        TimesUsed = 6000
                    },


                    new ComputationApplication()
                    {
                        Id = 1111,
                        AuthorFullName = "Caitlyn Ross",
                        AuthorId = 9876,
                        ForkId = 4444,
                        FullDescription =
                            "Material usage computing application that counts optimal number of composite materials for building plane frame",
                        Icon = "material_computer.png",
                        Keywords = new List<string>() {"plane", "computing", "composite", "material"},
                        LastUpdateDate = new DateTime(2017, 4, 24),
                        Name = "Compositer",
                        OpenSource = false,
                        Rate = 7,
                        Releases = new List<Server.Models.App.ComputationApplicationRelease>()
                        {
                            new ComputationApplicationRelease()
                            {
                                Id = 8450,
                                OpenSource = false,
                                FullDescription =
                                    "Material usage computing application that counts optimal number of composite materials for building plane frame",
                                AuthorId = 1311,
                                AssetId = 1506,
                                AuthorName = "Caitlyn Ross",
                                Available = true,
                                ComputationApplicationId = 1111,
                                ParentId = 1225,
                                Private = true,
                                ReleaseDate = new DateTime(2018, 3, 14),
                                UsageCounter = 12343454,
                                Version = "v1.1"
                            },
                            new ComputationApplicationRelease()
                            {
                                Id = 6735,
                                OpenSource = false,
                                FullDescription =
                                    "Material usage computing application that counts optimal number of composite materials for building plane frame",
                                AuthorId = 1311,
                                AssetId = 1555,
                                AuthorName = "Caitlyn Ross",
                                Available = true,
                                ComputationApplicationId = 1111,
                                ParentId = 5333,
                                Private = true,
                                ReleaseDate = new DateTime(2018, 6, 14),
                                UsageCounter = 16755,
                                Version = "v1.2"
                            }
                        },
                        ShortDescription = "Composite Material Computing app",
                        TimesUsed = 1200
                    }
                };

            //    #region Mock  List<ComputationApplication>

            //    new List<Baltic.Server.Controllers.Models.ComputationApplication>()
            //    {
            //        new Baltic.Server.Controllers.Models.ComputationApplication()
            //        {
            //            AuthorId = "AuthorI11", Description = "Description11", ForkId = "ForkId11", Id = appId,
            //            Name = "Name11", Rate = 11
            //        },
            //        new Baltic.Server.Controllers.Models.ComputationApplication()
            //        {
            //            AuthorId = "AuthorI12", Description = "Description12", ForkId = "ForkId12", Id = appId,
            //            Name = "Name12", Rate = 12
            //        },
            //    };

            //#endregion Mock  List<ComputationApplication>
        }

        /// <summary>
        /// [RateApp] Set user's rating for computation application with given id.
        /// </summary>
        /// <param name="appId">Id of computation application.</param>
        /// <param name="rate">New rate for computation application.</param>
        /// <returns>Single computation application with new rating.</returns>
        [HttpPut]
        [Route("rate")]
        public ComputationApplication RateApp(string appId, int rate)
        {
            return new ComputationApplication()
            {
                Id = 3140,
                AuthorFullName = "John Walker",
                AuthorId = 1311,
                ForkId = 1080,
                FullDescription =
                            "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                Icon = "pi_computer.png",
                Keywords = new List<string>() { "pi", "computing", "high", "accuracy" },
                LastUpdateDate = new DateTime(2019, 3, 14),
                Name = "Pi Computer",
                OpenSource = true,
                Rate = 10,
                Releases = new List<Server.Models.App.ComputationApplicationRelease>()
                        {
                            new ComputationApplicationRelease()
                            {
                                Id = 4141,
                                OpenSource = false,
                                FullDescription =
                                    "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                                AuthorId = 1311,
                                AssetId = 1786,
                                AuthorName = "John Walker",
                                Available = false,
                                ComputationApplicationId = 3140,
                                ParentId = 5745,
                                Private = true,
                                ReleaseDate = new DateTime(2018, 3, 14),
                                UsageCounter = 4646644,
                                Version = "v1.0"
                            },
                            new ComputationApplicationRelease()
                            {
                                Id = 3842,
                                OpenSource = true,
                                FullDescription =
                                    "Pi computing application that uses advantage numeric methods to compute pi number with high accuracy",
                                AuthorId = 1311,
                                AssetId = 1789,
                                AuthorName = "John Walker",
                                Available = true,
                                ComputationApplicationId = 3140,
                                ParentId = 5745,
                                Private = false,
                                ReleaseDate = new DateTime(2018, 6, 14),
                                UsageCounter = 464600000,
                                Version = "v2.0"
                            }
                        },
                ShortDescription = "Pi Computing app",
                TimesUsed = 6000
            };

            #region Mock ComputationApplication

            //new Baltic.Server.Controllers.Models.ComputationApplication()
            //    {
            //        AuthorId = "AuthorI13", Description = "Description13", ForkId = "ForkId13", Id = appId,
            //        Name = "Name13", Rate = rate
            //    };

            #endregion Mock ComputationApplication
        }

        /// <summary>
        /// [PrepareComputationApplicationContextDefinition] Fetch list of parameter definitions required to run release of given computation application.
        /// </summary>
        /// <param name="appReleaseId">Id of computation application release.</param>
        /// <returns>Form definition as a JSON.</returns>
        [HttpGet]
        [Route("context")]
        public string
            PrepareComputationApplicationContextDefinition(
                string appReleaseId) // do obsługi tylko na froncie JSON wytyczne do budowy
            // formularza tak samo jak w interaction
        {
            return

                #region Mock string

                appReleaseId;

            #endregion Mock string
        }

        /// <summary>
        /// [GetRequiredDataSourceDefinitions] Fetch list of data source definitions required to run release of given computation application.
        /// </summary>
        /// <param name="appReleaseId">Id of computation application release.</param>
        /// <returns>Data source definitions as JSON.</returns>
        [HttpGet]
        [Route("datasources")]
        public string GetRequiredDataSourceDefinitions(string appReleaseId)
        {
            return

                #region Mock string

                appReleaseId;

            #endregion Mock string
        }

        /// <summary>
        /// [RunApp] Create new task for given computation application using provided environment data.
        /// </summary>
        /// <param name="appReleaseId">Id of computation application release.</param>
        /// <param name="environment">Informations about evaluation, parameters and data sources needed to run a compuatation application.</param>
        /// <returns>Id of computation task as a string.</returns>
        [HttpPost]
        [Route("run")]
        public string
            RunApp(string appReleaseId,
                ComputationApplicationEnvironment environment) // -> daje nam taska i pobiera dane z formularza
        {
            return

                #region Mock string

                appReleaseId;

            #endregion Mock string
        }

        /// <summary>
        /// [RunApp] Create new request for computation application using provided data.
        /// </summary>
        /// <param name="createApplicationRequest"></param>
        /// <returns>Id of created computation aplication request as a string.</returns>
        [HttpPost]
        [Route("request")]
        public string CreateRequestForApplication(ApplicationRequest createApplicationRequest)
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// [GetApplicationRequestList] Return list of computation application requests consistent with given application request list request.
        /// </summary>
        /// <parameter name="request">Request dicribing criteria for computation application requests.</parameter>
        /// <returns>Array of application requests consistent with request.</returns>
        [HttpGet]
        [Route("requestlist")]
        public IEnumerable<ApplicationRequest> GetApplicationRequestList(ApplicationRequestListRequest request)
        {
            return

                #region Mock List<ApplicationRequest>

                new List<ApplicationRequest>()
                {
                    new ApplicationRequest
                    {
                        ComputationApplicationId = " ComputationApplicationId0", Description = " Description0",
                        Id = "Id0", Name = "Name0"
                    },
                    new ApplicationRequest
                    {
                        ComputationApplicationId = " ComputationApplicationId1", Description = " Description1",
                        Id = "Id1", Name = "Name1"
                    },
                    new ApplicationRequest
                    {
                        ComputationApplicationId = " ComputationApplicationId2", Description = " Description2",
                        Id = "Id2", Name = "Name2"
                    },
                    new ApplicationRequest
                    {
                        ComputationApplicationId = " ComputationApplicationId3", Description = " Description3",
                        Id = "Id3", Name = "Name3"
                    }
                };

            #endregion Mock List<ApplicationRequest>
        }

        /// <summary>
        /// [GetApplicationRequest] Return computation application request with given id.
        /// </summary>
        /// <param name="requestId">Id of computation application request.</param>
        /// <returns>Single application request with given id.</returns>
        [HttpGet]
        [Route("request")]
        public ApplicationRequest GetApplicationRequest(String requestId)
        {
            return

                #region Mock ApplicationRequest

                new ApplicationRequest()
                {
                    ComputationApplicationId = " ComputationApplicationId4", Description = " Description4",
                    Id = requestId, Name = "Name4"
                };

            #endregion Mock ApplicationRequest
        }
    }
}