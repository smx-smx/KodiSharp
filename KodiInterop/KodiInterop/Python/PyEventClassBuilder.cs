using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Smx.KodiInterop.Python
{
    public class PyEventClassBuilder 
	{
		public string ClassName { get; private set; }
		public string EventDest { get; private set; }
		public string BaseClassName { get; private set; }

		public readonly List<string> Methods = new List<string>();

		private readonly StringWriter _sw = new StringWriter();
		private readonly IndentedTextWriter _itw;

		public PyEventClassBuilder(string baseClass, Type classType = null) {
			this._itw = new IndentedTextWriter(this._sw);
			this.BaseClassName = baseClass;
			this.ClassName = PyVariableManager.Get.GetFreeVariableName() + baseClass.Replace(".", string.Empty); ;
			if(classType != null) {
				this.EventDest = classType.FullName;
			}
		}

		private void MakeHandler(string eventName) {
			_itw.WriteLine(string.Format("def {0}(self, *args, **kwargs):", eventName));
			_itw.Indent++;
			{
				_itw.WriteLine(string.Format(@"return PostEvent(json.dumps({{
					""source"": ""{0}"",
					""sender"": ""{1}"",
					""args"" : args,
					""kwargs"" : kwargs
				}}))", this.EventDest, eventName));
				_itw.Indent--;
			}
		}

		public PyVariable NewInstance(List<object> arguments = null) {
			PyVariable instance = PyVariableManager.Get.NewVariable();

            List<object> args = new List<object>()
            {
                "self.bridge"
            };
            if(arguments != null)
                args.AddRange(arguments);

			return instance.CallAssign(PyFunction.ClassFunction(this.ClassName), args);
		}

		public string GetCode() {
			_itw.WriteLine(string.Format("class {0}({1}):", this.ClassName, this.BaseClassName));
			_itw.Indent++;
			{
				_itw.WriteLine("def __init__(self, *args, **kwargs):");
				_itw.Indent++;
				{
					_itw.WriteLine(string.Format("super({0}, self).__init__()", this.BaseClassName));
					//_itw.WriteLine(string.Format("self.Source = '{0}'", this.ClassName));
					_itw.Indent--;
				}

				foreach (string eventName in this.Methods) {
					this.MakeHandler(eventName);
				}
				_itw.Indent--;
			}
			_itw.Close();
			return _sw.ToString();
		}

		/// <summary>
		/// Evaluates the generated class
		/// </summary>
		public void Install() {
			PythonInterop.Eval(this.GetCode());
		}
	}
}
