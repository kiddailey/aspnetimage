'---------------------------------------------------------------------------------'
' SearchProgID                                                                    '
' Finds what DLL is associated with a given ProgID searching the registry         '
'---------------------------------------------------------------------------------'
Option Explicit

Dim progID, clsidPath, oReg, progidPath, clsid, dllPath, threadModel, threading

'--- Display usage information if arguments are incorrect
If WScript.Arguments.Count <> 1 Then
	WScript.Echo "Usage: searchprogid.vbs [ProgID]"
	WScript.Echo "Example: searchprogid.vbs AspImage.Image"
	WScript.Quit
End If

'--- Read the command line argument (progID)
progID = WScript.Arguments(0)

'--- Here's the registry path where we'll look up the CLSID which corresponds to the ProgID
clsidPath = "HKEY_CLASSES_ROOT\" & progID & "\CLSID\"

'--- Create a WScript.Shell object
Set oReg = WScript.CreateObject("WScript.Shell")

'--- Read the CLSID from the registry
'--- If it doesn't exist, it will fail
On Error Resume Next
clsid = oReg.RegRead(clsidPath)
If Err.Number <> 0 Then
	WScript.Echo "Error: Cannot open registry for speficied ProgID."
	WScript.Echo "Error: Please recheck your ProgID."
	WScript.Quit
End If


'--- Build the registry path to the CLSID section of HKCR using the value we just got
progidPath = "HKEY_CLASSES_ROOT\CLSID\" & clsid & "\InProcServer32\"
threadModel = "HKEY_CLASSES_ROOT\CLSID\" & clsid & "\InProcServer32\ThreadingModel"

'--- Query the value of InProcServer32 to get the registered DLL location
dllPath = oReg.RegRead(progidPath)
If Err.Number <> 0 Then
	WScript.Echo "Error: Cannot open registry for speficied ProgID."
	WScript.Echo "Error: Please recheck your ProgID."
	WScript.Quit
End If
On Error Goto 0

'--- Query the value of ThreadingModel
threading = oReg.RegRead(threadModel)
If Err.Number <> 0 Then
	WScript.Echo "Error: Cannot open registry for speficied ProgID."
	WScript.Echo "Error: Please recheck your ProgID."
	WScript.Quit
End If
On Error Goto 0

'--- Display the information
WScript.Echo "ProgID: "  & progID & vbCrLf & "DLL:    " & dllPath & " ThreadingModel: " & threading

Set oReg = Nothing