﻿

namespace iCSharp.Kernel.Shell
{
	using Common.Logging;
	using System.Collections.Generic;
	using Common.Serializer;
	using iCSharp.Messages;
	using NetMQ.Sockets;
	using iCSharp.Kernel.Helpers;
	using System.Text.RegularExpressions;
	using System.IO;
	using System.Reflection;
	using System;
	using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis;

    //using System.
    //using System.Linq.Expressions.Analyzer
    public class CompleteRequestHandler : IShellMessageHandler
	{
		private ILog logger;
		private readonly IMessageSender messageSender;

        public CompleteRequestHandler(ILog logger, IMessageSender messageSender)
		{
			this.logger = logger;
			this.messageSender = messageSender;
		}

		public void HandleMessage(Message message, RouterSocket serverSocket, PublisherSocket ioPub)
		{
			CompleteRequest completeRequest = JsonSerializer.Deserialize<CompleteRequest>(message.Content);

            //string code = completeRequest.CodeCells[completeRequest.Selected_Cell_Index];

            string line = completeRequest.Line;

		//	this.logger.Info("original code:" + code);
			this.logger.Info("original line:" + line);

		//	code = Regex.Replace(code.Substring(1, code.Length - 2), @"\\n", "*");
			line = line.Substring(1, line.Length - 2);

		//	this.logger.Info("substring code:" + code);
			this.logger.Info("substring line:" + line);

			Regex returnType = new Regex(@"[string|int|void]");
			Regex methodName = new Regex(@"(\w)");

			int cur_pos = completeRequest.CursorPosition;

			this.logger.Info("cur_pos " + cur_pos);
			//Type type = typeof(code);


			//string newCode = code.

			line = line.Substring(0, cur_pos); //get substring of code from start to cursor position


			//string[] arrayOfKeywords = { "team", "tech", "te", "term", "tame", "tata" };

			List<CompleteReplyMatch> DirectiveMatches = new List<CompleteReplyMatch>();
			//DirectivesList(ref DirectiveMatches);

			List<string> listOfKeywords = new List<string>();

			//listOfKeywords.AddRange(arrayOfKeywords);

			List<CompleteReplyMatch> matches_ = new List<CompleteReplyMatch>();

			List<MethodMatch> methodMatches = new List<MethodMatch>();

            List<ClassDeclarationSyntax> classList = new List<ClassDeclarationSyntax>();
            List<MethodDeclarationSyntax> methodList = new List<MethodDeclarationSyntax>();
            List<MethodDeclarationSyntax> globalMethodList = new List<MethodDeclarationSyntax>();

            List<CompleteReplyMatch> methodMatchNames = new List<CompleteReplyMatch>();

            List<CompleteReplyMatch> classMatchNames = new List<CompleteReplyMatch>();

            List<CompleteReplyMatch> interfaceNames = new List<CompleteReplyMatch>();

            List<CompleteReplyMatch> enumNames = new List<CompleteReplyMatch>();

            List<CompleteReplyMatch> structNames = new List<CompleteReplyMatch>();

            List<CompleteReplyMatch> propertyNames = new List<CompleteReplyMatch>();

            List<CompleteReplyMatch> variableNames = new List<CompleteReplyMatch>();

            List<VariableMatch> variableMatches = new List<VariableMatch>();

            var syntax_code = Regex.Replace(completeRequest.CodeCells[0].Substring(1, completeRequest.CodeCells[0].Length - 2), @"\\n", "*");

            var tree = CSharpSyntaxTree.ParseText(syntax_code);

            var syntaxRoot = tree.GetRoot();


            //      var MyClass = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            //     var MyMethod = syntaxRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

            //  Console.WriteLine("ayyyyyyy" + MyClass.Identifier.ToString());
            //  Console.WriteLine("yaaaaaaay" + MyMethod.Identifier.ToString());

            // CATCH STUFF

            catchInterfaces(syntaxRoot, ref interfaceNames);
            catchEnums(syntaxRoot, ref enumNames);
            catchStructs(syntaxRoot, ref structNames);
            catchProperties(syntaxRoot, ref propertyNames);
            catchVariables(syntaxRoot, ref variableNames);

            foreach (var codes in completeRequest.CodeCells)
            {
                var code = Regex.Replace(codes.Substring(1, codes.Length - 2), @"\\n", "*");
                
                //  CatchAllWords(ref matches_, code);
                //  CatchMethods(code, ref methodMatchNames, ref methodMatches);
                // VariableMatches(code, ref variableMatches);
            }

            CatchAllWords(ref matches_, code);

            List<CompleteReplyMatch> methodMatchNames = new List<CompleteReplyMatch>();

            CatchClassMethods(root, ref classList, ref methodList);

            Console.WriteLine("Class List");

            foreach (var l in classList)
            {
                Console.WriteLine(l.Identifier);
            }

            Console.WriteLine("Method List");

            foreach (var i in methodList)
            {
                Console.WriteLine(i.Identifier);
         //       Console.WriteLine(i.)

            }


            List<MethodDeclarationSyntax> MethList = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            bool found = false;
            foreach (var m in MethList)
            {
                foreach (var i in methodList)
                {
                    if (((m.Identifier).Equals(i.Identifier)) && ((m.Parent).Equals(i.Parent)))
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    globalMethodList.Add(m);
                }
                found = false;

            }





            Console.WriteLine("Global Method List");

            foreach (var i in globalMethodList)
            {
                Console.WriteLine(i.Identifier);
            }

            //globalMethodList = (globalMethodList.Distinct((IEnumerable<MethodDeclarationSyntax>) methodList)).ToList();

            Console.WriteLine("New Global Method List");

            foreach (var i in globalMethodList)
            {
                Console.WriteLine(i.Identifier);
            }



            //List<MethodDeclarationSyntax> intersection = methodList.Intersect((IEnumerable<>) globalMethodList);



            List<CompleteReplyMatch> classMatchNames = new List<CompleteReplyMatch>();
            //CatchClasses(code, ref classMatchNames);

            List<VariableMatch> variableMatches = new List<VariableMatch>();
            VariableMatches(code, ref variableMatches);

            //string[] dirs = Directory.GetDirectories



            AddKeywordsToMatches(ref matches_);

            foreach (MethodMatch m in methodMatches)
            {
                //this.logger.Info(m.Name);
                //this.logger.Info("number of params = " + m.ParamList.Count);
                //this.logger.Info("param types");
                for (int i = 0; i < m.ParamList.Count; i++)
                {
                    //this.logger.Info(m.ParamList[i]);
                }

            }

            //code = Regex.Replace(code, @"[^\w&^\.]", "*"); //replace all non word and dot characters with '*'

            Tuple<string, int> cursorInfo;
            cursorInfo = FindWordToAutoComplete(line);

            string cursorWord = cursorInfo.Item1;
            int cursorWordLength = cursorInfo.Item2;


            List<CompleteReplyMatch> matchesSign = new List<CompleteReplyMatch>();

            MakeMethodSignMatches(line, methodMatches, ref matchesSign);

            this.logger.Info("cursor word " + cursorWord);

            this.logger.Info("Before Removal");

            for (int j = matches_.Count - 1; j > -1; j--)
            {
                //this.logger.Info(matches_[j].Name);
            }

            //Console.WriteLine(cursorWord);

            RemoveNonMatches(ref matches_, cursorWord, line);

            this.logger.Info("After Removal");

            List<CompleteReplyMatch> FinalDirectives = new List<CompleteReplyMatch>();

            for (int j = matches_.Count - 1; j > -1; j--)
            {
                //this.logger.Info(matches_[j].Name);
            }

            if (line.Length > 0)
            {
                if (line.StartsWith("using "))
                {
                    Console.WriteLine("We're Starting with using!!!!!!!");
                    DirectivesList(ref DirectiveMatches, line);
                    List<string> l = DirectiveMatches.Select(x => x.Name).Distinct().ToList();
                    foreach (var i in l)
                    {
                        CompleteReplyMatch completeReplyMatch = new CompleteReplyMatch
                        {
                            Name = i,
                            Documentation = "",
                            Value = "",

                        };
                        FinalDirectives.Add(completeReplyMatch);
                    }
                    matches_ = FinalDirectives;

                    RemoveNonMatches(ref matches_, cursorWord, line);
                }
                else if (line[(line.Length - 1)].Equals('.'))
                {
                    Console.WriteLine("in function and cursor word is " + cursorWord);
                    matches_ = methodMatchNames;
                    matches_.AddRange(classMatchNames);
                    foreach (var i in variableMatches)
                    {
                        if (i.Name.Equals(cursorWord))
                        {
                            try
                            {
                                //Type type = Type.GetType(i.VarType);
                                ShowMethods(i.VarType, ref matches_);
                            }
                            catch (NullReferenceException e)
                            {
                                Console.WriteLine("Do Not Have This Type");
                            }

                        }
                    }
                    //Type type = typ
                }
                else if (line[(line.Length - 1)].Equals('('))
                {
                    matches_ = matchesSign;
                }


            }

            //TEMPORARY
            finalMatches.AddRange(interfaceNames);
            finalMatches.AddRange(enumNames);
            finalMatches.AddRange(structNames);
            finalMatches.AddRange(propertyNames);
            finalMatches.AddRange(variableNames);

            CompleteReply completeReply = new CompleteReply();

            if (line.Length > 0)
            {
                if (line[line.Length - 1] == '.')
                {
                    completeReply.Matches = finalMatches;
                    completeReply.Status = "ok";

                }
                else
                {
                    completeReply.Matches = finalMatches;
                    completeReply.Status = "ok";
                    completeReply.MatchedText = cursorWord;
                    completeReply.FilterStartIndex = ReplacementStartPosition;
                }
            }
            else
            {
                completeReply.Matches = finalMatches;
                completeReply.Status = "ok";
            }
            
            Message completeReplyMessage = MessageBuilder.CreateMessage(MessageTypeValues.CompleteReply, JsonSerializer.Serialize(completeReply), message.Header);
			this.logger.Info("Sending complete_reply");
			this.messageSender.Send(completeReplyMessage, serverSocket);

		}

