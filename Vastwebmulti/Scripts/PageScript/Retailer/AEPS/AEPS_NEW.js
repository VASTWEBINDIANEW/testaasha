$(document).ready(function () {
    //document.getElementById("loadermoney").style.display = "block";
});
//////////////////////////////////////////
var DeviceKonsi = '';
var RdStatus = false;//AJ
var ddlAVDM = '';
var txtDeviceInfo = '';
var txtPidData = '';
var txtPidOptions = '';
var Timeout = 10000;
var Pidver = '2.0';
var Env = 'P';//S,P
var pTimeout = 20000;
var pgCount = 2;
var Dtype = 0;//0 for 'X' ,1 for 'P'
var txtCK = '';//ClientKey if required;
var txtotp = '';
var txtWadh = '';
var Fcount = 1; //Finger Count finger count rage from 0 to 10
var Icount = 0;//Iris Count 0 to 3
var Pcount = 0;//Face Count 0 or 1
var Ftype = 2;//Finger Type 0 for FMR and 1 for FIR
var Itype = ''; //Iris Type May be ISO
var Ptype = '';//Face Type
//////////////////////////////////////
var GetPIString = '';
var GetPAString = '';
var GetPFAString = '';
var DemoFinalString = '';

var select = '';
select += '<option val=0>Select</option>';
for (i = 1; i <= 100; i++) {
    select += '<option val=' + i + '>' + i + '</option>';
}
//$('#drpMatchValuePI').html(select); //AJ
//$('#drpMatchValuePFA').html(select);//AJ
//$('#drpLocalMatchValue').html(select);//AJ
//$('#drpLocalMatchValuePI').html(select);//AJ

var finalUrl = "";
var MethodInfo = "";
var MethodCapture = "";
var OldPort = false;

function test() {
    alert("I am calling..");
}

function reset() {
    $('#txtWadh').val('');
    //$('#txtDeviceInfo').val('');
    //$('#txtPidOptions').val('');
    //$('#txtPidData').val('');
    // $("select#ddlAVDM").prop('selectedIndex', 0);
    $("select#Timeout").prop('selectedIndex', 0);
    $("select#Icount").prop('selectedIndex', 0);
    $("select#Fcount").prop('selectedIndex', 0);
    $("select#Icount").prop('selectedIndex', 0);
    $("select#Itype").prop('selectedIndex', 0);
    $("select#Ptype").prop('selectedIndex', 0);
    $("select#Ftype").prop('selectedIndex', 0);
    $("select#Dtype").prop('selectedIndex', 0);
    $('#txtotp').val('');
    $("select#pTimeout").prop('selectedIndex', 1);
    $("select#pgCount").prop('selectedIndex', 1);
}
// All New Function

function Demo() {

    var GetPIStringstr = '';
    var GetPAStringstr = '';
    var GetPFAStringstr = '';

    if (GetPI() == true) {
        GetPIStringstr = '<Pi ' + GetPIString + ' />';
        //alert(GetPIStringstr);
    }
    else {
        GetPIString = '';
    }

    if (GetPA() == true) {
        GetPAStringstr = '<Pa ' + GetPAString + ' />';
        //alert(GetPAStringstr);
    }
    else {
        GetPAString = '';
    }

    if (GetPFA() == true) {
        GetPFAStringstr = '<Pfa ' + GetPFAString + ' />';
        //alert(GetPFAStringstr);
    }
    else {
        GetPFAString = '';
    }

    if (GetPI() == false && GetPA() == false && GetPFA() == false) {
        //alert("Fill Data!");
        DemoFinalString = '';
    }
    else {
        DemoFinalString = '<Demo>' + GetPIStringstr + ' ' + GetPAStringstr + ' ' + GetPFAStringstr + ' </Demo>';
        //alert(DemoFinalString)
    }
}

