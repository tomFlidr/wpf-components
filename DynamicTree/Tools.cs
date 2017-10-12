using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;

public class Tools {
	/// <summary>
	/// Return Type object by sstring in forms: "Full.Class.Name" or "AssemblyName:Full.Class.Name"
	/// </summary>
	/// <param name="fullClassName" type="String">"Full.Class.Name" or "AssemblyName:Full.Class.Name"</param>
	/// <returns type="Type">Desired type</returns>
	public static Type GetTypeGlobaly (string fullClassName) {

		Type type = Type.GetType(fullClassName);
		if (type != null) {
			return type;
		}

		if (fullClassName.IndexOf(":") > -1) {
			string[] fullNameAndAssembly = fullClassName.Split(':');
			type = Tools.GetTypeGlobaly(fullNameAndAssembly[0], fullNameAndAssembly[1]);
		}

		if (type == null) {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (System.Reflection.Assembly assembly in assemblies) {
				type = assembly.GetType(fullClassName);
				if (type != null) {
					break; // TODO: might not be correct. Was : Exit For
				}
			}
		}

		return type;

	}

	/// <summary>
	/// Return Type object by two strings in form: "AssemblyName", "Full.Class.Name"
	/// </summary>
	/// <param name="assemblyName" type="String">"AssemblyName" for AssemblyName.dll</param>
	/// <param name="fullClassName" type="String">Full class name including namespace</param>
	/// <returns type="Type">Desired type</returns>
	public static Type GetTypeGlobaly (string assemblyName, string fullClassName) {

		Type type = null;

		// ziska pouze assemblies, které jsou načteny do paměti protože se použily
		//Dim assemblies As Reflection.Assembly() = AppDomain.CurrentDomain.GetAssemblies()

		// načte všechny assemblies které jsou v nastavení referencí
		//AppDomain.CurrentDomain.GetAssemblies()

		// načte všechny assemblies které jsou ve složce
		try {
			IEnumerable<Assembly> assemblies = 
				from file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory)
				where Path.GetExtension(file) == ".dll"
				select Assembly.LoadFrom(file);
			foreach (System.Reflection.Assembly assembly in assemblies) {
				if (assembly.GetName().Name == assemblyName) {
					type = assembly.GetType(fullClassName);
					break; // TODO: might not be correct. Was : Exit For
				}
			}
		} catch (Exception ex) {
		}

		return type;

	}

	public static void DispatchEvent (object source, string eventName, EventArgs eventArgs) {
		EventInfo eventObject = source.GetType().GetEvent(eventName);


		if (eventObject != null) {
			IEnumerable<FieldInfo> fis = source.GetType().GetRuntimeFields();
			foreach (FieldInfo fi in fis) {

				if (fi.Name == eventName + "Event") {
					System.Delegate del = fi.GetValue(source) as System.Delegate;

					List<System.Delegate> invocationList = del.GetInvocationList().ToList();


					foreach (System.Delegate invItem in invocationList) {
						invItem.DynamicInvoke(source, eventArgs);

					}

				}
			}

		}
	}

}