		public void CatchAllWords(ref List<CompleteReplyMatch> matches_, string newCode)
		{
			//this.logger.Info(s);
			this.logger.Info("weselna elhamdlAllah");

			string catchPattern = @"(\w+)";

			Regex p = new Regex(catchPattern);
            
			foreach (Match m in p.Matches(newCode))
			{
				CompleteReplyMatch crm = new CompleteReplyMatch()
				{
					Name = m.ToString(),
					Documentation = "",
					Value = "",
                    Glyph = "variable"
				};
				matches_.Add(crm);
			}
		}
        
        public void catchInterfaces(SyntaxNode tree, ref List<CompleteReplyMatch> interfaceList)
        {
            var myInterface = tree.DescendantNodes().OfType<InterfaceDeclarationSyntax>().ToList();

            foreach (var node in myInterface)
            {
                CompleteReplyMatch crm = new CompleteReplyMatch()
                {
                    Name = node.Identifier.ToString(),// m.Groups["classname"].ToString(),
                    Documentation = "<font style=\"color:blue\">interface</font>" + node.Identifier.ToString(),
                    Value = "",
                    Glyph = "interface"
                };
                interfaceList.Add(crm);
            }
        }
        
        public void catchEnums(SyntaxNode tree, ref List<CompleteReplyMatch> enumList)
        {
            var myEnum = tree.DescendantNodes().OfType<EnumDeclarationSyntax>().ToList();
            
            foreach (var node in myEnum)
            {
                CompleteReplyMatch crm = new CompleteReplyMatch()
                {
                    Name = node.Identifier.ToString(),// m.Groups["classname"].ToString(),
                    Documentation = "<font style=\"color:blue\">enum</font>" + node.Identifier.ToString(),
                    Value = "",
                    Glyph = "enum"
                };
                enumList.Add(crm);
            }
        }