function GetPI() {
    var Flag = false;
    GetPIString = '';

    //    if ($("#txtName").val().trim().length > 0)
    //    {
    //        Flag = true;
    //        GetPIString += "name="+ "\""+$("#txtName").val().trim()+"\"";
    //    }

    //    if ($("#drpMatchValuePI").val() > 0 && Flag)
    //    {
    //        Flag = true;
    //        GetPIString += " mv="+ "\""+$("#drpMatchValuePI").val().trim()+"\"";
    //    }

    //    if ($('#rdExactPI').is(':checked') && Flag)
    //    {
    //        Flag = true;
    //        GetPIString += " ms="+ "\"E\"";
    //    }
    //    else if ($('#rdPartialPI').is(':checked') && Flag)
    //    {
    //        Flag = true;
    //        GetPIString += " ms="+ "\"P\"";
    //    }
    //    else if ($('#rdFuzzyPI').is(':checked') && Flag)
    //    {
    //        Flag = true;
    //        GetPIString += " ms="+ "\"F\"";
    //    }
    //    if ($("#txtLocalNamePI").val().trim().length > 0)
    //    {
    //        Flag = true;
    //        GetPIString += " lname="+ "\""+$("#txtLocalNamePI").val().trim()+"\"";
    //    }

    //    if ($("#txtLocalNamePI").val().trim().length > 0 && $("#drpLocalMatchValuePI").val() > 0)
    //    {
    //        Flag = true;
    //        GetPIString += " lmv="+ "\""+$("#drpLocalMatchValuePI").val().trim()+"\"";
    //    }

    //    // if ($("#drpGender").val() > 0) 
    //     //{ 

    //        if ($("#drpGender").val().trim() == "MALE")
    //        {
    //            Flag = true;
    //             GetPIString += " gender="+ "\"M\"";
    //}
    //else if ($("#drpGender").val().trim() == "FEMALE")
    //{
    //    Flag = true;
    //    GetPIString += " gender="+ "\"F\"";
    //}
    //else if ($("#drpGender").val().trim() == "TRANSGENDER")
    //{
    //    Flag = true;
    //    GetPIString += " gender="+ "\"T\"";
    //}
    ////}
    //if ($("#txtDOB").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPIString += " dob="+ "\""+$("#txtDOB").val().trim()+"\"";
    //}

    //if ($("#drpDOBType").val() != "0")
    //{
    //    Flag = true;
    //    GetPIString += " dobt="+ "\""+$("#drpDOBType").val().trim()+"\"";
    //}

    //if ($("#txtAge").val().trim().length)
    //{
    //    Flag = true;
    //    GetPIString += " age="+ "\""+$("#txtAge").val().trim()+"\"";
    //}

    //if ($("#txtPhone").val().trim().length > 0 || $("#txtEmail").val().trim().length > 0)
    //{
    //    Flag = true;
    //    GetPIString += " phone="+ "\""+$("#txtPhone").val().trim()+"\"";
    //}
    //if ($("#txtEmail").val().trim().length > 0)
    //{
    //    Flag = true;
    //    GetPIString += " email="+ "\""+$("#txtEmail").val().trim()+"\"";
    //}

    //alert(GetPIString);
    return Flag;
}

