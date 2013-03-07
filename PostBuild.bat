DEL "%TargetDir%Code.RAR"
"%ProgramFiles%\WinRAR\RAR.exe" a -cfg- -inul -k -ep1 -m5 -rr -x"%TargetDir%" -x*.pfx -x*.dll -x*.pdb -x*.exe -x.svn "%TargetDir%Code.RAR" "%ProjectDir%..\%ProjectName%"
"%ProgramFiles%\WinRAR\RAR.exe" a -cfg- -inul -k -ep1 -m5 -rr -ag_yyyy.mm.dd_n -ms*.rar -x*.tmp -x*.pdb -x*.pfx -x.svn C:\Assembly\%ProjectName%_%ConfigurationName%.RAR "%TargetDir%*.*"
DEL "%TargetDir%Code.RAR"

cd "%ProjectDir%"

"c:\Program Files\CollabNet\Subversion Client\SvnCommitHelper.exe"

IF EXIST PostBuild.bat ( call PostBuild.bat )