        public void catchStructs(SyntaxNode tree, ref List<CompleteReplyMatch> structList)
        {
            var myStruct = tree.DescendantNodes().OfType<StructDeclarationSyntax>().ToList();

            foreach (var node in myStruct)
            {
                CompleteReplyMatch crm = new CompleteReplyMatch()
                {
                    Name = node.Identifier.ToString(),// m.Groups["classname"].ToString(),
                    Documentation = "<font style=\"color:blue\">struct</font>" + node.Identifier.ToString(),
                    Value = "",
                    Glyph = "struct"
                };
                structList.Add(crm);
            }
        }

        public void catchProperties(SyntaxNode tree, ref List<CompleteReplyMatch> propertyList)
        {
            var myProperty = tree.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();

            foreach (var node in myProperty)
            {
                CompleteReplyMatch crm = new CompleteReplyMatch()
                {
                    Name = node.Identifier.ToString(),// m.Groups["classname"].ToString(),
                    Documentation = "<font style=\"color:blue\">property</font>" + node.Identifier.ToString(),
                    Value = "",
                    Glyph = "property"
                };
                propertyList.Add(crm);
            }
        }

        public void catchVariables(SyntaxNode tree, ref List<CompleteReplyMatch> variableList)
        {
            var myVariable = tree.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList();
            
            foreach (var node in myVariable)
            {
                CompleteReplyMatch crm = new CompleteReplyMatch()
                {
                    Name = node.Identifier.ToString(),// m.Groups["classname"].ToString(),
                    Documentation = "<font style=\"color:blue\">variable</font>" + node.Identifier.ToString(),
                    Value = "",
                    Glyph = "variable"
                };
                variableList.Add(crm);
            }
        }