function GetPA() {
    var Flag = false;
    GetPAString = '';

    //if ($("#txtCareOf").val().trim().length > 0)
    //{
    //    Flag = true;
    //    GetPAString += "co="+ "\""+$("#txtCareOf").val().trim()+"\"";
    //}
    //if ($("#txtLandMark").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPAString += " lm="+ "\""+$("#txtLandMark").val().trim()+"\"";
    //}
    //if ($("#txtLocality").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPAString += " loc="+ "\""+$("#txtLocality").val().trim()+"\"";
    //}
    //if ($("#txtCity").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPAString += " vtc="+ "\""+$("#txtCity").val().trim()+"\"";
    //}
    //if ($("#txtDist").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPAString += " dist="+ "\""+$("#txtDist").val().trim()+"\"";
    //}
    //if ($("#txtPinCode").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPAString += " pc="+ "\""+$("#txtPinCode").val().trim()+"\"";
    //}
    //if ($("#txtBuilding").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPAString += " house="+ "\""+$("#txtBuilding").val().trim()+"\"";
    //}
    //if ($("#txtStreet").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPAString += " street="+ "\""+$("#txtStreet").val().trim()+"\"";
    //}
    //if ($("#txtPOName").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPAString += " po="+ "\""+$("#txtPOName").val().trim()+"\"";
    //}
    //if ($("#txtSubDist").val().trim().length > 0 )
    //{
    //    Flag = true;
    //    GetPAString += " subdist="+ "\""+$("#txtSubDist").val().trim()+"\"";
    //}
    //if ($("#txtState").val().trim().length > 0)
    //{
    //    Flag = true;
    //    GetPAString += " state="+ "\""+$("#txtState").val().trim()+"\"";
    //}
    //if ( $('#rdMatchStrategyPA').is(':checked') && Flag)
    //{
    //    Flag = true;
    //    GetPAString += " ms="+ "\"E\"";
    //}
    //alert(GetPIString);
    return Flag;
}

function GetPFA() {
    var Flag = false;
    GetPFAString = '';

    //if ($("#txtAddressValue").val().trim().length > 0)
    //{
    //    Flag = true;
    //    GetPFAString += "av="+ "\""+$("#txtAddressValue").val().trim()+"\"";
    //}

    //if ($("#drpMatchValuePFA").val() > 0 && $("#txtAddressValue").val().trim().length > 0)
    //{
    //    Flag = true;
    //    GetPFAString += " mv="+ "\""+$("#drpMatchValuePFA").val().trim()+"\"";
    //}

    //if ($('#rdExactPFA').is(':checked') && Flag)
    //{
    //    Flag = true;
    //    GetPFAString += " ms="+ "\"E\"";
    //}
    //else if ($('#rdPartialPFA').is(':checked') && Flag)
    //{
    //    Flag = true;
    //    GetPFAString += " ms="+ "\"P\"";
    //}
    //else if ($('#rdFuzzyPFA').is(':checked') && Flag)
    //{
    //    Flag = true;
    //    GetPFAString += " ms="+ "\"F\"";
    //}

    //if ($("#txtLocalAddress").val().trim().length > 0)
    //{
    //    Flag = true;
    //    GetPFAString += " lav="+ "\""+$("#txtLocalAddress").val().trim()+"\"";
    //}

    //if ($("#drpLocalMatchValue").val() > 0 && $("#txtLocalAddress").val().trim().length > 0)
    //{
    //    Flag = true;
    //    GetPFAString += " lmv="+ "\""+$("#drpLocalMatchValue").val().trim()+"\"";
    //}
    //alert(GetPIString);
    return Flag;
}

$("#ddlAVDM").change(function () {
    //alert($("#ddlAVDM").val());
    discoverAvdmFirstNode($("#ddlAVDM").val());
});

$("#chkHttpsPort").change(function () {
    if ($("#chkHttpsPort").prop('checked') == true) {
        OldPort = true;
    }
    else {
        OldPort = false;
    }

});

