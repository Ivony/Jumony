//----------------------------------------------------------
// Copyright (C) Microsoft Corporation. All rights reserved.
//----------------------------------------------------------
// MicrosoftMvcAjax.js

Type.registerNamespace('Sys.Mvc');Sys.Mvc.$create_AjaxOptions=function(){return {};}
Sys.Mvc.InsertionMode=function(){};Sys.Mvc.InsertionMode.prototype = {replace:0,insertBefore:1,insertAfter:2}
Sys.Mvc.InsertionMode.registerEnum('Sys.Mvc.InsertionMode',false);Sys.Mvc.AjaxContext=function(request,updateTarget,loadingElement,insertionMode){this.$3=request;this.$4=updateTarget;this.$1=loadingElement;this.$0=insertionMode;}
Sys.Mvc.AjaxContext.prototype={$0:0,$1:null,$2:null,$3:null,$4:null,get_data:function(){if(this.$2){return this.$2.get_responseData();}else{return null;}},get_insertionMode:function(){return this.$0;},get_loadingElement:function(){return this.$1;},get_object:function(){var $0=this.get_response();return ($0)?$0.get_object():null;},get_response:function(){return this.$2;},set_response:function(value){this.$2=value;return value;},get_request:function(){return this.$3;},get_updateTarget:function(){return this.$4;}}
Sys.Mvc.AsyncHyperlink=function(){}
Sys.Mvc.AsyncHyperlink.handleClick=function(anchor,evt,ajaxOptions){evt.preventDefault();Sys.Mvc.MvcHelpers.$2(anchor.href,'post','',anchor,ajaxOptions);}
Sys.Mvc.MvcHelpers=function(){}
Sys.Mvc.MvcHelpers.$0=function($p0,$p1,$p2){if($p0.disabled){return null;}var $0=$p0.name;if($0){var $1=$p0.tagName.toUpperCase();var $2=encodeURIComponent($0);var $3=$p0;if($1==='INPUT'){var $4=$3.type;if($4==='submit'){return $2+'='+encodeURIComponent($3.value);}else if($4==='image'){return $2+'.x='+$p1+'&'+$2+'.y='+$p2;}}else if(($1==='BUTTON')&&($0.length)&&($3.type==='submit')){return $2+'='+encodeURIComponent($3.value);}}return null;}
Sys.Mvc.MvcHelpers.$1=function($p0){var $0=$p0.elements;var $1=new Sys.StringBuilder();var $2=$0.length;for(var $4=0;$4<$2;$4++){var $5=$0[$4];var $6=$5.name;if(!$6||!$6.length){continue;}var $7=$5.tagName.toUpperCase();if($7==='INPUT'){var $8=$5;var $9=$8.type;if(($9==='text')||($9==='password')||($9==='hidden')||((($9==='checkbox')||($9==='radio'))&&$5.checked)){$1.append(encodeURIComponent($6));$1.append('=');$1.append(encodeURIComponent($8.value));$1.append('&');}}else if($7==='SELECT'){var $A=$5;var $B=$A.options.length;for(var $C=0;$C<$B;$C++){var $D=$A.options[$C];if($D.selected){$1.append(encodeURIComponent($6));$1.append('=');$1.append(encodeURIComponent($D.value));$1.append('&');}}}else if($7==='TEXTAREA'){$1.append(encodeURIComponent($6));$1.append('=');$1.append(encodeURIComponent(($5.value)));$1.append('&');}}var $3=$p0._additionalInput;if($3){$1.append($3);$1.append('&');}return $1.toString();}
Sys.Mvc.MvcHelpers.$2=function($p0,$p1,$p2,$p3,$p4){if($p4.confirm){if(!confirm($p4.confirm)){return;}}if($p4.url){$p0=$p4.url;}if($p4.httpMethod){$p1=$p4.httpMethod;}if($p2.length>0&&!$p2.endsWith('&')){$p2+='&';}$p2+='X-Requested-With=XMLHttpRequest';var $0=$p1.toUpperCase();var $1=($0==='GET'||$0==='POST');if(!$1){$p2+='&';$p2+='X-HTTP-Method-Override='+$0;}var $2='';if($0==='GET'||$0==='DELETE'){if($p0.indexOf('?')>-1){if(!$p0.endsWith('&')){$p0+='&';}$p0+=$p2;}else{$p0+='?';$p0+=$p2;}}else{$2=$p2;}var $3=new Sys.Net.WebRequest();$3.set_url($p0);if($1){$3.set_httpVerb($p1);}else{$3.set_httpVerb('POST');$3.get_headers()['X-HTTP-Method-Override']=$0;}$3.set_body($2);if($p1.toUpperCase()==='PUT'){$3.get_headers()['Content-Type']='application/x-www-form-urlencoded;';}$3.get_headers()['X-Requested-With']='XMLHttpRequest';var $4=null;if($p4.updateTargetId){$4=$get($p4.updateTargetId);}var $5=null;if($p4.loadingElementId){$5=$get($p4.loadingElementId);}var $6=new Sys.Mvc.AjaxContext($3,$4,$5,$p4.insertionMode);var $7=true;if($p4.onBegin){$7=$p4.onBegin($6)!==false;}if($5){Sys.UI.DomElement.setVisible($6.get_loadingElement(),true);}if($7){$3.add_completed(Function.createDelegate(null,function($p1_0){
Sys.Mvc.MvcHelpers.$3($3,$p4,$6);}));$3.invoke();}}
Sys.Mvc.MvcHelpers.$3=function($p0,$p1,$p2){$p2.set_response($p0.get_executor());if($p1.onComplete&&$p1.onComplete($p2)===false){return;}var $0=$p2.get_response().get_statusCode();if(($0>=200&&$0<300)||$0===304||$0===1223){if($0!==204&&$0!==304&&$0!==1223){var $1=$p2.get_response().getResponseHeader('Content-Type');if(($1)&&($1.indexOf('application/x-javascript')!==-1)){eval($p2.get_data());}else{Sys.Mvc.MvcHelpers.updateDomElement($p2.get_updateTarget(),$p2.get_insertionMode(),$p2.get_data());}}if($p1.onSuccess){$p1.onSuccess($p2);}}else{if($p1.onFailure){$p1.onFailure($p2);}}if($p2.get_loadingElement()){Sys.UI.DomElement.setVisible($p2.get_loadingElement(),false);}}
Sys.Mvc.MvcHelpers.updateDomElement=function(target,insertionMode,content){if(target){switch(insertionMode){case 0:target.innerHTML=content;break;case 1:if(content&&content.length>0){target.innerHTML=content+target.innerHTML.trimStart();}break;case 2:if(content&&content.length>0){target.innerHTML=target.innerHTML.trimEnd()+content;}break;}}}
Sys.Mvc.AsyncForm=function(){}
Sys.Mvc.AsyncForm.handleClick=function(form,evt){var $0=Sys.Mvc.MvcHelpers.$0(evt.target,evt.offsetX,evt.offsetY);form._additionalInput = $0;}
Sys.Mvc.AsyncForm.handleSubmit=function(form,evt,ajaxOptions){evt.preventDefault();var $0=form.validationCallbacks;if($0){for(var $2=0;$2<$0.length;$2++){var $3=$0[$2];if(!$3()){return;}}}var $1=Sys.Mvc.MvcHelpers.$1(form);Sys.Mvc.MvcHelpers.$2(form.action,form.method||'post',$1,form,ajaxOptions);}
Sys.Mvc.AjaxContext.registerClass('Sys.Mvc.AjaxContext');Sys.Mvc.AsyncHyperlink.registerClass('Sys.Mvc.AsyncHyperlink');Sys.Mvc.MvcHelpers.registerClass('Sys.Mvc.MvcHelpers');Sys.Mvc.AsyncForm.registerClass('Sys.Mvc.AsyncForm');
// ---- Do not remove this footer ----
// Generated using Script# v0.5.0.0 (http://projects.nikhilk.net)
// -----------------------------------