      /*  public void CatchClasses(SyntaxNode tree, ref List<CompleteReplyMatch> classMatchNames)
		{
            //(?<documentation>\/\/\/(?:(?!\n\w).)*?)?(\n)*
            var MyClass = tree.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            
            this.logger.Info("Running catchclass");
            Regex p = new Regex(@"class([\s]+)(?<classname>\w+)");
            
			Regex doc = new Regex(@"(<summary>)(?<docsum>(?:(?!<\/summary>).)*)");
			string editedDoc;

			foreach (var node in MyClass)
			{
				//Type mytype = t(code); 
				//MethodInfo methodInfo = getMethods(m.Groups.ToString());
				//	.ToString();

				this.logger.Info("found class");

				Match i = doc.Match(m.Groups["documentation"].ToString());
				editedDoc = i.Groups["docsum"].ToString();
				editedDoc = editedDoc.Replace("///", "");
				editedDoc = editedDoc.Replace("*", "");
				editedDoc = editedDoc.TrimStart(' ');
				editedDoc = editedDoc.TrimEnd(' ');
				editedDoc = editedDoc.Replace(@"\s+", " ");

				this.logger.Info(editedDoc + " class new docsum here----");

				CompleteReplyMatch crm = new CompleteReplyMatch()
				{

					Name = node.Identifier.ToString(),// m.Groups["classname"].ToString(),
					Documentation = "this is a class",
					Value = "",
                    Glyph = "class"
				};
				classMatchNames.Add(crm);

			}
		}
        */