function discoverAvdmFirstNode(PortNo) {
    $('#txtWadh').val('');
    //$('#txtDeviceInfo').val('');
    txtDeviceInfo = '';
    //$('#txtPidOptions').val('');
    txtPidOptions = '';
    // $('#txtPidData').val('');
    txtPidData = '';
    //alert(PortNo);
    var primaryUrl = "http://127.0.0.1:";
    try {
        var protocol = window.location.href;
        if (protocol.indexOf("https") >= 0) {
            primaryUrl = "https://127.0.0.1:";
        }
    } catch (e) { }
    url = "";
    var verb = "RDSERVICE";
    var err = "";
    var res;
    $.support.cors = true;
    var httpStaus = false;
    var jsonstr = "";
    var data = new Object();
    var obj = new Object();

    $.ajax({
        type: "RDSERVICE",
        async: false,
        crossDomain: true,
        url: primaryUrl + PortNo,
        contentType: "text/xml; charset=utf-8",
        processData: false,
        cache: false,
        async: false,
        crossDomain: true,
        beforeSend: function () {
            $("#loadaeps").show();
        },
        success: function (data) {
            httpStaus = true;
            res = { httpStaus: httpStaus, data: data };
            //alert(data);
            //
            //$("#txtDeviceInfo").val(data); AJ
            txtDeviceInfo = data;
            var $doc = $.parseXML(data);
            //alert($($doc).find('Interface').eq(1).attr('path'));
            if ($($doc).find('Interface').eq(0).attr('path') == "/rd/capture") {
                MethodCapture = $($doc).find('Interface').eq(0).attr('path');
            }
            if ($($doc).find('Interface').eq(1).attr('path') == "/rd/capture") {
                MethodCapture = $($doc).find('Interface').eq(1).attr('path');
            }

            if ($($doc).find('Interface').eq(0).attr('path') == "/rd/info") {
                MethodInfo = $($doc).find('Interface').eq(0).attr('path');
            }
            if ($($doc).find('Interface').eq(1).attr('path') == "/rd/info") {
                MethodInfo = $($doc).find('Interface').eq(1).attr('path');
            }
            // MethodInfo=$($doc).find('Interface').eq(0).attr('path'); 
            // MethodCapture=$($doc).find('Interface').eq(1).attr('path'); 
            $("#response1").text("RDSERVICE Discover Successfully");


        },
        error: function (jqXHR, ajaxOptions, thrownError) {
            //$('#txtDeviceInfo').val(""); AJ
            txtDeviceInfo = '';
            //alert(thrownError);
            res = { httpStaus: httpStaus, err: getHttpError(jqXHR) };
        },

    });

    return res;
}

