using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Smx.KodiInterop.Python
{
    public class PyFunctionBuilder
    {
		public string Name { get; private set; }

		private readonly StringWriter _sw = new StringWriter();
		public IndentedTextWriter Writer { get; private set; }

		public PyFunctionBuilder(string functionName) {
			if(functionName == null) {
				functionName = "_f" + PyVariableManager.Get.GetFreeVariableName();
			}
			this.Name = functionName;
			this.Writer = new IndentedTextWriter(this._sw);
			
			// TODO: arguments (when and if they will be needed)
			Writer.WriteLine(string.Format("def {0}():", this.Name));
			Writer.Indent++;
		}

		public string GetCode() {
			Writer.Close();
			return _sw.ToString();
		}

		/// <summary>
		/// Evaluates the generated function
		/// </summary>
		public void Install() {
			PythonInterop.Eval(this.GetCode());
		}	
	}
}