        public void CatchClassMethods(SyntaxNode root, ref List<ClassDeclarationSyntax> classList, ref List<MethodDeclarationSyntax> methodList)
        {

            //this.logger.Info("code in methods " + code);

            //[^(\n\w)]

            Regex p = new Regex(@"(?<documentation>\/\/\/(?:(?!\n\w).)*?)?(\n)*((?<access>public|private)([\s]+))?(?<returntype>string|sbyte|short|int|long|byte|ushort|uint|ulong|char|float|double|decimal|bool|enum|void)([\s]+)(?<methodname>\w+)(?<paramlist>\([^\)]*\))");

            Regex r = new Regex(@"(?<returntype>string|sbyte|short|int|long|byte|ushort|uint|ulong|char|float|double|decimal|bool|enum)([\s]+)(?<parnames>\w+)");

            Regex doc = new Regex(@"(<summary>)(?<docsum>(?:(?!<\/summary>).)*)");
            string editedDoc;


            List<ClassDeclarationSyntax> classListCaptured = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            //List<MemberDeclarationSyntax> methodList;

            //Console.WriteLine("class names");
            foreach (var l in classListCaptured)
            {
                /*Console.WriteLine("class name");
				Console.WriteLine(l.Identifier);
				Console.WriteLine("Class ParamList");
				Console.WriteLine(l.TypeParameterList);
				Console.WriteLine("class methods");
				Console.WriteLine(l.Members);
				Console.WriteLine("class directives");
                Console.WriteLine(l.ContainsDirectives);
				Console.WriteLine("class annotations");
                Console.WriteLine(l.ContainsAnnotations);
				Console.WriteLine("class attributes");
                Console.WriteLine(l.AttributeLists);
				Console.WriteLine("class baselist");
                Console.WriteLine(l.BaseList);
				Console.WriteLine("class close brace token");
                Console.WriteLine(l.CloseBraceToken);
				Console.WriteLine("class keyword");
                Console.WriteLine(l.Keyword);
				Console.WriteLine("class modifier");
				Console.WriteLine(l.Modifiers);
				Console.WriteLine("class type parameter list");
                Console.WriteLine(l.TypeParameterList);*/
                classList.Add(l);

                SyntaxTree tree = CSharpSyntaxTree.ParseText(l.Members.ToString());
                var MethRoot = tree.GetRoot();

                List<MethodDeclarationSyntax> MethList = MethRoot.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

                foreach (var m in MethList)
                {
                    /*Console.WriteLine("method modifers");
                    Console.WriteLine(m.Modifiers);
					Console.WriteLine("method identifier");
                    Console.WriteLine(m.Identifier);
					Console.WriteLine("method type parameter list");
                    Console.WriteLine(m.ParameterList);
					Console.WriteLine("method return type");
                    Console.WriteLine(m.ReturnType);
					//Console.WriteLine("class type parameter list");
                    //Console.WriteLine(m.);*/

                    methodList.Add(m);

                }


            }








            /*

			foreach (Match m in p.Matches(code))
			{



				List<string> parameterTypes = new List<string>();

				this.logger.Info("found method match");
				this.logger.Info(m.Groups["documentation"] + " documentation");
				Match i = doc.Match(m.Groups["documentation"].ToString());
				editedDoc = i.Groups["docsum"].ToString();

				this.logger.Info(i.Groups["docsum"].ToString() + " docsum here!!!");

				editedDoc = editedDoc.Replace("///", "");
				editedDoc = editedDoc.Replace("*", "");
				editedDoc = editedDoc.TrimStart(' ');
				editedDoc = editedDoc.TrimEnd(' ');
				editedDoc = editedDoc.Replace(@"\s+", " ");

				this.logger.Info(editedDoc + " method new docsum here----");
				this.logger.Info(m.Groups["methodname"] + " name" + "-- Access = " + m.Groups["access"]);
				this.logger.Info(m.Groups["returntype"] + " type");
				this.logger.Info(m.Groups["paramlist"] + " paramList");

				foreach (Match parType in r.Matches(m.Groups["paramlist"].ToString()))
				{

					this.logger.Info("we have a parameter");
					this.logger.Info(parType.Groups["returntype"]);
					this.logger.Info(parType.Groups["parnames"]);

					parameterTypes.Add(parType.Groups["returntype"].ToString());

				}

				MethodMatch mm = new MethodMatch()
				{
					Name = m.Groups["methodname"].ToString(),
					Documentation = editedDoc,
					Value = "",
					ParamList = parameterTypes,

				};
				methodMatches.Add(mm);

				CompleteReplyMatch crm = new CompleteReplyMatch()
				{

					Name = m.Groups["methodname"].ToString(),
					Documentation = editedDoc,
					Value = "",

				};
				methodMatchNames.Add(crm);
			}

*/
        }

