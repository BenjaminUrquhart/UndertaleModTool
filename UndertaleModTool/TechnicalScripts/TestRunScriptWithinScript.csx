// By Grossley
ScriptMessage("Script start");
ScriptMessage("Running normal test script within this script");
bool x = RunUMTScript(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TechnicalScripts", "ScriptTestSuccess.txt"));
ScriptMessage("Test script ran successfully? Status: " + x.ToString());
ScriptMessage("Running broken test script within this script");
x = RunUMTScript(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TechnicalScripts", "ScriptTestFail.txt"));
ScriptMessage("Test script ran successfully? Status: " + x.ToString());
ScriptMessage("Script end");
