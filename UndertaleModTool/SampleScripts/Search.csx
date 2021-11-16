﻿using System.Text;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

EnsureDataLoaded();

if (Data.IsYYC())
{
    ScriptError("You cannot do a code search on a YYC game! There is no code to search!");
    return;
}

StringBuilder results = new();
ConcurrentDictionary<string, List<string>> resultsDict = new();
ConcurrentBag<string> failedList = new();
IOrderedEnumerable<string> failedSorted;                              //failedList.OrderBy()
IOrderedEnumerable<KeyValuePair<string, List<string>>> resultsSorted; //resultsDict.OrderBy()
int result_count = 0;

ThreadLocal<GlobalDecompileContext> DECOMPILE_CONTEXT = new ThreadLocal<GlobalDecompileContext>(() => new GlobalDecompileContext(Data, false));

bool case_sensitive = ScriptQuestion("Case sensitive?");
bool regex_check = ScriptQuestion("Regex search?");
string keyword = SimpleTextInput("Enter your search", "Search box below", "", false);
if (String.IsNullOrEmpty(keyword) || String.IsNullOrWhiteSpace(keyword))
{
    ScriptError("Search cannot be empty or null.");
    return;
}

SetProgressBar(null, "Code Entries", 0, Data.Code.Count);
StartUpdater();

await DumpCode();

await StopUpdater();

UpdateProgressStatus("Sorting results...");
await Task.Run(SortResults);

UpdateProgressStatus("Generating result list...");
await ClickableTextOutput("Search results.", keyword, result_count, resultsSorted, true, failedSorted);

HideProgressBar();
EnableUI();


string GetFolder(string path)
{
    return Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
}

async Task DumpCode()
{
    var data = DECOMPILE_CONTEXT.Value.Data;
    if (data?.KnownSubFunctions is null) //if we run script before opening any code
        Decompiler.BuildSubFunctionCache(data);

    await Task.Run(() => Parallel.ForEach(Data.Code, DumpCode));
}

void SortResults()
{
    if (failedList.Count > 0)
        failedSorted = failedList.OrderBy(c => Data.Code.IndexOf(Data.Code.ByName(c)));

    resultsSorted = resultsDict.OrderBy(c => Data.Code.IndexOf(Data.Code.ByName(c.Key)));
}

bool RegexContains(string s, string sPattern, bool isCaseInsensitive)
{
    if (isCaseInsensitive)
        return System.Text.RegularExpressions.Regex.IsMatch(s, sPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    return System.Text.RegularExpressions.Regex.IsMatch(s, sPattern);
}
void DumpCode(UndertaleCode code)
{
    try
    {
        if (code.ParentEntry is null)
        {
            var line_number = 1;
            string decompiled_text = (code != null ? Decompiler.Decompile(code, DECOMPILE_CONTEXT.Value) : "");
            string[] splitted = decompiled_text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            bool name_written = false;
            foreach (string lineInt in splitted)
            {
                if (((regex_check && RegexContains(lineInt, keyword, case_sensitive)) || ((!regex_check && case_sensitive) ? lineInt.Contains(keyword) : lineInt.ToLower().Contains(keyword.ToLower()))))
                {
                    if (name_written == false)
                    {
                        resultsDict[code.Name.Content] = new List<string>();
                        name_written = true;
                    }
                    resultsDict[code.Name.Content].Add($"Line {line_number}: {lineInt}");
                    Interlocked.Increment(ref result_count);
                }
                line_number += 1;
            }
        }
    }
    catch (Exception e)
    {
        failedList.Add(code.Name.Content);
    }

    IncProgressP();
}