        public void AddKeywordsToMatches(ref List<CompleteReplyMatch> matches_)
		{
            
			string[] arrayOfKeywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "using static", "virtual", "void", "volatile", "while" };
			List<string> listOfKeywords = new List<string>();
			listOfKeywords.AddRange(arrayOfKeywords);
            
			foreach (string i in listOfKeywords)
			{
				CompleteReplyMatch crm = new CompleteReplyMatch()
				{

					Name = i,
					Documentation = i + " Keyword",
					Value = "",
                    Glyph = "keyword"

				};
				matches_.Add(crm);

			}
		}

		public Tuple<string,int> FindWordToAutoComplete(string line)
		{
			line = Regex.Replace(line, @"[^\w&^\.]", "*");

			string cursorWord, cursorLine;
			int curWordLength = 0;

			Regex p = new Regex(@".*\*"); //regex to match up to last '*'
			Match mat = p.Match(line);

			if (mat.Success)
			{

				cursorLine = line.Substring(mat.Index + mat.Length);

			}
			else
			{
				cursorLine = line;
			}
			if (cursorLine.Length > 0)
			{
				if (cursorLine[cursorLine.Length - 1] == '.')
				{
					cursorLine = cursorLine.Substring(0, cursorLine.Length - 1);
				}
			}

			p = new Regex(@".*\.");
			mat = p.Match(cursorLine);

			if (mat.Success)
			{
				cursorWord = cursorLine.Substring(mat.Index + mat.Length);


			}
			else
			{
				cursorWord = cursorLine;
				cursorLine = "";
			}

			curWordLength = cursorWord.Length;

			if(line.Length>0){
				if(line[line.Length-1] == '.'){
					curWordLength = 0;
				}
			}

			return Tuple.Create(cursorWord, curWordLength);

		}

		public void RemoveNonMatches(ref List<CompleteReplyMatch> matches_, string cursorWord, string line)
		{

			//Console.WriteLine("1. " + matches_[0].Name + " 2. " + matches_[matches_.Count-1].Name);

			for (int j = matches_.Count - 1; j > -1; j--)
			{
				if ((line.StartsWith("using ")) && (line[line.Length-1] == '.')){
					return;
				}
				if (!(matches_[j].Name.StartsWith(cursorWord)))
				{
					matches_.RemoveAt(j);
				}
			}
		}

		public void MakeMethodSignMatches(string line, List<MethodMatch> methodMatches, ref List<CompleteReplyMatch> matchesSign)
		{

			Regex regex = new Regex(@"(?<name>\w+)(\()");

			Match i = regex.Match(line);

			string add;

			foreach (MethodMatch m in methodMatches)
			{

				add = "";
				if (i.Groups["name"].ToString().Equals(m.Name))
				{
					add = m.Name + "(";
					foreach (string s in m.ParamList)
					{
						add = add + s + ", ";
					}
					add = add + ")";
				}

				CompleteReplyMatch crm = new CompleteReplyMatch
				{
					Name = add,
					Documentation = "",
					Value = "",
                    Glyph = "method"
				};
				matchesSign.Add(crm);
			}

		}

		public void VariableMatches(string code, ref List<VariableMatch> variableMatches)
		{
			Console.WriteLine("GHAYARNA EL CODE1");
			Regex regex = new Regex(@"(?<type>string|sbyte|short|int|long|byte|ushort|uint|ulong|char|float|double|decimal|bool|enum|Comparer<[^>]*>|EqualityComparer<[^>]*>|HashSet<[^>]*>|LinkedList<[^>]*>|List<[^>]*>|Queue<[^*]>|SortedSet<[^*]>|Stack<[^*]>|)([\s]+)(?<name>\w+)");
			string type;
			foreach (Match m in regex.Matches(code))
			{
				Console.WriteLine("Found variable match");
				Console.WriteLine(m.Groups["type"]);
				type = m.Groups["type"].ToString();
				Type t = ProperTypeName(type);

				VariableMatch variableMatch = new VariableMatch
				{

					Name = m.Groups["name"].ToString(),
					VarType = t,

				};
				variableMatches.Add(variableMatch);



			}


		}