function discoverAvdm() {
    // openNav();
    var SuccessFlag = 0;

    var primaryUrl = "http://127.0.0.1:";
    try {
        var protocol = window.location.href;
        if (protocol.indexOf("https") >= 0) {
            primaryUrl = "https://127.0.0.1:";
        }
    } catch (e) { }
    url = "";
    //$("#ddlAVDM").empty();
    ddlAVDM = '';
    //alert("Please wait while discovering port from 11100 to 11120.\nThis will take some time.");
    for (var i = 11100; i <= 11120; i++) {
        if (primaryUrl == "https://127.0.0.1:" && OldPort == true) {
            i = "8005";
        }
        //$("#lblStatus1").text("Discovering RD service on port : " + i.toString());
        //alert();
        var verb = "RDSERVICE";
        var err = "";
        SuccessFlag = 0;
        var res;
        $.support.cors = true;
        var httpStaus = false;
        var jsonstr = "";
        var data = new Object();
        var obj = new Object();
        //   console.log(primaryUrl + i.toString());
        $.ajax({
            type: "RDSERVICE",
            async: false,
            crossDomain: true,
            url: primaryUrl + i.toString(),
            contentType: "text/xml; charset=utf-8",
            processData: false,
            cache: false,
            crossDomain: true,
            success: function (data, status, xhr) {
                //  console.log("success()");
                var ct = xhr.getResponseHeader("content-type") || "";
                //  console.log(ct);
                httpStaus = true;
                res = { httpStaus: httpStaus, data: data };
                // console.log(data);
                finalUrl = primaryUrl + i.toString();
                // console.log("1");
                var ResponseDatatype = typeof data;
                //  console.log("ResponseDatatype : " + ResponseDatatype);
                var $doc;
                if (ResponseDatatype == 'string') {
                    $doc = $.parseXML(data);
                } else {
                    $doc = data;
                }

                //if (ct.indexOf('TEXT/XML') > -1) {
                //    console.log('TEXT/XML response');
                //}

                var status = $($doc).find('RDService').attr('status');

                //var $doc = $.parseXML(data);
                //console.log("2");
                //console.log($doc);
                //console.log($($doc).find('RDService'));
                //console.log($($doc).find('RDService').attr('status'));
                //console.log("3");
                var CmbData1 = $($doc).find('RDService').attr('status');
                var CmbData2 = $($doc).find('RDService').attr('info');
                //    console.log("CmbData1 " + CmbData1);
                //    console.log("CmbData2 " + CmbData2);

                if (CmbData1 == "READY") {
                    if (RegExp('\\b' + 'Mantra' + '\\b').test(CmbData2) == true) {
                        DeviceKonsi = 'Mantra';
                        //closeNav();
                        //      console.log("Mantra found");
                        //$("#txtDeviceInfo").val(data); aj
                        txtDeviceInfo = data;
                        if ($($doc).find('Interface').eq(0).attr('path') == "/rd/capture") {
                            MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                        }
                        if ($($doc).find('Interface').eq(1).attr('path') == "/rd/capture") {
                            MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                        }
                        if ($($doc).find('Interface').eq(0).attr('path') == "/rd/info") {
                            MethodInfo = $($doc).find('Interface').eq(0).attr('path');
                        }
                        if ($($doc).find('Interface').eq(1).attr('path') == "/rd/info") {
                            MethodInfo = $($doc).find('Interface').eq(1).attr('path');
                        }
                        ddlAVDM = i.toString();
                        SuccessFlag = 1;
                        $("#response1").text("RDSERVICE Discover Successfully");

                        //alert("RDSERVICE Discover Successfully");
                        RdStatus = true;
                        return;
                    }
                    else if (RegExp('\\b' + 'Startek' + '\\b').test(CmbData2) == true || RegExp('\\b' + 'L1 provided by Access Computech' + '\\b').test(CmbData2) == true) {

                        DeviceKonsi = 'Startek';
                        //  closeNav();
                        //  console.log("Startek found");
                        //$("#txtDeviceInfo").val(data); aj
                        txtDeviceInfo = data;
                        if ($($doc).find('Interface').eq(0).attr('path') == "/rd/capture") {
                            MethodCapture = $($doc).find('Interface').eq(0).attr('path') + '?ts=' + Date.now();
                        }
                        if ($($doc).find('Interface').eq(1).attr('path') == "/rd/capture") {
                            MethodCapture = $($doc).find('Interface').eq(1).attr('path') + '?ts=' + Date.now();
                        }
                        if ($($doc).find('Interface').eq(0).attr('path') == "/rd/info") {
                            MethodInfo = $($doc).find('Interface').eq(0).attr('path');
                        }
                        if ($($doc).find('Interface').eq(1).attr('path') == "/rd/info") {
                            MethodInfo = $($doc).find('Interface').eq(1).attr('path');
                        }
                        ddlAVDM = i.toString();
                        SuccessFlag = 1;
                        $("#response1").text("RDSERVICE Discover Successfully");

                        //alert("RDSERVICE Discover Successfully");
                        RdStatus = true;
                        return;
                    }
                    else if (RegExp('\\b' + 'IDEMIA').test(CmbData2) == true) {
                        // closeNav();
                        DeviceKonsi = 'Morpho';
                        //console.log("Morpho found");
                        txtDeviceInfo = data;
                        //console.log("/127.0.0.1:" + i.toString() + "/capture");
                        //console.log("/127.0.0.1:" + i.toString() + "/getDeviceInfo");
                        if ($($doc).find('Interface').eq(0).attr('path') == "/127.0.0.1:" + i.toString() + "/capture") {
                            //console.log("x");
                            MethodCapture = $($doc).find('Interface').eq(0).attr('path');
                        }
                        if ($($doc).find('Interface').eq(1).attr('path') == "/127.0.0.1:" + i.toString() + "/capture") {
                            //console.log("X");
                            MethodCapture = $($doc).find('Interface').eq(1).attr('path');
                        }
                        if ($($doc).find('Interface').eq(0).attr('path') == "/127.0.0.1:" + i.toString() + "/getDeviceInfo") {
                            //console.log("y");
                            MethodInfo = $($doc).find('Interface').eq(0).attr('path');
                        }
                        if ($($doc).find('Interface').eq(1).attr('path') == "/127.0.0.1:" + i.toString() + "/getDeviceInfo") {
                            //console.log("Y");
                            MethodInfo = $($doc).find('Interface').eq(1).attr('path');
                        }
                        ddlAVDM = i.toString();
                        SuccessFlag = 1;
                        $("#response1").text("RDSERVICE Discover Successfully");

                        //alert("RDSERVICE Discover Successfully");
                        RdStatus = true;
                        return;
                    }
                }
            },
            error: function (jqXHR, ajaxOptions, thrownError) {
                console.log("error()");
                console.log(thrownError);
                if (i == "8005" && OldPort == true) {
                    OldPort = false;
                    i = "11099";
                }
                //$('#txtDeviceInfo').val(""); AJ
                txtDeviceInfo = '';
                //alert(thrownError);

                //res = { httpStaus: httpStaus, err: getHttpError(jqXHR) };
            }
        });
        if (SuccessFlag == 1) {
            break;
        }
    }
    if (SuccessFlag == 0) {
        $("#deviceerror2").text("➀ Connection failed Please try again.");

        $("#Devicerror").show();
    }
    // closeNav();
    return res;
}

