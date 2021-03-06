///////////////////////////////////////////////////////////
//  TaskManager.cs
//  Implementation of the Class TaskManager
//  Generated by Enterprise Architect
//  Created on:      01-mar-2020 18:40:57
//  Original author: smialek
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using Baltic.DataModel.CAL;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Diagram;
using Baltic.DataModel.Execution;
using Baltic.Engine.TaskProcessor;
using Baltic.Database.TaskRegistry;
using Baltic.Database.UnitRegistry;
using Baltic.Database.Entities;

namespace Baltic.Engine.TaskManager
{
	public class TaskManager : ITaskManager, ICALTranslation {

		private IUnitProcessing UnitRegistry;
		private ITaskManagement TaskRegistry;
		private ITaskProcessor TaskProcessor;
		private IDataModelImplFactory Factory;

		public TaskManager(){

		}

		~TaskManager(){

		}

		public virtual void Dispose(){

		}

		/// 
		/// <param name="ur"></param>
		/// <param name="tr"></param>
		/// <param name="q"></param>
		public void Init(IUnitProcessing ur, ITaskManagement tr, ITaskProcessor q, IDataModelImplFactory dmf){
			UnitRegistry = ur;
			TaskRegistry = tr;
			TaskProcessor = q;
			Factory = dmf;
		
			//*test*
			Console.WriteLine(ConsoleString() + "INIT");
			//*test*
    	
		}
	
		/// 
		/// <param name="releaseUid"></param>
		/// <param name="par"></param>
		public string InitiateTask(string releaseUid, TaskParameters par){
			ComputationApplicationRelease app = (ComputationApplicationRelease) UnitRegistry.GetUnitRelease(releaseUid);
			return InitiateTask(app,par);
		}
	
		///
		/// <param name="appUid"></param>
		/// <param name="par"></param>
		public string InitiateAppTestTask(string appUid, TaskParameters par){
			ComputationApplication app = (ComputationApplication) UnitRegistry.GetUnit(appUid);
			ComputationApplicationRelease appRelease = DiagramUtility.TranslateDiagram(app.Diagram);
			return InitiateTask(appRelease,par);
		}

		/// 
		/// <param name="app"></param>
		/// <param name="par"></param>
		private string InitiateTask(ComputationApplicationRelease app, TaskParameters par){
		
			//*test*
			Console.WriteLine(ConsoleString() + "TaskManager InitiateTask START: " + app.Uid);
			//*test*
		
			CTask task = Factory.CreateCTask();
			task.AppUid = app.Uid;
			CTaskWriter writer = new CTaskWriter(task,Factory);
			writer.Task.Execution.Parameters = par;
		
			if (UnitStrength.Strong == par.ClusterAllocation) {
				writer.GenerateJobBatch();
			}
		
			foreach (DeclaredDataPin pin in app.DeclaredPins) {
				pin.TokenNo = writer.GenerateNextToken();
				if (DataBinding.Provided != pin.Binding && null != pin.Outgoing)
					pin.Outgoing.Target.TokenNo = pin.TokenNo;
				writer.GenerateDataToken(pin,null == pin.Access || "" == pin.Access.JSONSchema);
			}
			foreach (UnitCall call in app.Calls)
			foreach (ComputedDataPin pin in call.Pins)
				if (null == pin.Incoming && null == pin.Outgoing) {
					pin.TokenNo = writer.GenerateNextToken();
					writer.GenerateDataToken(pin,null == pin.Access || "" == pin.Access.JSONSchema);
				}
		
			TranslateApp(par.ClusterAllocation,app,writer,par.Invariants);

			//*test*
			Console.WriteLine(ConsoleString() + "TranslateApp: " + app.Uid + "\n@@ " +
			                  writer.Task.ToString());
			//*test*
			
			TaskRegistry.StoreTask(writer.Task);
		
			//*test*
			Console.WriteLine(ConsoleString() + "InitiateTask FINISH: " + app.Uid);
			//*test*
    	
			return writer.Task.Uid;
		}

		/// 
		/// <param name="taskUid"></param>
		/// <param name="pinUid"></param>
		/// <param name="ds"></param>
		public void InjectDataSet(string taskUid, string pinUid, CDataSet ds){
			TokenMessage tm = null;
			CDataToken dt = null;
		
			//*test*
			Console.WriteLine(ConsoleString() + "TaskManager InjectDataSet START: " + ds.Name);
			//*test*
    	
			CTask task = TaskRegistry.GetTask(taskUid);
    	
			if (null != task) {
				if (0 != task.Tokens.Count)
					dt = task.Tokens.Find(dtk => dtk.PinUid == pinUid);
				else
					dt = task.Batches[0].Tokens.Find(dtk => dtk.PinUid == pinUid);
				if (null != dt) {
					TaskRegistry.SetDataSet(ds,dt.Uid);
				
					tm = new TokenMessage();
					tm.TokenNo = DataBinding.Provided != dt.Binding ? dt.TokenNo : -dt.TokenNo;
					tm.TaskUid = task.Uid;
					tm.PinName = dt.PinName;
					tm.DataSet = new CDataSet(){Name = ds.Name,JSONPars=ds.JSONPars};
					//tm.seq_stack = new List<SeqToken>();
				}
			}
    	
			if (tm != null) TaskProcessor.PutTokenMessage(tm);
		
			//*test*
			Console.WriteLine(ConsoleString() + "TaskManager injectDataSet FINISH: " + ds.Name);
			//*test*
		}
	
