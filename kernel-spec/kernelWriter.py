import json
from platform import system
from os import getcwd

arguments = []

if system() != "Windows":
	arguments.append("mono")

path = getcwd().replace("kernel-spec", "") + "/Kernel/bin/Release/iCSharp.Kernel.exe"
arguments.append(path)
arguments.append("{connection_file}")

kernelData = {}

kernelData["argv"] = arguments
kernelData["display_name"] = "C#"
kernelData["language"] = "csharp"

with open('kernel-spec/kernel.json', 'w') as outfile:
    json.dump(kernelData, outfile)