// SIG // Begin signature block
// SIG // MIIQTAYJKoZIhvcNAQcCoIIQPTCCEDkCAQExCzAJBgUr
// SIG // DgMCGgUAMGcGCisGAQQBgjcCAQSgWTBXMDIGCisGAQQB
// SIG // gjcCAR4wJAIBAQQQEODJBs441BGiowAQS9NQkAIBAAIB
// SIG // AAIBAAIBAAIBADAhMAkGBSsOAwIaBQAEFL+dyg1zQlak
// SIG // XJ2wvBtfhoZfn6RpoIIODzCCBBMwggNAoAMCAQICEGoL
// SIG // mU/AACKrEdsCQnwC074wCQYFKw4DAh0FADB1MSswKQYD
// SIG // VQQLEyJDb3B5cmlnaHQgKGMpIDE5OTkgTWljcm9zb2Z0
// SIG // IENvcnAuMR4wHAYDVQQLExVNaWNyb3NvZnQgQ29ycG9y
// SIG // YXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUZXN0IFJv
// SIG // b3QgQXV0aG9yaXR5MB4XDTA2MDYyMjIyNTczMVoXDTEx
// SIG // MDYyMTA3MDAwMFowcTELMAkGA1UEBhMCVVMxEzARBgNV
// SIG // BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQx
// SIG // HjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEb
// SIG // MBkGA1UEAxMSTWljcm9zb2Z0IFRlc3QgUENBMIIBIjAN
// SIG // BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAj/Pz33qn
// SIG // cihhfpDzgWdPPEKAs8NyTe9/EGW4StfGTaxnm6+j/cTt
// SIG // fDRsVXNecQkcoKI69WVT1NzP8zOjWjMsV81IIbelJDAx
// SIG // UzWp2tnbdH9MLnhnzdvJ7bGPt67/eW+sIwZUDiNDN3jd
// SIG // Pk4KbdAq9sZ+W5J0DbMTD1yxcbQQ/LEgCAgueW5f0nI0
// SIG // rpI6gbAyrM5DWTCmwfyu+MzofYZrXK7r3pX6Kjl1BlxB
// SIG // OlHcVzVOksssnXuk3Jrp/iGcYR87pEx/UrGFOWR9kYlv
// SIG // nhRCs7yi2moXhyTmG9V8fY+q3ALJoV7d/YEqnybDNkHT
// SIG // z/xzDRx0KDjypQrF0Q+7077QkwIDAQABo4HrMIHoMIGo
// SIG // BgNVHQEEgaAwgZ2AEMBjRdejAX15xXp6XyjbQ9ahdzB1
// SIG // MSswKQYDVQQLEyJDb3B5cmlnaHQgKGMpIDE5OTkgTWlj
// SIG // cm9zb2Z0IENvcnAuMR4wHAYDVQQLExVNaWNyb3NvZnQg
// SIG // Q29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBU
// SIG // ZXN0IFJvb3QgQXV0aG9yaXR5ghBf6k/S8h1DELboVD7Y
// SIG // lSYYMA8GA1UdEwEB/wQFMAMBAf8wHQYDVR0OBBYEFFSl
// SIG // IUygrm+cYE4Pzt1G1ddh1hesMAsGA1UdDwQEAwIBhjAJ
// SIG // BgUrDgMCHQUAA4HBACzODwWw7h9lGeKjJ7yc936jJard
// SIG // LMfrxQKBMZfJTb9MWDDIJ9WniM6epQ7vmTWM9Q4cLMy2
// SIG // kMGgdc3mffQLETF6g/v+aEzFG5tUqingK125JFP57MGc
// SIG // JYMlQGO3KUIcedPC8cyj+oYwi6tbSpDLRCCQ7MAFS15r
// SIG // 4Dnxn783pZ5nSXh1o+NrSz5mbGusDIj0ujHBCqblI96+
// SIG // Rk7oVQ2DI3oQkSmGQf+BrmRXoJfB3YuXXFc+F88beLHS
// SIG // F0S8oJhPjzCCBKgwggOQoAMCAQICCmEBi3MAAAAAABMw
// SIG // DQYJKoZIhvcNAQEFBQAwcTELMAkGA1UEBhMCVVMxEzAR
// SIG // BgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1v
// SIG // bmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlv
// SIG // bjEbMBkGA1UEAxMSTWljcm9zb2Z0IFRlc3QgUENBMB4X
// SIG // DTA5MDgxNzIzMjAxN1oXDTExMDYyMTA3MDAwMFowgYEx
// SIG // EzARBgoJkiaJk/IsZAEZFgNjb20xGTAXBgoJkiaJk/Is
// SIG // ZAEZFgltaWNyb3NvZnQxFDASBgoJkiaJk/IsZAEZFgRj
// SIG // b3JwMRcwFQYKCZImiZPyLGQBGRYHcmVkbW9uZDEgMB4G
// SIG // A1UEAxMXTVNJVCBUZXN0IENvZGVTaWduIENBIDEwggEi
// SIG // MA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDKz+fW
// SIG // ilZnvB1mb2XQEkuK0GeO6we7n8RfXKMFTp9ifiOnD0v5
// SIG // FFYrPjAKGMrOxroVu8rPTOPukz6hlYdMzIkV68iyS4FU
// SIG // ZjQGz5wQNnLKbUN1PFlP+NsWJZjzvRuZv9WWweCKnUeE
// SIG // Fxur+rzMtvz50aVAechNt36xI6rIxVXRv5xvDKzkKTGv
// SIG // BmaP0YsqkNcUe3GJy17yWoEWX+kKGX69xNezEai06On2
// SIG // cpKToU0ibyRNhgs2Ygzb5U/9hISMYt7YFdEYggL0zTNp
// SIG // 59hmfaB5FT0yMor1iUcSFVtTGObPmB1dsD4EPcYSTZtp
// SIG // 5R4hzYecLp8kSV78s1ycVDt5pQY1AgMBAAGjggEvMIIB
// SIG // KzAQBgkrBgEEAYI3FQEEAwIBADAdBgNVHQ4EFgQUhOTQ
// SIG // p5jIj+9WN5bdvfFGrMW5xZ4wGQYJKwYBBAGCNxQCBAwe
// SIG // CgBTAHUAYgBDAEEwCwYDVR0PBAQDAgGGMA8GA1UdEwEB
// SIG // /wQFMAMBAf8wHwYDVR0jBBgwFoAUVKUhTKCub5xgTg/O
// SIG // 3UbV12HWF6wwTAYDVR0fBEUwQzBBoD+gPYY7aHR0cDov
// SIG // L2NybC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVj
// SIG // dHMvbGVnYWN5dGVzdHBjYS5jcmwwUAYIKwYBBQUHAQEE
// SIG // RDBCMEAGCCsGAQUFBzAChjRodHRwOi8vd3d3Lm1pY3Jv
// SIG // c29mdC5jb20vcGtpL2NlcnRzL0xlZ2FjeVRlc3RQQ0Eu
// SIG // Y3J0MA0GCSqGSIb3DQEBBQUAA4IBAQA4GSVNJtByD1os
// SIG // xEzGCLI18ykM+RrR02D1DyopRstCY+OoOeX5WX5BVknd
// SIG // j0w6P1Ea4TD450ozSN7q1yWQcgIT2K8DbwyKTnDn5enx
// SIG // josg2n+ljxnLputPDiFAdfNP+XHew9x/gB+JR7oSK/Ps
// SIG // LzXbuVITIRDkPogIJUFQMrwKI9o0bv2sLWV+fSk+fEXB
// SIG // OaHysKBGU+EIhjrfHx4QP38jQUi2yJZQ85klqVVuSL21
// SIG // dwIP5QZiYplN6zicK6ez3r+yozQLOg6mc5MBgrUPBTsV
// SIG // sSbHM2+BGVaorOyI7JMr0sHBl6IGFbqRIPqtWY4rimD8
// SIG // uNi6hHfLJFmTMDstbNJEMIIFSDCCBDCgAwIBAgIKa4DO
// SIG // qQAAAACmmDANBgkqhkiG9w0BAQUFADCBgTETMBEGCgmS
// SIG // JomT8ixkARkWA2NvbTEZMBcGCgmSJomT8ixkARkWCW1p
// SIG // Y3Jvc29mdDEUMBIGCgmSJomT8ixkARkWBGNvcnAxFzAV
// SIG // BgoJkiaJk/IsZAEZFgdyZWRtb25kMSAwHgYDVQQDExdN
// SIG // U0lUIFRlc3QgQ29kZVNpZ24gQ0EgMTAeFw0wOTExMDYx
// SIG // ODE3MDdaFw0xMDExMDYxODE3MDdaMBUxEzARBgNVBAMT
// SIG // ClZTIEJsZCBMYWIwgZ8wDQYJKoZIhvcNAQEBBQADgY0A
// SIG // MIGJAoGBAJMiPNeJy8vp5oeABJLebUDw5LUKy+N3pOFp
// SIG // h5QGJmE4b4JgN2LEXNVLh6lOle35xLCbQOJCVs1eDOgq
// SIG // puOWq5EvFYOugrxGcS4wfHNt4/Rwjigo/UQDYU755puL
// SIG // RBqLVtGqlcMYwLhzAWV0R7HWtmBDfhqAH19O3P3foI2X
// SIG // zrLrAgMBAAGjggKvMIICqzALBgNVHQ8EBAMCB4AwHQYD
// SIG // VR0OBBYEFAjpDmzPyPih2x+qdItA5Ul2ZAe3MD0GCSsG
// SIG // AQQBgjcVBwQwMC4GJisGAQQBgjcVCIPPiU2t8gKFoZ8M
// SIG // gvrKfYHh+3SBT4KusGqH9P0yAgFkAgEMMB8GA1UdIwQY
// SIG // MBaAFITk0KeYyI/vVjeW3b3xRqzFucWeMIHoBgNVHR8E
// SIG // geAwgd0wgdqggdeggdSGNmh0dHA6Ly9jb3JwcGtpL2Ny
// SIG // bC9NU0lUJTIwVGVzdCUyMENvZGVTaWduJTIwQ0ElMjAx
// SIG // LmNybIZNaHR0cDovL21zY3JsLm1pY3Jvc29mdC5jb20v
// SIG // cGtpL21zY29ycC9jcmwvTVNJVCUyMFRlc3QlMjBDb2Rl
// SIG // U2lnbiUyMENBJTIwMS5jcmyGS2h0dHA6Ly9jcmwubWlj
// SIG // cm9zb2Z0LmNvbS9wa2kvbXNjb3JwL2NybC9NU0lUJTIw
// SIG // VGVzdCUyMENvZGVTaWduJTIwQ0ElMjAxLmNybDCBqQYI
// SIG // KwYBBQUHAQEEgZwwgZkwQgYIKwYBBQUHMAKGNmh0dHA6
// SIG // Ly9jb3JwcGtpL2FpYS9NU0lUJTIwVGVzdCUyMENvZGVT
// SIG // aWduJTIwQ0ElMjAxLmNydDBTBggrBgEFBQcwAoZHaHR0
// SIG // cDovL3d3dy5taWNyb3NvZnQuY29tL3BraS9tc2NvcnAv
// SIG // TVNJVCUyMFRlc3QlMjBDb2RlU2lnbiUyMENBJTIwMS5j
// SIG // cnQwHwYDVR0lBBgwFgYKKwYBBAGCNwoDBgYIKwYBBQUH
// SIG // AwMwKQYJKwYBBAGCNxUKBBwwGjAMBgorBgEEAYI3CgMG
// SIG // MAoGCCsGAQUFBwMDMDoGA1UdEQQzMDGgLwYKKwYBBAGC
// SIG // NxQCA6AhDB9kbGFiQHJlZG1vbmQuY29ycC5taWNyb3Nv
// SIG // ZnQuY29tMA0GCSqGSIb3DQEBBQUAA4IBAQBqcp669vuu
// SIG // QzcKv0NTjeY2jhqSYRlwon/Q83ON8GCb1vf3AEFmwPNI
// SIG // 5hxSmGpqr4JrfuJFFa6SxO8praB4oaZeTKt7bAH/uRpb
// SIG // HP3U8Y6tuJAzfWaYUiNoF02lpgFEa44pw3sGJ3XA6uj0
// SIG // cG4jo1U5b81pkFblA4WRIuU1VHUDmARJbinQVt3JAFyU
// SIG // /J4SuAMUxraGUS8voUpk/Jyy8A7dhNepQQmc8BlY6lIQ
// SIG // fyU6WYQhOSuuQO5mfZhJaFGA53gqWzJfVBD32i7O6lAt
// SIG // /SXE7oV+Fwo5FHC8dOMzIn4bITvDQxgfO0M530uBmnCY
// SIG // qsRRYNNgYql6JvUjP/DSy6ZfMYIBqTCCAaUCAQEwgZAw
// SIG // gYExEzARBgoJkiaJk/IsZAEZFgNjb20xGTAXBgoJkiaJ
// SIG // k/IsZAEZFgltaWNyb3NvZnQxFDASBgoJkiaJk/IsZAEZ
// SIG // FgRjb3JwMRcwFQYKCZImiZPyLGQBGRYHcmVkbW9uZDEg
// SIG // MB4GA1UEAxMXTVNJVCBUZXN0IENvZGVTaWduIENBIDEC
// SIG // CmuAzqkAAAAAppgwCQYFKw4DAhoFAKBwMBAGCisGAQQB
// SIG // gjcCAQwxAjAAMBkGCSqGSIb3DQEJAzEMBgorBgEEAYI3
// SIG // AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEV
// SIG // MCMGCSqGSIb3DQEJBDEWBBRNK3Gfie0nnYwwXX0L9yA3
// SIG // H8n/JjANBgkqhkiG9w0BAQEFAASBgGhh/Xi5VODB8lXb
// SIG // /iYj0nHWqJ0UmW10auNUwRtrslNLSaL4cIApO/xBRS+G
// SIG // 2HfjOdCbSpxNDLwi37gfViTh8rM/pEMCArmEJim/5w/n
// SIG // bOc8ytMUIJ+xJOnSe9x+rveFkcxV/UyxIAQc85yamIxH
// SIG // iqVJIbUKGf3Ak3LaYBzIFIMY
// SIG // End signature block
