///////////////////////////////////////////////////////////
//  AssetRegistry.cs
//  Implementation of the Class AssetRegistry
//  Generated by Enterprise Architect
//  Created on:      02-mar-2020 09:58:55
//  Original author: smialek
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Diagram;

namespace Baltic.Database.UnitRegistry
{
	public class UnitRegistryMock : IUnitProcessing
	{
	
		List<ComputationApplicationRelease> _cars = new List<ComputationApplicationRelease>();
		List<ComputationApplication> _cas = new List<ComputationApplication>();
		List<ComputationModuleRelease> _cmrs = new List<ComputationModuleRelease>();
		List<ComputationModule> _cms = new List<ComputationModule>();

		public UnitRegistryMock()
		{
		
			// #### Computation Modules ##################
		
		
			ComputationModule cmo00 = new ComputationModule {
				Name = "Frame Splitter"					
			};
			ComputationModule cmo0 = new ComputationModule {
				Name = "Image Splitter"					
			};
			_cms.Add(cmo0);
			ComputationModule cmo1 = new ComputationModule {
				Name = "Image Processor"					
			};
			_cms.Add(cmo1);
			ComputationModule cmo2 = new ComputationModule {
				Name = "Image Merger"					
			};
			_cms.Add(cmo2);
			ComputationModule cmm = new ComputationModule {
				Name = "Image Splitter"
			};
			_cms.Add(cmm);
		
			// #### Computation Unit Releases ##########
		
			ComputationApplicationRelease app = null;
		
			//--------------------------------------------
			ComputationModuleRelease cm00 = new ComputationModuleRelease {
				Uid = "fs001",
				YAML = "fs001",
				Version = "0.1"
			};
			DeclaredDataPin mdp10 = new DeclaredDataPin {
				Name = "film",
				Uid = "film01",
				Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Single,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp20 = new DeclaredDataPin {
				Name = "filmf1",
				Uid = "filmf01",
				Binding = DataBinding.Provided,
				Multiplicity = DataMultiplicity.Multiple,
				Access = new AccessType(){JSONSchema = "file"}
			};
			cm00.DeclaredPins.Add(mdp10);
			cm00.DeclaredPins.Add(mdp20);
			cm00.Unit = cmo00;
		
			_cmrs.Add(cm00);
		
			//---------------------------------------------
			ComputationModuleRelease cm0 = new ComputationModuleRelease {
				Uid = "is001",
				YAML = "is001",
				Version = "0.1"
			};
			DeclaredDataPin mdp01 = new DeclaredDataPin {
				Name = "image",
				Uid = "image01",
				Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Single,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp02 = new DeclaredDataPin {
				Name = "imagep1",
				Uid = "imagep01",
				Binding = DataBinding.Provided,
				Multiplicity = DataMultiplicity.Multiple,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp03 = new DeclaredDataPin {
				Name = "imagep2",
				Uid = "imagep02",
				Binding = DataBinding.Provided,
				Multiplicity = DataMultiplicity.Multiple,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp04 = new DeclaredDataPin {
				Name = "imagep3",
				Uid = "imagep03",
				Binding = DataBinding.Provided,
				Multiplicity = DataMultiplicity.Multiple,
				Access = new AccessType(){JSONSchema = "file"}
			};
			cm0.DeclaredPins.Add(mdp01);
			cm0.DeclaredPins.Add(mdp02);
			cm0.DeclaredPins.Add(mdp03);
			cm0.DeclaredPins.Add(mdp04);
			cm0.Unit = cmo0;
		
			_cmrs.Add(cm0);
		
			//--------------------------------------------
			ComputationModuleRelease cm1 = new ComputationModuleRelease {
				Uid = "ip002",
				YAML = "ip002",
				Version = "0.1"
			};
			DeclaredDataPin mdp11 = new DeclaredDataPin {
				Name = "imagep",
				Uid = "imagep00",
				Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Single,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp12 = new DeclaredDataPin {
				Name = "imagepp",
				Uid = "imagepp00",
				Binding = DataBinding.Provided,
				Multiplicity = DataMultiplicity.Single,
				Access = new AccessType(){JSONSchema = "file"}
			};
			cm1.DeclaredPins.Add(mdp11);
			cm1.DeclaredPins.Add(mdp12);
			cm1.Unit = cmo1;
		
			_cmrs.Add(cm1);
		
			//--------------------------------------------
			ComputationModuleRelease cm2 = new ComputationModuleRelease {
				Uid = "im003",
				YAML = "im003",
				Version = "0.1"
			};
			DeclaredDataPin mdp21 = new DeclaredDataPin {
				Name = "imagerp1",
				Uid = "imagerp01",
				Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Multiple,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp22 = new DeclaredDataPin {
				Name = "imagerp2",
				Uid = "imagerp02",
				Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Multiple,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp23 = new DeclaredDataPin {
				Name = "imagerp3",
				Uid = "imagerp03",
				Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Multiple,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp24 = new DeclaredDataPin {
				Name = "fimage",
				Uid = "fimage00",
				Binding = DataBinding.Provided,
				Multiplicity = DataMultiplicity.Single,
				Access = new AccessType(){JSONSchema = "file"}
			};
			cm2.DeclaredPins.Add(mdp21);
			cm2.DeclaredPins.Add(mdp22);
			cm2.DeclaredPins.Add(mdp23);
			cm2.DeclaredPins.Add(mdp24);
			cm2.Unit = cmo2;
		
			_cmrs.Add(cm2);
		
			ComputationModuleRelease cm = new ComputationModuleRelease {
				Uid = "is001", 
				YAML = "is001",
				Version = "0.1"
			};
			DeclaredDataPin mdp1 = new DeclaredDataPin {
				Name = "image", Uid = "image01", Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Multiple,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp2 = new DeclaredDataPin {
				Name = "image_no", Uid = "image_no01", Binding = DataBinding.Provided,
				Multiplicity = DataMultiplicity.Single,
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin mdp3 = new DeclaredDataPin {
				Name = "stream", Uid = "stream_no01", Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Single,
				Access = new AccessType(){JSONSchema = "file"}
			};
			cm.DeclaredPins.Add(mdp1);
			cm.DeclaredPins.Add(mdp2);
			cm.DeclaredPins.Add(mdp3);
			cm.Unit = cmm;
		
			_cmrs.Add(cm);
		
			// #### Computation Applications ######################################
		
			//----------------------------------------------
			ComputationApplication capp = new ComputationApplication {
				Name = "YetAnotherImageProcessor"
			};
			_cas.Add(capp);
		
			ComputationApplication capp2 = new ComputationApplication {
				Name = "MyApp"
			};
			_cas.Add(capp2);
			ComputationApplication capp3 = new ComputationApplication {
				Name = "MyNextApp"
			};
			_cas.Add(capp3);
		
			// #### Computation Application Releases ##############################
		
			app = new ComputationApplicationRelease {
				Uid = "YetAnotherImageProcessor_x01y2",
				Version = "0.1"
			};
			app.Unit = capp;
			DeclaredDataPin dp01 = new DeclaredDataPin {
				Name = "Film",
				Uid = "Film01",
				Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Single,
				Type = new DataType(),
				Access = new AccessType(){JSONSchema = "file"}
			};
			DeclaredDataPin dp02 = new DeclaredDataPin {
				Name = "Proc_Film",
				Uid = "Proc_Film01",
				Binding = DataBinding.Provided,
				Multiplicity = DataMultiplicity.Single,
				Access = new AccessType(){JSONSchema = "file"}
			};
			app.DeclaredPins.Add(dp01);
			app.DeclaredPins.Add(dp02);
		
			//--------------------------
			UnitCall uc00 = new UnitCall {
				Name = "frame splitter",
				Strength = UnitStrength.Strong,
				Unit = cm00
			};
			ComputedDataPin cdp10 = new ComputedDataPin {
				Uid = "film_1212",
				Declared = mdp10
			};
			ComputedDataPin cdp20 = new ComputedDataPin {
				Uid = "filmf1_1212",
				Declared = mdp20
			};
		
			uc00.Pins.Add(cdp10);
			uc00.Pins.Add(cdp20);
			app.Calls.Add(uc00);
		
			//--------------------------
			UnitCall uc0 = new UnitCall {
				Name = "splitter",
				Strength = UnitStrength.Strong,
				Unit = cm0
			};
			ComputedDataPin cdp01 = new ComputedDataPin {
				Uid = "image02",
				Declared = mdp01
			};
			ComputedDataPin cdp02 = new ComputedDataPin {
				Uid = "image_no021",
				Declared = mdp02
			};
			ComputedDataPin cdp03 = new ComputedDataPin {
				Uid = "image_no022",
				Declared = mdp03
			};
			ComputedDataPin cdp04 = new ComputedDataPin {
				Uid = "image_no023",
				Declared = mdp04
			};
			uc0.Pins.Add(cdp01);
			uc0.Pins.Add(cdp02);
			uc0.Pins.Add(cdp03);
			uc0.Pins.Add(cdp04);
			app.Calls.Add(uc0);
		
			//--------------------------
			UnitCall uc1 = new UnitCall {
				Name = "processor1",
				Strength = UnitStrength.Strong,
				Unit = cm1
			};
			ComputedDataPin cdp11 = new ComputedDataPin {
				Uid = "image021",
				Declared = mdp11
			};
			ComputedDataPin cdp12 = new ComputedDataPin {
				Uid = "image_nop021",
				Declared = mdp12
			};
			uc1.Pins.Add(cdp11);
			uc1.Pins.Add(cdp12);
			app.Calls.Add(uc1);
		
			//--------------------------
			UnitCall uc2 = new UnitCall {
				Name = "processor2",
				Strength = UnitStrength.Strong,
				Unit = cm1
			};
			ComputedDataPin cdp21 = new ComputedDataPin {
				Uid = "image031",
				Declared = mdp11
			};
			ComputedDataPin cdp22 = new ComputedDataPin {
				Uid = "image_nop031",
				Declared = mdp12
			};
			uc2.Pins.Add(cdp21);
			uc2.Pins.Add(cdp22);
			app.Calls.Add(uc2);
		
			//--------------------------
			UnitCall uc3 = new UnitCall {
				Name = "processor3",
				Strength = UnitStrength.Strong,
				Unit = cm1
			};
			ComputedDataPin cdp31 = new ComputedDataPin {
				Uid = "image041",
				Declared = mdp11
			};
			ComputedDataPin cdp32 = new ComputedDataPin {
				Uid = "image_nop041",
				Declared = mdp12
			};
			uc3.Pins.Add(cdp31);
			uc3.Pins.Add(cdp32);
			app.Calls.Add(uc3);
		
			//--------------------------
			UnitCall uc4 = new UnitCall {
				Name = "merger",
				Strength = UnitStrength.Strong,
				Unit = cm2
			};
			ComputedDataPin cdp41 = new ComputedDataPin {
				Uid = "imager021",
				Declared = mdp21
			};
			ComputedDataPin cdp42 = new ComputedDataPin {
				Uid = "imager022",
				Declared = mdp22
			};
			ComputedDataPin cdp43 = new ComputedDataPin {
				Uid = "imagr023",
				Declared = mdp23
			};
			ComputedDataPin cdp44 = new ComputedDataPin {
				Uid = "fimage02",
				Declared = mdp24
			};
		
			uc4.Pins.Add(cdp41);
			uc4.Pins.Add(cdp42);
			uc4.Pins.Add(cdp43);
			uc4.Pins.Add(cdp44);
		
			//----------------------------
			PinGroup pg1 = new PinGroup {
				Name = "images"
			};
			pg1.Depths.Add(0);
			pg1.Depths.Add(1);
			cdp41.Group = pg1;
			cdp42.Group = pg1;
			cdp43.Group = pg1;
		
			app.Calls.Add(uc4);
		
			//--------------------------------------------------
			DataFlow df00 = new DataFlow {
				Source = dp01,
				Target = cdp10
			};
			dp01.Outgoing = df00;
			cdp10.Incoming = df00;
		
			DataFlow df01 = new DataFlow {
				Source = cdp20,
				Target = cdp01
			};
			cdp20.Outgoing = df01;
			cdp01.Incoming = df01;
		
			DataFlow df02 = new DataFlow {
				Source = cdp02,
				Target = cdp11
			};
			cdp02.Outgoing = df02;
			cdp11.Incoming = df02;
			DataFlow df03 = new DataFlow {
				Source = cdp03,
				Target = cdp21
			};
			cdp03.Outgoing = df03;
			cdp21.Incoming = df03;
			DataFlow df04 = new DataFlow {
				Source = cdp04,
				Target = cdp31
			};
			cdp04.Outgoing = df04;
			cdp31.Incoming = df04;
		
			DataFlow df05 = new DataFlow {
				Source = cdp12,
				Target = cdp41
			};
			cdp12.Outgoing = df05;
			cdp41.Incoming = df05;
			DataFlow df06 = new DataFlow {
				Source = cdp22,
				Target = cdp42
			};
			cdp22.Outgoing = df06;
			cdp42.Incoming = df06;
			DataFlow df07 = new DataFlow {
				Source = cdp32,
				Target = cdp43
			};
			cdp32.Outgoing = df07;
			cdp43.Incoming = df07;
		
			DataFlow df08 = new DataFlow {
				Source = cdp44,
				Target = dp02
			};
			cdp44.Outgoing = df08;
			dp02.Incoming = df08;
		
			app.Flows.Add(df01);
			app.Flows.Add(df02);
			app.Flows.Add(df03);
			app.Flows.Add(df04);
			app.Flows.Add(df05);
			app.Flows.Add(df06);
			app.Flows.Add(df07);
			app.Flows.Add(df08);
		
			_cars.Add(app);
		
			//##################################################################
		
			app = new ComputationApplicationRelease {
				Uid = "ala_r13y6",
				Version = "0.1"
			};
			app.Unit = capp2;
					
			DeclaredDataPin dp1 = new DeclaredDataPin {
				Name = "Image", Uid = "Image01", Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Multiple
			};
			DeclaredDataPin dp2 = new DeclaredDataPin {
				Name = "Proc_Image", Uid = "Proc_Image01", Binding = DataBinding.Provided,
				Multiplicity = DataMultiplicity.Single
			};
			DeclaredDataPin dp3 = new DeclaredDataPin {
				Name = "Stream", Uid = "Stream01", Binding = DataBinding.RequiredStrong,
				Multiplicity = DataMultiplicity.Single
			};
			app.DeclaredPins.Add(dp1);
			app.DeclaredPins.Add(dp2);
			app.DeclaredPins.Add(dp3);
					
			UnitCall uc = new UnitCall {
				Name = "splitter", Strength = UnitStrength.Strong, Unit = cm
			};
			app.Calls.Add(uc);
					
			ComputedDataPin cdp1 = new ComputedDataPin {
				Uid = "image02", Declared = mdp1
			};
			ComputedDataPin cdp2 = new ComputedDataPin {
				Uid = "image_no02", Declared = mdp2
			};
			ComputedDataPin cdp3 = new ComputedDataPin {
				Uid = "stream_no02", Declared = mdp3
			};
			uc.Pins.Add(cdp1);
			uc.Pins.Add(cdp2);
			uc.Pins.Add(cdp3);
					
			DataFlow df1 = new DataFlow {
				Source = dp1, Target = cdp1
			};
			DataFlow df2 = new DataFlow {
				Source = cdp2, Target = dp2
			};
			DataFlow df3 = new DataFlow {
				Source = dp3, Target = cdp3
			};
			app.Flows.Add(df1);
			app.Flows.Add(df2);
			app.Flows.Add(df3);
					
			dp1.Outgoing = df1;
			dp2.Incoming = df2;
			dp3.Outgoing = df3;
			cdp1.Incoming = df1;
			cdp2.Outgoing = df2;
			cdp3.Incoming = df3;

			_cars.Add(app);
		
			//##################################################################
		
//		app = new ComputationApplicationRelease {
//			uid = "bob_r1345",
//			version = "0.1"
//		};
//		app.unit = capp3;
//					
//		DeclaredDataPin dp11 = new DeclaredDataPin {
//			name = "Image", uid = "Image01", binding = DataBinding.required_strong,
//			multiplicity = DataMultiplicity.multiple
//		};
//		DeclaredDataPin dp12 = new DeclaredDataPin {
//			name = "Proc_Image", uid = "Proc_Image01", binding = DataBinding.provided,
//			multiplicity = DataMultiplicity.single
//		};
//		app.declared_pins.Add(dp11);
//		app.declared_pins.Add(dp12);
//		
//		ComputationApplicationRelease inv_app = cars.Find(ap => ap.uid == "YetAnotherImageProcessor_x01y2");
//		UnitCall uc11 = new UnitCall {
//			name = "splitter", binding = DataBinding.required_strong, unit = inv_app
//		};
//		app.calls.Add(uc11);
//
//		cars.Add(app);
	
		}

		~UnitRegistryMock()
		{

		}

		public virtual void Dispose()
		{

		}

		/// 
		/// <param name="releaseUid"></param>
		public ComputationUnitRelease GetUnitRelease(string releaseUid)
		{
			return _cars.Find(app => app.Uid == releaseUid);
		}
	
		/// 
		/// <param name="unitUid"></param>
		/// <param name="release"></param>
		public string AddReleaseToUnit(string unitUid, ComputationUnitRelease release){
			return null;
		}

		/// 
		/// <param name="diagramUid"></param>
		public CALDiagram GetDiagram(string diagramUid){
			return null;
		}

		/// 
		/// <param name="unitUid"></param>
		public ComputationUnit GetUnit(string unitUid){
			return null;
		}
	}
}//end AssetRegistry