function openNav() {
    document.getElementById("myNav").style.width = "100%";
}

function closeNav() {
    document.getElementById("myNav").style.width = "0%";
}

function deviceInfoAvdm() {
    //alert($("#ddlAVDM").val());
    // ddlAVDM.empty(); 
    url = "";
    // alert(i.toString()); 
    // $("#lblStatus").text("Discovering RD Service on Port : " + i.toString());
    //Dynamic URL

    //finalUrl = "http://127.0.0.1:" + $("#ddlAVDM").val(); AJ
    finalUrl = "http://127.0.0.1:" + ddlAVDM;

    try {
        var protocol = window.location.href;
        if (protocol.indexOf("https") >= 0) {
            //finalUrl = "https://127.0.0.1:" + $("#ddlAVDM").val(); AJ
            finalUrl = "https://127.0.0.1:" + ddlAVDM;
        }
    } catch (e) { }

    //
    var verb = "DEVICEINFO";
    //alert(finalUrl);

    var err = "";

    var res;
    $.support.cors = true;
    var httpStaus = false;
    var jsonstr = "";
    ;
    $.ajax({

        type: "DEVICEINFO",
        async: false,
        crossDomain: true,
        url: finalUrl + MethodInfo,
        contentType: "text/xml; charset=utf-8",
        processData: false,
        success: function (data) {
            //alert(data);
            httpStaus = true;
            res = { httpStaus: httpStaus, data: data };

            //$('#txtDeviceInfo').val(data);AJ
            txtDeviceInfo = data;
        },
        error: function (jqXHR, ajaxOptions, thrownError) {
            alert(thrownError);
            res = { httpStaus: httpStaus, err: getHttpError(jqXHR) };
        },
    });

    return res;

}

