namespace Smx.KodiInterop
{
	public class PyVariable
	{
		public string Name { get; private set; }
		public object Value {
			get {
				return PythonInterop.GetVariable(this.Name);
			}
			set {
				PythonInterop.EvalToVar(this.Name, value.ToString());
			}
		}

		/// <summary>
		/// Represents a Python variable
		/// </summary>
		/// <param name="varName">Name of the variable</param>
		/// <param name="evalCode">Code that will be evaluated as the variable value/content</param>
		public PyVariable(string varName, string evalCode = null) {
			this.Name = varName;
			if(evalCode != null)
				this.Value = evalCode;
		}
	}
}