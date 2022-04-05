using System;
using System.Collections.Generic;
using Baltic.CommonServices;
using Baltic.Core.Utils;
using Baltic.Security.Auth;
using Baltic.Security.Entities;
using Baltic.Security.Tables;
using Baltic.Types.Protos;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baltic.Security.Controllers
{
    // public class NamespaceConstraint : ActionMethodSelectorAttribute
    // {
    //     public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
    //     {
    //         var dataTokenNamespace = (string)routeContext.RouteData.DataTokens.FirstOrDefault(dt => dt.Key == "Namespace").Value;
    //         var actionNamespace = ((ControllerActionDescriptor)action).MethodInfo.DeclaringType.FullName;
    //
    //         Console.WriteLine(actionNamespace);
    //         Console.WriteLine(dataTokenNamespace);
    //         return dataTokenNamespace == actionNamespace;
    //     }
    // }    
    
    [ApiController]
    [Route("[controller]")]
    //[ApiVersion("2.0")]    
    public class TestController : Controller
    {
        private readonly NodeManager _nodeDictionary;

        public TestController(NodeManager nodeDictionary)
        {
            _nodeDictionary = nodeDictionary;
        }
        
        [HttpGet("GetNodesInfo")]
        public IActionResult GetNodesInfo()
        {
            var nodes = new List<Object>(); 
            foreach (var node in _nodeDictionary)
            {
                var client = _nodeDictionary.GetClient<NodeServiceApi.NodeServiceApiClient>(node.Key);
                var status = client.GetNodeStatus(new Empty());
                nodes.Add(new
                {    
                    status.Id,
                    status.Name,
                    status.OsNameAndVersion,
                    status.UpTime
                });
            }

            return Ok(nodes);
        }
        
        [HttpGet]
        public ActionResult<string> Index()
        {
            return "To było łatwe, tu wolno zajrzeć każdemu";
        }

        [HttpGet("CheckAuth")]
        [Authorize]
        public ActionResult<string> CheckAuth()
        {
            return "Brawo - znasz magiczny klucz !!!";
        }
        
        [HttpGet("CheckAdminRole")]        
        [Authorize(Policy = PoliciesLookup.RequireAdmin)]
        public ActionResult<string> CheckAdminRole()
        {
            return "Drugi poziom zdobyty - zapewne masz supermoce";
        }
       
        [HttpGet("AddUser")]
        public ActionResult<string> AddUser()
        {
            var usersTable = new UsersTable();

            var inserted = usersTable.Insert(new 
            {
                Name = "rroszczyk",
                Password = "t34f6t34tg234vg42"
            });
            
            var ue = new UserEntity();
            DBMapper.Map<UserEntity>(inserted, ue);
            Console.WriteLine($">>{ue.Id}");

            return $"{inserted.id}";
        }
    }
}