function CaptureAvdm() {
    var captureJson = {};
    var strWadh = "";
    var strOtp = "";
    Demo();
    txtWadh = "";
    if (txtWadh.trim() != "") {
        strWadh = " wadh=\"" + txtWadh + '"';
    }
    if (txtotp.trim() != "") {
        strOtp = " otp=\"" + txtotp + '"';
    }
    var XML = '';

    if (DeviceKonsi == "Mantra") {
        //Matra
        XML = '<?xml version="1.0"?> <PidOptions ver="1.0"> <Opts fCount="' + Fcount + '" fType="' + Ftype + '" iCount="' + Icount + '" pCount="' + Pcount + '" pgCount="' + pgCount + '"' + strOtp + ' format="' + Dtype + '"   pidVer="' + Pidver + '" timeout="' + Timeout + '" pTimeout="' + pTimeout + '"' + strWadh + ' posh="UNKNOWN" env="' + Env + '" /> ' + DemoFinalString + '<CustOpts><Param name="mantrakey" value="' + txtCK + '" /></CustOpts> </PidOptions>';
    } else if (DeviceKonsi == "Morpho") {
        //Morpho
        XML = '<PidOptions ver=\"1.0\">' + '<Opts fCount=\"1\" fType=\"' + Ftype + '\" iCount=\"\" iType=\"\" pCount=\"\" pType=\"\" format=\"0\" pidVer=\"2.0\" timeout=\"10000\" otp=\"\" wadh=\"\" posh=\"\"/>' + '</PidOptions>';
    }
    else if (DeviceKonsi == "Startek") {
        //Startek
        XML = '<PidOptions>' + '<Opts fCount=\"1\" fType=\"' + Ftype + '\"  iCount=\"0\" pCount=\"0\" format=\"0\" pidVer=\"2.0\" timeout=\"20000\" otp=\"\" posh=\"UNKNOWN\" env=\"P\" wadh=\"\" /> <Demo></Demo> <CustOpts> <Param name=\"ValidationKey\" value=\"\" /> </CustOpts>' + '</PidOptions>';
    }
    // url = ""; 
    var verb = "CAPTURE";
    var err = "";
    var res;
    $.support.cors = true;
    var httpStaus = false;
    var jsonstr = "";
    ;
    // $("#loadermoney").style.display = "block";//TODO
    $.ajax({
        type: "CAPTURE",
        async: false,
        crossDomain: true,
        url: finalUrl + MethodCapture,
        data: XML,
        contentType: "text/xml; charset=utf-8",
        processData: false,
        success: function (data) {
            //  document.getElementById("loadermoney").style.display = "none";
            //alert(data);
            httpStaus = true;
            res = { httpStaus: httpStaus, data: data };
            // $('#txtPidData').val(data); AJ
            txtPidData = data;
            //$('#txtPidOptions').val(XML); AJ
            txtPidOptions = XML;
            var $doc;
            var infochkkk;
            if (DeviceKonsi == "Mantra") {
                $doc = $.parseXML(data);
                infochkkk = data;

            }


            else if (DeviceKonsi == "Morpho") {
                $doc = data;
                const xmlText = new XMLSerializer().serializeToString(data);
                infochkkk = xmlText;
            }
            else if (DeviceKonsi == "Startek") {
                /*   console.log(data)*/
                $doc = data;
                infochkkk = data;
            }
            $("#capxml").val(infochkkk);

            var errInfo = $($doc).find('Resp').attr('errInfo');
            console.log("errInfo => " + errInfo);
            try {
                //console.log("================ PidOptions=======================");
                //console.log(txtPidOptions);
                //console.log("================= Capture data ======================");
                //console.log(txtPidData);
                //console.log("==================XML to JS =====================");
                if ($($doc).find('Resp').attr("errCode") == 0) {
                    captureJson.PidDatatype = $($doc).find('Data').attr('type');
                    captureJson.Piddata = $($doc).find('Data').text();
                    captureJson.ci = $($doc).find('Skey').attr('ci');
                    captureJson.dc = $($doc).find('DeviceInfo').attr("dc");
                    captureJson.dpID = $($doc).find('DeviceInfo').attr("dpId");
                    captureJson.errCode = $($doc).find('Resp').attr("errCode");
                    captureJson.errInfo = $($doc).find('Resp').attr("errInfo");
                    captureJson.fCount = $($doc).find('Resp').attr("fCount");
                    captureJson.fType = $($doc).find('Resp').attr("fType");
                    captureJson.hmac = $($doc).find('Hmac').text();
                    captureJson.iCount = 0;//$($doc).find('data');
                    captureJson.mc = $($doc).find('DeviceInfo').attr("mc");
                    captureJson.mi = $($doc).find('DeviceInfo').attr("mi");
                    captureJson.nmPoints = $($doc).find('Resp').attr("nmPoints");
                    captureJson.pCount = 0;//$($doc).find('data');
                    captureJson.pType = 0;//$($doc).find('data');
                    captureJson.iType = null;//$($doc).find('data');
                    captureJson.qScore = $($doc).find('Resp').attr("qScore");
                    captureJson.rdsID = $($doc).find('DeviceInfo').attr("rdsId");
                    captureJson.rdsVer = $($doc).find('DeviceInfo').attr("rdsVer");
                    captureJson.sessionKey = $($doc).find('Skey').text();
                    //captureJson.env = "p";
                    //console.log(captureJson);
                    //console.log(JSON.stringify(captureJson));
                    try {
                        //  console.log("sr no : " + $($doc).find('DeviceInfo additional_info Param').attr("value"));

                        $("#devicesrno").val($($doc).find('DeviceInfo additional_info Param').attr("value"));
                    } catch (err) { }

                    $('#txtdevice').val(DeviceKonsi);
                    $("#capAEPS").val(JSON.stringify(captureJson));
                    $("#payaeps").show();
                    $("#scanimage").hide();
                    $('#AEPSFOUND').show();
                    $("#Devicerror").hide();
                    //  $("#fingImgBlack").hide();
                    //  $("#fingImgGreen").show();

                    //   $("#AEPS").submit();

                } else {
                    $("#Devicerror").show();
                    $('#AEPSFOUND').hide();
                    $('#deviceerror8').text($($doc).find('Resp').attr("errInfo"));
                    $('#deviceerror9').text("Please Try again");
                    $('#againscanclick').text("scan again");
                    $('#deviceerror32').text(errInfo);
                }
            } catch (err) { console.log(err); }
        },
        error: function (jqXHR, ajaxOptions, thrownError) {
            //$('#txtPidOptions').val(XML);

            alert(thrownError);
            res = { httpStaus: httpStaus, err: getHttpError(jqXHR) };
        },
    });

    return res;
}
function getHttpError(jqXHR) {
    var err = "Unhandled Exception";
    if (jqXHR.status === 0) {
        err = 'Service Unavailable';
    } else if (jqXHR.status == 404) {
        err = 'Requested page not found';
    } else if (jqXHR.status == 500) {
        err = 'Internal Server Error';
    } else if (thrownError === 'parsererror') {
        err = 'Requested JSON parse failed';
    } else if (thrownError === 'timeout') {
        err = 'Time out error';
    } else if (thrownError === 'abort') {
        err = 'Ajax request aborted';
    } else {
        err = 'Unhandled Error';
    }
    return err;
}
function scanfing() {

    //$("#fingImgGreen").hide();
    //$("#fingImgBlack").show();

    //if (!$("#remember_me_2").is(":checked")) {
    //    $("#remember_me_2-error").show();
    //    return false;
    //}
    $('#showspinneraeps').show();
    discoverAvdm();
    if (RdStatus) {
        $("#response2").text("Tap on " + DeviceKonsi + " scanner !");
        //alert("Tap on " + DeviceKonsi + " scanner !");
        //document.getElementById("loadermoney").style.display = "block";
        CaptureAvdm();
        $('#showspinneraeps').hide();
        $("#loadaeps").hide();

        //  $('#AEPSFOUND').show();

    }
    else {
        $('#showspinneraeps').hide();
        $("#loadaeps").hide();
        $("#Devicerror").show();
        $('#AEPSFOUND').hide();
        $("#deviceerror1").text(" Device not found (check the following list)");
        $("#deviceerror3").text("➁ Check your device connection");
        $("#deviceerror4").text("➂ Check device power supply status");
        $("#deviceerror5").text("➃ Check if drivers are installed correctly");
        $("#deviceerror6").text("➄ You may not have scanned the finger in scan timing");
        $("#deviceerror7").text("➅ There may be a connectivity problem with the bank");
        //alert("Device not found!!");
    }
    //alert("Scan done");
}
function bankChange() {
    $("#bank").val($("#iin option:selected").text());
}
$("input[name='type']").on('click', function () {
    var radioValue = $("input[name='type']:checked").val();
    if (radioValue == "CW") {
        $('#amountParent').show();
    } else {
        $('#amountParent').hide();
    }
});