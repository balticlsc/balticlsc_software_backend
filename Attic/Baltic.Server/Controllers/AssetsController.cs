using System;
using Baltic.Server.Controllers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Baltic.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("assets")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        /// <summary>
        /// [GetAsset] Return asset with given id.
        /// </summary>
        /// <param name="assetId">Id of asset.</param>
        /// <returns>Single asset with given id.</returns>
        [HttpGet]
        public Asset GetAsset(string assetId)
        {
            return new Asset();
        }

        /// <summary>
        /// [CreateAsset] Upload a given asset.
        /// </summary>
        /// <param name="asset">Asset to upload.</param>
        /// <returns>Id of created asset.</returns>
        [HttpPost]
        public string CreateAsset(Asset asset)
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// [UpdateAsset] Replace asset with given id.
        /// </summary>
        /// <param name="assetId">Id of asset.</param>
        /// <param name="asset">New version of asset.</param>
        [HttpPut]
        public void UpdateAsset(string assetId, Asset asset)
        {

        }

        /// <summary>
        /// [DeleteAsset] Remove asset with given id.
        /// </summary>
        /// <param name="assetId">Id of asset.</param>
        [HttpDelete]
        public void DeleteAsset(string assetId)
        {

        }
    }
}