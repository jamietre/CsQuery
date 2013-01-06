Dim $target = "\projects\drintl-trunk\libraries\CsQuery", $success =0

$success = $success + DirCopy(@WorkingDir & "\..\CsQuery",$target & "\CsQuery",1)
$success = $success + DirCopy(@WorkingDir & "\..\CsQuery.Tests",$target & "\CsQuery.Tests",1)


if $success <> 2 then
    MsgBox(0,"Failed","Did not complete successfully:" & $success)
endif

