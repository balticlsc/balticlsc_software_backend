using System;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.Node.BatchManager.Models;
using Baltic.Node.Engine.BatchManager;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Baltic.Node.BatchManager.Controllers {
	[ApiController]
	public class TokensController : Controller{

		private ITokens _tokens;

		public TokensController(ITokens tokens)
		{
			_tokens = tokens;
		}

		// TODO introduce try-catch!!!
		
		/// 
		/// <param name="tm"></param>
		[HttpPost("token")]
		public IActionResult PutTokenMessage([FromBody] XOutputTokenMessage tm)
		{
			TokenMessage token = new TokenMessage()
			{
				PinName = tm.PinName,
				SenderUid = tm.SenderUid,
				DataSet = new CDataSet()
				{
					Values = tm.Values
				}
			};
			var result = _tokens.PutTokenMessage(token, tm.BaseMsgUid, tm.IsFinal);
			if (0 == result)
				return HandleSuccess();
			switch (result)
			{
				case -1:
					return HandleError("Incorrect Sender Uid: " + tm.SenderUid);
				case -2:
					return HandleError("Incorrect Required Message Uid");
				case -3:
					return HandleError("Incorrect Pin name");
				case -4:
					return HandleError("Internal node error");
				// TODO handle errors reported by TaskProcessor
				default: return HandleError("PutTokenMessage failed (code " + (result-4) + ")");
			}
		}

		/// 
		/// <param name="ack"></param>
		[HttpPost("ack")]
		public IActionResult AckTokenMessages([FromBody] XTokensAck ack)
		{
			short result = _tokens.AckTokenMessages(ack.MsgUids, ack.SenderUid, ack.IsFinal, ack.IsFailed, ack.Note);
			if (0 == result)
				return HandleSuccess();
			switch (result)
			{
				case -1:
					return HandleError("Incorrect Sender Uid: " + ack.SenderUid);
				case -2:
					return HandleError("Empty Message Uid list");
				case -3:
					return HandleError("Improper Ack message Uid");
				// TODO handle errors reported by TaskProcessor
				default: return HandleError("Failed to Ack message (code " + (result-3) + ")");
			}
		}
		
		private IActionResult HandleError(string message)
		{
			Log.Error("Error: {ex}", message);
			return new JsonResult(new
			{
				Success = false,
				Message = message
			});
		}

		private IActionResult HandleError(Exception ex)
		{
			Log.Debug(ex.ToString());
			return HandleError(ex.Message);
		}

		private IActionResult HandleSuccess(object data = null)
		{
			return new JsonResult(new
			{
				Success = true,
				Data = data
			});
		}
	}
}