		public Type ProperTypeName(string type)
		{

			int index = type.IndexOf("<");
			if (index > 0)
			{
				type = type.Substring(0, index);
			}

			Console.WriteLine("type after index " + type);

			switch (type)
			{
				case "int":
					return typeof(int);
					break;
				case "string":
					return typeof(string);
					break;
				case "sbyte":
					return typeof(sbyte);
					break;
				case "short":
					return typeof(short);
					break;
				case "long":
					return typeof(long);
					break;
				case "byte":
					return typeof(byte);
					break;
				case "ushort":
					return typeof(ushort);
					break;
				case "ulong":
					return typeof(ulong);
					break;
				case "char":
					return typeof(char);
					break;
				case "float":
					return typeof(float);
					break;
				case "double":
					return typeof(double);
					break;
				case "decimal":
					return typeof(decimal);
					break;
				case "bool":
					return typeof(bool);
					break;
				case "Comparer":
					return typeof(Comparer<>);
					break;
				case "EqualityComparer":
					return typeof(EqualityComparer<>);
					break;
				case "HashSet":
					return typeof(HashSet<>);
					break;
				case "LinkedList":
					return typeof(LinkedList<>);
					break;
				case "List":
					Console.WriteLine("in the list branch");
					return typeof(List<>);
					break;
				case "Queue":
					return typeof(Queue<>);
					break;
				case "SortedSet":
					return typeof(SortedSet<>);
					break;
				case "Stack":
					return typeof(Stack<>);
					break;
				default:
					Console.WriteLine("in the default branch");
					return null;
					break;
			}

		}

		public void ShowMethods(Type type, ref List<CompleteReplyMatch> matches_)
		{
			foreach (var method in type.GetMethods())
			{
				var parameters = method.GetParameters();
				var parameterDescriptions = string.Join
					(", ", method.GetParameters()
								 .Select(x => x.ParameterType + " " + x.Name)
								 .ToArray());

				CompleteReplyMatch crm = new CompleteReplyMatch
				{

					Name = method.Name,
					Documentation = parameterDescriptions,
					Value = "",
                    Glyph = "method"
				};
				matches_.Add(crm);

			}
		}

		public void DirectivesList(ref List<CompleteReplyMatch> DirectiveMatches, string line)
		{
			Regex regex = new Regex(@"(using)([\s]+)(?<directive>(\w|\.)*)");
			Match match = regex.Match(line);
			string input = match.Groups["directive"].ToString();

			Console.WriteLine("Directive aho " + match.Groups["directive"].ToString());

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					//foreach (Type t in assembly.GetTypes())

					var namespaces = assembly.GetTypes()
						 .Select(t => GetToNearestDot(t, input))
						 .Distinct();

					foreach (var n in namespaces)
					{
						if (!(n.Equals(" "))) { 
						CompleteReplyMatch crm = new CompleteReplyMatch
						{
							Name = n,
							Documentation = "",
							Value = "",
                            Glyph = "directive"

						};
						DirectiveMatches.Add(crm);
					}
				}
				}
				catch (ReflectionTypeLoadException e)
				{
					Console.WriteLine("Could not load type");
				}
			}
		}

		public string GetToNearestDot(Type t, string s)
        {

            //Console.WriteLine("Arrived here " + t.FullName +" " + s);
            try{
                if (!(t.Namespace.StartsWith(s)))
                {
                    return " ";
                }
            }catch(NullReferenceException e)
            {
                return " ";
            }
			
			//Console.WriteLine("3adeina elhamdlAllah " + t.FullName);

            string ns = t.Namespace ?? "";

            int firstDot = s.LastIndexOf('.');

            //Console.WriteLine( "ns before " + ns);
            if (ns.Equals(""))
            {

            }
            else
            {
                ns = ns.Substring(firstDot + 1);
            }

            int firstDotns = ns.IndexOf('.');
            //int firstDot = s.LastIndexOf('.');

            //Console.WriteLine( "ns after " + ns);

            int finaldot = ns.IndexOf('.');

            return (finaldot == -1) ? ns : ns.Substring(0, finaldot);

        }


	}
}