		/// 
		/// <param name="appUid"></param>
		/// <param name="version"></param>
		/// <param name="diagramUid"></param>
		public ComputationApplicationRelease CreateAppRelease(string appUid, string version, string diagramUid){
			CALDiagram diagram = UnitRegistry.GetDiagram(diagramUid);
			ComputationApplicationRelease appRelease = DiagramUtility.TranslateDiagram(diagram);
			UnitRegistry.AddReleaseToUnit(appUid,appRelease);
			return appRelease;
		}

		///
		/// <param name="strength"></param>
		/// <param name="unitCall"></param>
		/// <param name="task"></param>
		/// <param name="invs"></param>
		private void TranslateUnitCall(UnitStrength strength, UnitCall unitCall, CTaskWriter task, List<CustomInvariantValue> invs){
			if ((unitCall.Strength == UnitStrength.Strong || unitCall.Unit is ComputationModuleRelease)
			    && strength == UnitStrength.Weak) {
				task.GenerateJobBatch();
				foreach (ComputedDataPin pin in unitCall.Pins)
					task.GenerateDataToken(pin,null == pin.Access || "" == pin.Access.JSONSchema);
			}
			List<CustomInvariantValue> callInvariants = new List<CustomInvariantValue>();
			CustomInvariantValue cInvV; InvariantValue invV;
			if (unitCall.Unit is ComputationModuleRelease) {
				
				foreach(ExecInvariant eInv in unitCall.Unit.Invariants){
					if (null != (cInvV = invs.Find(inv => eInv.Uid == inv.InvariantUid)))
					    callInvariants.Add(cInvV);
					else if (null!= (invV = unitCall.InvariantValues.Find(val => eInv.Uid == val.InvarDeclaration.Uid)))
						callInvariants.Add(new CustomInvariantValue{JSONValue = invV.JSONDefault, InvariantUid = eInv.Uid});
				}
				
				task.GenerateJob((ComputationModuleRelease) unitCall.Unit, callInvariants);
				foreach (ComputedDataPin pin in unitCall.Pins)
					task.GenerateDataToken(pin,null == pin.Access || "" == pin.Access.JSONSchema);
			} else {
				ComputationApplicationRelease app = (ComputationApplicationRelease)unitCall.Unit;
				foreach (ComputedDataPin pin in unitCall.Pins)
					pin.Declared.TokenNo = pin.TokenNo;
				
				foreach(ExecInvariant eInv in app.Invariants){
					if (null != (cInvV = invs.Find(inv => eInv.Uid == inv.InvariantUid)))
					    callInvariants.Add(new CustomInvariantValue{JSONValue = cInvV.JSONValue, InvariantUid = eInv.TargetInvariant.Uid});
					else if (null!= (invV = unitCall.InvariantValues.Find(val => eInv.Uid == val.InvarDeclaration.Uid)))
						callInvariants.Add(new CustomInvariantValue{JSONValue = invV.JSONDefault, InvariantUid = eInv.TargetInvariant.Uid});
				}
				
				if (strength == UnitStrength.Weak)
					TranslateApp(unitCall.Strength,app,task,callInvariants);
				else TranslateApp(UnitStrength.Strong,app,task,callInvariants);
			}
		}

		///
		/// <param name="strength"></param>
		/// <param name="app"></param>
		/// <param name="task"></param>
		/// <param name="invs"></param>
		private void TranslateApp(UnitStrength strength, ComputationApplicationRelease app, CTaskWriter task, List<CustomInvariantValue> invs){
			foreach (DataFlow flow in app.Flows) {
				if (flow.Source is DeclaredDataPin)
					flow.Target.TokenNo = flow.Source.TokenNo;
				else if (flow.Target is DeclaredDataPin) 
					flow.Source.TokenNo = flow.Target.TokenNo;
				else {
					flow.Source.TokenNo = task.GenerateNextToken();
					flow.Target.TokenNo = flow.Source.TokenNo;
				}
			}
			foreach (UnitCall call in app.Calls)
				TranslateUnitCall(strength,call,task,invs);
		}
	
		private string ConsoleString(){
			return "@@ TaskManager " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " @@ ";
		}
	
	}
}//end TaskManager