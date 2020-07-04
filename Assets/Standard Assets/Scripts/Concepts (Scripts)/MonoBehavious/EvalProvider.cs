using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using System.Data;
using System.Reflection;

public static class EvalProvider
{  
    public static Func<T, TResult> CreateEvalMethod<T, TResult>(string code, string[] usingStatements = null, string[] assemblies = null)
    {
        GameManager.print(0);
        Type returnType = typeof(TResult);
        Type inputType = typeof(T);
        var includeUsings = new HashSet<string>(new[] { "System" });
        includeUsings.Add(returnType.Namespace);
        includeUsings.Add(inputType.Namespace);
        if (usingStatements != null)
            foreach (var usingStatement in usingStatements)
                includeUsings.Add(usingStatement);
        using (CSharpCodeProvider compiler = new CSharpCodeProvider())  
        {  
            var name = "F" + Guid.NewGuid().ToString().Replace("-", string.Empty);  
            var includeAssemblies = new HashSet<string>(new[] { "system.dll" });  
            if (assemblies != null)  
                foreach (var assembly in assemblies)  
                    includeAssemblies.Add(assembly);  
  
            var parameters = new CompilerParameters(includeAssemblies.ToArray())  
            {  
                GenerateInMemory = true  
            };  
            string source = string.Format(@" 
{0} 
namespace {1} 
{{ 
    public static class EvalClass 
    {{ 
        public static {2} Eval({3} arg) 
        {{ 
            {4} 
        }} 
    }} 
}}", GetUsing(includeUsings), name, returnType.Name, inputType.Name, code);  
  
            var compilerResult = compiler.CompileAssemblyFromSource(parameters, source);  
            var compiledAssembly = compilerResult.CompiledAssembly;  
            var type = compiledAssembly.GetType(string.Format("{0}.EvalClass", name));  
            var method = type.GetMethod("Eval");  
            return (Func<T, TResult>)Delegate.CreateDelegate(typeof(Func<T, TResult>), method);  
        }  
    }

    public static object Eval (string sExpression)
    {
        CSharpCodeProvider c = new CSharpCodeProvider();
        CompilerParameters cp = new CompilerParameters();
        cp.ReferencedAssemblies.Add("system.dll");
        cp.CompilerOptions = "/t:library";
        cp.GenerateInMemory = true;
        StringBuilder sb = new StringBuilder("");
        sb.Append("using System;\n");
        sb.Append("namespace CSCodeEvaler{ \n");
        sb.Append("public class CSCodeEvaler{ \n");
        sb.Append("public object EvalCode(){\n");
        sb.Append("return " + sExpression + "; \n");
        sb.Append("} \n");
        sb.Append("} \n");
        sb.Append("}\n");
        CompilerResults cr = c.CompileAssemblyFromSource(cp, sb.ToString());
        if (cr.Errors.Count > 0)
        {
            throw new InvalidExpressionException(
                string.Format("Error ({0}) evaluating: {1}", 
                cr.Errors[0].ErrorText, sExpression));
        }
        System.Reflection.Assembly a = cr.CompiledAssembly;
        object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");
        Type t = o.GetType();
        MethodInfo mi = t.GetMethod("EvalCode");
        object s = mi.Invoke(o, null);
        return s;
    }

    static string GetUsing (HashSet<string> usingStatements)  
    {
        StringBuilder result = new StringBuilder();  
        foreach (string usingStatement in usingStatements)  
        {  
            result.AppendLine(string.Format("using {0};", usingStatement));  
        }  
        return result.ToString();  
    }
}